using System;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Material.Icons;

namespace AvaloniaTestDemo.Views;

public partial class CancellationTokenViewModel() : DemoPageBase("消息取消", MaterialIconKind.CalendarCheckOutline,int.MinValue)
{
    private CancellationTokenSource? _cts;
    
    [ObservableProperty]
    private int _progress;

    [ObservableProperty]
    private string _status = "未开始";

    [RelayCommand]
    private async Task StartAsync()
    {
        // 如果已有任务在运行，忽略
        if (_cts != null)
            return;

        _cts = new CancellationTokenSource();
        Progress = 0;
        Status = "运行中...";

        try
        {
            // 模拟一个会报告进度的异步操作
            for (int i = 1; i <= 100; i++)
            {
                _cts.Token.ThrowIfCancellationRequested();
                await Task.Delay(50, _cts.Token); // 模拟工作
                Progress = i;
            }
            Status = "已完成";
        }
        catch (OperationCanceledException)
        {
            Status = "已取消";
            Progress = 0;
        }
        finally
        {
            _cts.Dispose();
            _cts = null;
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        if (_cts == null)
        {
            Status = "没有正在运行的任务";
            return;
        }

        _cts.Cancel();
        Status = "正在取消...";
    }
}