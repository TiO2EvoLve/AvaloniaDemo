using System;
using System.Threading.Tasks;
using Quartz;

namespace AvaloniaTestDemo.Services;

public static class QuartzDemoJobs
{
    public static event Action<string>? JobLogged;

    public static void Log(string jobType, string message)
    {
        var entry = $"[{jobType}] {DateTime.Now:HH:mm:ss} - {message}";
        JobLogged?.Invoke(entry);
    }
}

[DisallowConcurrentExecution]
public class QuartzDemoJob : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        var jobType = context.MergedJobDataMap.GetString("jobType") ?? "任务";
        var message = context.MergedJobDataMap.GetString("message") ?? string.Empty;
        QuartzDemoJobs.Log(jobType, message);
        return Task.CompletedTask;
    }
}
