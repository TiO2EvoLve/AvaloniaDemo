using System;
using System.Diagnostics;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using Markdown.Avalonia.SyntaxHigh.StyleCollections;
using MdEngine = Markdown.Avalonia.Markdown;
using MdPlugins = Markdown.Avalonia.Full.MdAvPlugins;
using MdStyle = Markdown.Avalonia.MarkdownStyle;

namespace AvaloniaTestDemo.Controls;

/// <summary>
/// 一个不包含独立滚动条的 Markdown 渲染控件，便于放进聊天消息列表。
/// 内部使用 Markdown.Avalonia 将 Markdown 转成 Avalonia 控件。
/// </summary>
public class MarkdownPresenter : ContentControl
{
    private static readonly ICommand OpenLinkCommand = new RelayCommand<string>(OpenLink);

    public static readonly StyledProperty<string?> MarkdownProperty =
        AvaloniaProperty.Register<MarkdownPresenter, string?>(nameof(Markdown));

    private readonly MdEngine _engine;
    private readonly DispatcherTimer _renderTimer;
    private bool _isAttached;
    private bool _isDirty;
    private bool _hasRendered;

    public MarkdownPresenter()
    {
        HorizontalContentAlignment = HorizontalAlignment.Stretch;
        VerticalContentAlignment = VerticalAlignment.Stretch;
        Padding = new Thickness(0);

        var plugins = new MdPlugins
        {
            HyperlinkCommand = OpenLinkCommand
        };

        _engine = new MdEngine
        {
            Plugins = plugins,
            HyperlinkCommand = OpenLinkCommand
        };

        Styles.Add(MdStyle.FluentTheme);
        Styles.Add(new AppendixOfFluentTheme());

        // 流式输出时文本变化非常频繁，合并到约 10fps 重新排版，避免每个 token 都重建整棵 Markdown 视觉树。
        _renderTimer = new DispatcherTimer(DispatcherPriority.Background)
        {
            Interval = TimeSpan.FromMilliseconds(100)
        };
        _renderTimer.Tick += OnRenderTimerTick;
    }

    public string? Markdown
    {
        get => GetValue(MarkdownProperty);
        set => SetValue(MarkdownProperty, value);
    }

    /// <summary>Markdown 完成一次重新渲染后触发，可用于让外层聊天列表滚动到底部。</summary>
    public event EventHandler? Rendered;

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property != MarkdownProperty)
        {
            return;
        }

        _isDirty = true;

        if (_isAttached && !_renderTimer.IsEnabled)
        {
            _renderTimer.Start();
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        _isAttached = true;

        // 控件可能在挂载到可视树之前就收到了 Markdown 内容。
        if (_isDirty)
        {
            _isDirty = false;
            RenderMarkdown();
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _isAttached = false;
        _renderTimer.Stop();

        base.OnDetachedFromVisualTree(e);
    }

    private void OnRenderTimerTick(object? sender, EventArgs e)
    {
        if (!_isAttached || !_isDirty)
        {
            _renderTimer.Stop();
            return;
        }

        _isDirty = false;
        RenderMarkdown();
    }

    private void RenderMarkdown()
    {
        var markdown = Markdown;

        if (string.IsNullOrWhiteSpace(markdown))
        {
            if (_hasRendered)
            {
                Content = null;
                _hasRendered = false;
            }

            return;
        }

        try
        {
            var control = _engine.Transform(markdown);
            control.Classes.Add("Markdown_Avalonia_MarkdownViewer");
            Content = control;
            _hasRendered = true;
        }
        catch (Exception)
        {
            // 流式输出时可能遇到尚未闭合的 Markdown 语法；解析异常时退回纯文本，保证对话不中断。
            Content = new SelectableTextBlock
            {
                Text = markdown,
                TextWrapping = Avalonia.Media.TextWrapping.Wrap
            };
            _hasRendered = true;
        }

        Rendered?.Invoke(this, EventArgs.Empty);
    }

    private static void OpenLink(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return;
        }

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        catch
        {
            // 忽略无法打开的非 HTTP 链接（例如文档内锚点）。
        }
    }
}
