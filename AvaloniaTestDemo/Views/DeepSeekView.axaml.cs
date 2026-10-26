using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;

namespace AvaloniaTestDemo.Views;

public partial class DeepSeekView : UserControl
{
    private DeepSeekViewModel? _viewModel;
    private bool _isSubscribed;
    private bool _scrollQueued;

    public DeepSeekView()
    {
        InitializeComponent();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        _viewModel = DataContext as DeepSeekViewModel;
        Subscribe();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        Unsubscribe();

        base.OnDataContextChanged(e);

        _viewModel = DataContext as DeepSeekViewModel;
        Subscribe();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        Unsubscribe();

        base.OnDetachedFromVisualTree(e);
    }

    private void Subscribe()
    {
        if (_isSubscribed || _viewModel is null)
        {
            return;
        }

        _viewModel.ConversationUpdated += OnConversationUpdated;
        _isSubscribed = true;
    }

    private void Unsubscribe()
    {
        if (!_isSubscribed || _viewModel is null)
        {
            return;
        }

        _viewModel.ConversationUpdated -= OnConversationUpdated;
        _isSubscribed = false;
    }

    /// <summary>
    /// 流式输出时内容增长很快，这里用 Dispatcher 合并滚动请求，避免每个 token 都排队滚动。
    /// 用户主动向上滚动阅读时不强制拉回底部。
    /// </summary>
    private void OnConversationUpdated(object? sender, EventArgs e)
    {
        if (_scrollQueued)
        {
            return;
        }

        _scrollQueued = true;

        Dispatcher.UIThread.Post(() =>
        {
            _scrollQueued = false;

            if (ChatScrollViewer is null)
            {
                return;
            }

            var distanceToBottom = ChatScrollViewer.Extent.Height
                                   - ChatScrollViewer.Viewport.Height
                                   - ChatScrollViewer.Offset.Y;

            if (distanceToBottom <= 96)
            {
                ChatScrollViewer.ScrollToEnd();
            }
        }, DispatcherPriority.Background);
    }

    private void MarkdownPresenter_OnRendered(object? sender, EventArgs e)
    {
        // Markdown 重新排版后消息高度会变化，再触发一次滚动到底部。
        OnConversationUpdated(sender, e);
    }

    private void InputBox_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (DataContext is not DeepSeekViewModel viewModel)
        {
            return;
        }

        if (e.Key == Key.Enter && e.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            e.Handled = true;
            if (viewModel.SendCommand.CanExecute(null))
            {
                viewModel.SendCommand.Execute(null);
            }
        }
        else if (e.Key == Key.Escape && viewModel.IsBusy)
        {
            e.Handled = true;
            if (viewModel.SendCancelCommand.CanExecute(null))
            {
                viewModel.SendCancelCommand.Execute(null);
            }
        }
    }
}
