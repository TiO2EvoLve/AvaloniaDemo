using System.Collections.Specialized;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace AvaloniaTestDemo.Views;

public partial class ReactiveView : UserControl
{
    public ReactiveView()
    {
        InitializeComponent();
        
        AttachedToVisualTree += (_, _) =>
        {
            if (DataContext is ReactiveViewModel vm)
            {
                vm.LogContent.CollectionChanged += OnLogChanged;
            }
        };
    }
    
    private void OnLogChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        // 必须切回 UI 线程
        Dispatcher.UIThread.Post(() =>
        {
            LogScroll.ScrollToEnd();
        });
    }
}