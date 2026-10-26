using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaTestDemo.Views;

/// <summary>
/// 对话列表中的一条消息。助手消息额外保存思考过程（reasoning_content）。
/// </summary>
public partial class DeepSeekChatMessage : ObservableObject
{
    public DeepSeekChatMessage(bool isUser, string content = "")
    {
        IsUser = isUser;
        _content = content;
        CreatedAt = DateTimeOffset.Now;
    }

    public bool IsUser { get; }

    public bool IsAssistant => !IsUser;

    public string Role => IsUser ? "user" : "assistant";

    public DateTimeOffset CreatedAt { get; }

    public string TimeText => CreatedAt.ToString("HH:mm");

    [ObservableProperty]
    private string _content;

    /// <summary>模型返回的思维链内容。</summary>
    [ObservableProperty]
    private string _reasoning = string.Empty;

    [ObservableProperty]
    private bool _isStreaming;

    /// <summary>思考过程默认展开，方便实时查看；用户可以手动收起。</summary>
    [ObservableProperty]
    private bool _isReasoningExpanded = true;

    /// <summary>消息末尾的提示，例如“已停止生成”“请求失败”。不会作为上下文回传。</summary>
    [ObservableProperty]
    private string _note = string.Empty;

    public bool HasReasoning => !string.IsNullOrWhiteSpace(Reasoning);

    public bool HasContent => !string.IsNullOrWhiteSpace(Content);

    public bool HasNote => !string.IsNullOrWhiteSpace(Note);

    partial void OnReasoningChanged(string value)
    {
        OnPropertyChanged(nameof(HasReasoning));
    }

    partial void OnContentChanged(string value)
    {
        OnPropertyChanged(nameof(HasContent));
    }

    partial void OnNoteChanged(string value)
    {
        OnPropertyChanged(nameof(HasNote));
    }
}
