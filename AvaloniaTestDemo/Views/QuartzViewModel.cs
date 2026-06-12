using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Threading;
using AvaloniaTestDemo.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Material.Icons;
using Quartz;

namespace AvaloniaTestDemo.Views;

public partial class QuartzViewModel : DemoPageBase
{
    private const string DemoGroup = "demo";
    private const string RecurringJobName = "recurring-job";
    private const string RecurringTriggerName = "recurring-trigger";

    private readonly IScheduler _scheduler;

    public ObservableCollection<string> JobLogs { get; } = [];

    [ObservableProperty]
    private string _status = "Quartz 调度器已就绪";

    [ObservableProperty]
    private bool _isRecurringActive;

    public QuartzViewModel(QuartzService quartzService) : base("Quartz 调度", MaterialIconKind.TimerOutline, int.MinValue)
    {
        _scheduler = quartzService.Scheduler;
        QuartzDemoJobs.JobLogged += OnJobLogged;
    }

    [RelayCommand]
    private async Task EnqueueImmediateAsync()
    {
        var id = Guid.NewGuid().ToString("N")[..8];
        var job = CreateJob($"immediate-{id}", "立即任务", "后台立即执行");
        var trigger = TriggerBuilder.Create()
            .WithIdentity($"immediate-trigger-{id}", DemoGroup)
            .StartNow()
            .Build();

        await _scheduler.ScheduleJob(job, trigger);
        Status = "已调度一个立即任务";
    }

    [RelayCommand]
    private async Task ScheduleDelayedAsync()
    {
        var id = Guid.NewGuid().ToString("N")[..8];
        var job = CreateJob($"delayed-{id}", "延迟任务", "5 秒后执行");
        var trigger = TriggerBuilder.Create()
            .WithIdentity($"delayed-trigger-{id}", DemoGroup)
            .StartAt(DateTimeOffset.Now.AddSeconds(5))
            .Build();

        await _scheduler.ScheduleJob(job, trigger);
        Status = "已调度一个 5 秒后执行的任务";
    }

    [RelayCommand]
    private async Task StartRecurringAsync()
    {
        var job = CreateJob(RecurringJobName, "周期任务", "每 30 秒触发");
        var trigger = TriggerBuilder.Create()
            .WithIdentity(RecurringTriggerName, DemoGroup)
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInSeconds(30)
                .RepeatForever())
            .Build();

        await _scheduler.ScheduleJob(job, trigger);
        IsRecurringActive = true;
        Status = "已启动周期任务（每 30 秒）";
    }

    [RelayCommand]
    private async Task StopRecurringAsync()
    {
        var jobKey = new JobKey(RecurringJobName, DemoGroup);
        if (await _scheduler.CheckExists(jobKey))
        {
            await _scheduler.DeleteJob(jobKey);
        }

        IsRecurringActive = false;
        Status = "已停止周期任务";
    }

    [RelayCommand]
    private void ClearLogs()
    {
        JobLogs.Clear();
        Status = "日志已清空";
    }

    private static IJobDetail CreateJob(string name, string jobType, string message) =>
        JobBuilder.Create<QuartzDemoJob>()
            .WithIdentity(name, DemoGroup)
            .UsingJobData("jobType", jobType)
            .UsingJobData("message", message)
            .Build();

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
