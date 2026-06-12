using System;
using System.Collections.ObjectModel;
using Avalonia.Threading;
using AvaloniaTestDemo.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hangfire;
using Material.Icons;

namespace AvaloniaTestDemo.Views;

public partial class HangfireViewModel : DemoPageBase
{
    private const string RecurringJobId = "demo-recurring-job";

    public ObservableCollection<string> JobLogs { get; } = [];

    [ObservableProperty]
    private string _status = "Hangfire 后台服务已就绪";

    [ObservableProperty]
    private bool _isRecurringActive;

    public HangfireViewModel(HangfireService _) : base("Hangfire 调度", MaterialIconKind.ClockOutline, int.MinValue)
    {
        HangfireDemoJobs.JobLogged += OnJobLogged;
    }

    [RelayCommand]
    private void EnqueueImmediate()
    {
        BackgroundJob.Enqueue(() => HangfireDemoJobs.RunImmediate("后台立即执行"));
        Status = "已入队一个立即任务";
    }

    [RelayCommand]
    private void ScheduleDelayed()
    {
        BackgroundJob.Schedule(
            () => HangfireDemoJobs.RunDelayed("5 秒后执行"),
            TimeSpan.FromSeconds(5));
        Status = "已调度一个 5 秒后执行的任务";
    }

    [RelayCommand]
    private void StartRecurring()
    {
        RecurringJob.AddOrUpdate(
            RecurringJobId,
            () => HangfireDemoJobs.RunRecurring("每分钟触发"),
            "0 * * * * *");
        IsRecurringActive = true;
        Status = "已启动周期任务（每 1 分）";
    }

    [RelayCommand]
    private void StopRecurring()
    {
        RecurringJob.RemoveIfExists(RecurringJobId);
        IsRecurringActive = false;
        Status = "已停止周期任务";
    }

    [RelayCommand]
    private void ClearLogs()
    {
        JobLogs.Clear();
        Status = "日志已清空";
    }

    private void OnJobLogged(string message)
    {
        Dispatcher.UIThread.Post(() =>
        {
            JobLogs.Insert(0, message);
            if (JobLogs.Count > 100)
            {
                JobLogs.RemoveAt(JobLogs.Count - 1);
            }
        });
    }
}
