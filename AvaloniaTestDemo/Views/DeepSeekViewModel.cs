using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AvaloniaTestDemo.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Material.Icons;

namespace AvaloniaTestDemo.Views;

public partial class DeepSeekViewModel : DemoPageBase
{
    private const string SystemPrompt =
        "你是一个乐于助人的 AI 助手。请使用简体中文回答，回答要准确、条理清晰。";

    private readonly DeepSeekChatService _chatService = new();

    public DeepSeekViewModel() : base("AI 对话", MaterialIconKind.RobotAngry)
    {
        Messages.CollectionChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(IsConversationEmpty));
            OnPropertyChanged(nameof(HasMessages));
            ClearCommand.NotifyCanExecuteChanged();
            RequestScrollToEnd();
        };
    }

    public ObservableCollection<DeepSeekChatMessage> Messages { get; } = [];

    public IReadOnlyList<string> ModelOptions { get; } = ["deepseek-flash", "deepseek-v4-pro"];

    [ObservableProperty]
    private string _selectedModel = "deepseek-flash";

    [ObservableProperty]
    private string _apiKey = "sk-53573fcceb8c48dfa36b0b9bab727ab1";

    [ObservableProperty]
    private bool _enableThinking = true;

    [ObservableProperty]
    private string _input = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _statusText = "就绪";

    [ObservableProperty]
    private string _usageText = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public bool IsConversationEmpty => Messages.Count == 0;

    public bool HasMessages => Messages.Count > 0;

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public bool HasUsage => !string.IsNullOrWhiteSpace(UsageText);

    /// <summary>消息新增或流式内容更新时触发，供视图自动滚动到底部。</summary>
    public event EventHandler? ConversationUpdated;

    private bool CanSend => !IsBusy && !string.IsNullOrWhiteSpace(Input);

    private bool CanClear => !IsBusy && Messages.Count > 0;

    partial void OnInputChanged(string value)
    {
        SendCommand.NotifyCanExecuteChanged();
    }

    partial void OnIsBusyChanged(bool value)
    {
        SendCommand.NotifyCanExecuteChanged();
        ClearCommand.NotifyCanExecuteChanged();
    }

    partial void OnErrorMessageChanged(string value)
    {
        OnPropertyChanged(nameof(HasError));
    }

    partial void OnUsageTextChanged(string value)
    {
        OnPropertyChanged(nameof(HasUsage));
    }

    [RelayCommand(IncludeCancelCommand = true, CanExecute = nameof(CanSend))]
    private async Task SendAsync(CancellationToken cancellationToken)
    {
        var prompt = Input.Trim();
        if (prompt.Length == 0)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(ApiKey))
        {
            ErrorMessage = "请先填写 DeepSeek API Key";
            return;
        }

        ErrorMessage = string.Empty;
        UsageText = string.Empty;
        Input = string.Empty;

        var userMessage = new DeepSeekChatMessage(true, prompt);
        Messages.Add(userMessage);

        var assistantMessage = new DeepSeekChatMessage(false) { IsStreaming = true };
        Messages.Add(assistantMessage);

        IsBusy = true;
        StatusText = EnableThinking ? "正在思考…" : "正在生成…";
        RequestScrollToEnd();

        // 只回传正文，不回传 reasoning_content（接口会忽略它），避免浪费上下文 token。
        var history = Messages
            .Where(message => !ReferenceEquals(message, assistantMessage))
            .Where(message => !string.IsNullOrWhiteSpace(message.Content))
            .Select(message => new DeepSeekRequestMessage(message.Role, message.Content))
            .ToList();

        try
        {
            var request = new DeepSeekChatRequest(
                ApiKey.Trim(),
                SelectedModel,
                history,
                EnableThinking,
                SystemPrompt);

            await foreach (var chunk in _chatService.StreamAsync(request, cancellationToken))
            {
                if (!string.IsNullOrEmpty(chunk.ReasoningContent))
                {
                    assistantMessage.Reasoning += chunk.ReasoningContent;
                    StatusText = "正在思考…";
                }

                if (!string.IsNullOrEmpty(chunk.Content))
                {
                    assistantMessage.Content += chunk.Content;
                    StatusText = "正在回答…";
                }

                if (chunk.Usage is { } usage)
                {
                    UsageText = usage.ToString();
                }

                if (!string.IsNullOrEmpty(chunk.FinishReason) && chunk.FinishReason != "stop")
                {
                    StatusText = chunk.FinishReason switch
                    {
                        "length" => "输出达到长度上限",
                        "content_filter" => "内容被安全策略过滤",
                        "insufficient_system_resource" => "服务资源不足，生成被中断",
                        "aborted" => "生成已中止",
                        _ => $"生成结束：{chunk.FinishReason}"
                    };
                }

                RequestScrollToEnd();
            }

            if (!assistantMessage.HasContent && !assistantMessage.HasReasoning)
            {
                assistantMessage.Note = "模型没有返回内容，请重试。";
            }
            else if (!assistantMessage.HasContent)
            {
                assistantMessage.Note = "模型只返回了思考过程，没有返回正文。";
            }

            if (StatusText is "正在思考…" or "正在回答…")
            {
                StatusText = "完成";
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            StatusText = "已停止生成";
            assistantMessage.Note = "已停止生成";
        }
        catch (OperationCanceledException)
        {
            ErrorMessage = "请求超时（5 分钟）或连接被中断，请重试。";
            StatusText = "请求失败";
            assistantMessage.Note = "请求超时或连接中断";
        }
        catch (DeepSeekApiException ex)
        {
            ErrorMessage = ex.Message;
            StatusText = "请求失败";
            assistantMessage.Note = "请求失败，详情见下方错误提示";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"调用 DeepSeek API 时发生错误：{ex.Message}";
            StatusText = "请求失败";
            assistantMessage.Note = "请求失败，详情见下方错误提示";
        }
        finally
        {
            assistantMessage.IsStreaming = false;
            IsBusy = false;
            RequestScrollToEnd();
        }
    }

    [RelayCommand(CanExecute = nameof(CanClear))]
    private void Clear()
    {
        Messages.Clear();
        UsageText = string.Empty;
        ErrorMessage = string.Empty;
        StatusText = "就绪";
    }

    private void RequestScrollToEnd()
    {
        ConversationUpdated?.Invoke(this, EventArgs.Empty);
    }
}
