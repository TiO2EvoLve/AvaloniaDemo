using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaTestDemo.Services;

/// <summary>
/// 发送给 DeepSeek Chat Completions 接口的一条历史消息。
/// 注意：思考内容（reasoning_content）不需要回传，接口会忽略它。
/// </summary>
public sealed record DeepSeekRequestMessage(string Role, string Content);

public sealed record DeepSeekChatRequest(
    string ApiKey,
    string Model,
    IReadOnlyList<DeepSeekRequestMessage> Messages,
    bool EnableThinking,
    string? SystemPrompt = null);

public sealed record DeepSeekUsage(
    int PromptTokens,
    int CompletionTokens,
    int ReasoningTokens,
    int TotalTokens)
{
    public override string ToString()
    {
        var text = $"输入 {PromptTokens} · 输出 {CompletionTokens} · 总计 {TotalTokens} tokens";
        return ReasoningTokens > 0 ? $"{text} · 思考 {ReasoningTokens} tokens" : text;
    }
}

public sealed record DeepSeekStreamChunk(
    string? Content,
    string? ReasoningContent,
    string? FinishReason,
    DeepSeekUsage? Usage);

public sealed class DeepSeekApiException(string message, int? statusCode = null) : Exception(message)
{
    public int? StatusCode { get; } = statusCode;
}

/// <summary>
/// DeepSeek 流式对话客户端。解析 SSE 中的 content / reasoning_content 增量。
/// </summary>
public sealed class DeepSeekChatService
{
    private const string Endpoint = "https://api.deepseek.com/chat/completions";

    private static readonly HttpClient SharedHttpClient = new()
    {
        Timeout = TimeSpan.FromMinutes(5)
    };

    private readonly HttpClient _httpClient;

    public DeepSeekChatService(HttpClient? httpClient = null)
    {
        _httpClient = httpClient ?? SharedHttpClient;
    }

    /// <summary>
    /// 发起流式对话。每个 chunk 可能只包含思考内容、正文内容或用量信息。
    /// </summary>
    public async IAsyncEnumerable<DeepSeekStreamChunk> StreamAsync(
        DeepSeekChatRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.ApiKey))
        {
            throw new DeepSeekApiException("请先配置 DeepSeek API Key。");
        }

        var messages = new List<object>(request.Messages.Count + 1);

        if (!string.IsNullOrWhiteSpace(request.SystemPrompt))
        {
            messages.Add(new { role = "system", content = request.SystemPrompt });
        }

        foreach (var message in request.Messages)
        {
            messages.Add(new { role = message.Role, content = message.Content });
        }

        var json = JsonSerializer.Serialize(new
        {
            model = request.Model,
            messages,
            stream = true,
            // DeepSeek V4 使用 thinking 控制思考模式，默认开启。
            thinking = new { type = request.EnableThinking ? "enabled" : "disabled" },
            stream_options = new { include_usage = true }
        });

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, Endpoint)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", request.ApiKey.Trim());
        httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/event-stream"));

        using var response = await _httpClient
            .SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
            .ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content
                .ReadAsStringAsync(cancellationToken)
                .ConfigureAwait(false);

            throw new DeepSeekApiException(
                BuildErrorMessage(response.StatusCode, errorBody),
                (int)response.StatusCode);
        }

        await using var stream = await response.Content
            .ReadAsStreamAsync(cancellationToken)
            .ConfigureAwait(false);
        using var reader = new StreamReader(stream, Encoding.UTF8);

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var line = await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false);
            if (line is null)
            {
                yield break;
            }

            // SSE：只处理 data 行，空行和以 ':' 开头的注释行直接忽略。
            if (line.Length == 0 || line[0] == ':' || !line.StartsWith("data:", StringComparison.Ordinal))
            {
                continue;
            }

            var payload = line[5..].Trim();
            if (payload.Length == 0)
            {
                continue;
            }

            if (payload == "[DONE]")
            {
                yield break;
            }

            DeepSeekStreamChunk? chunk;
            try
            {
                chunk = ParseChunk(payload);
            }
            catch (JsonException)
            {
                // 跳过无法解析的行，避免单行异常中断整个流。
                continue;
            }

            if (chunk is not null)
            {
                yield return chunk;
            }
        }
    }

    private static DeepSeekStreamChunk? ParseChunk(string payload)
    {
        using var document = JsonDocument.Parse(payload);
        var root = document.RootElement;

        if (root.ValueKind == JsonValueKind.Object &&
            root.TryGetProperty("error", out var error) &&
            error.ValueKind == JsonValueKind.Object)
        {
            var message = error.TryGetProperty("message", out var errorMessage)
                ? errorMessage.GetString()
                : "接口返回了未知错误。";
            throw new DeepSeekApiException(message ?? "接口返回了未知错误。");
        }

        string? content = null;
        string? reasoningContent = null;
        string? finishReason = null;
        DeepSeekUsage? usage = null;

        if (root.ValueKind == JsonValueKind.Object &&
            root.TryGetProperty("choices", out var choices) &&
            choices.ValueKind == JsonValueKind.Array &&
            choices.GetArrayLength() > 0 &&
            choices[0].ValueKind == JsonValueKind.Object)
        {
            var choice = choices[0];

            if (choice.TryGetProperty("delta", out var delta) && delta.ValueKind == JsonValueKind.Object)
            {
                if (delta.TryGetProperty("content", out var contentElement) &&
                    contentElement.ValueKind == JsonValueKind.String)
                {
                    content = contentElement.GetString();
                }

                if (delta.TryGetProperty("reasoning_content", out var reasoningElement) &&
                    reasoningElement.ValueKind == JsonValueKind.String)
                {
                    reasoningContent = reasoningElement.GetString();
                }
            }

            if (choice.TryGetProperty("finish_reason", out var finishElement) &&
                finishElement.ValueKind == JsonValueKind.String)
            {
                finishReason = finishElement.GetString();
            }
        }

        if (root.TryGetProperty("usage", out var usageElement) && usageElement.ValueKind == JsonValueKind.Object)
        {
            var reasoningTokens = 0;
            if (usageElement.TryGetProperty("completion_tokens_details", out var details) &&
                details.ValueKind == JsonValueKind.Object)
            {
                reasoningTokens = GetInt32(details, "reasoning_tokens");
            }

            usage = new DeepSeekUsage(
                GetInt32(usageElement, "prompt_tokens"),
                GetInt32(usageElement, "completion_tokens"),
                reasoningTokens,
                GetInt32(usageElement, "total_tokens"));
        }

        if (content is null && reasoningContent is null && finishReason is null && usage is null)
        {
            return null;
        }

        return new DeepSeekStreamChunk(content, reasoningContent, finishReason, usage);
    }

    private static int GetInt32(JsonElement element, string propertyName)
    {
        return element.TryGetProperty(propertyName, out var property) && property.TryGetInt32(out var value)
            ? value
            : 0;
    }

    private static string BuildErrorMessage(HttpStatusCode statusCode, string responseBody)
    {
        string? apiMessage = null;

        try
        {
            using var document = JsonDocument.Parse(responseBody);
            if (document.RootElement.ValueKind == JsonValueKind.Object &&
                document.RootElement.TryGetProperty("error", out var error) &&
                error.ValueKind == JsonValueKind.Object &&
                error.TryGetProperty("message", out var message))
            {
                apiMessage = message.GetString();
            }
        }
        catch (JsonException)
        {
            // 非 JSON 错误响应，忽略。
        }

        var messageBuilder = new StringBuilder();
        messageBuilder.Append($"DeepSeek 请求失败（HTTP {(int)statusCode}）");

        if (!string.IsNullOrWhiteSpace(apiMessage))
        {
            messageBuilder.Append("：").Append(apiMessage);
        }

        var hint = statusCode switch
        {
            HttpStatusCode.Unauthorized => "API Key 无效或已过期，请检查后重试。",
            HttpStatusCode.PaymentRequired => "账户余额不足，请前往 DeepSeek 开放平台充值。",
            HttpStatusCode.TooManyRequests => "请求过于频繁或并发过高，请稍后再试。",
            HttpStatusCode.BadRequest => "请求参数有误，请检查模型名称与账号权限。",
            _ => null
        };

        if (hint is not null)
        {
            messageBuilder.Append(' ').Append(hint);
        }

        return messageBuilder.ToString();
    }
}
