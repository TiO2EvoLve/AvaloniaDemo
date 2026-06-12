using System;

namespace AvaloniaTestDemo.Services;

public static class HangfireDemoJobs
{
    public static event Action<string>? JobLogged;

    public static void RunImmediate(string message)
    {
        Log("立即任务", message);
    }

    public static void RunDelayed(string message)
    {
        Log("延迟任务", message);
    }

    public static void RunRecurring(string message)
    {
        Log("周期任务", message);
    }

    private static void Log(string jobType, string message)
    {
        var entry = $"[{jobType}] {DateTime.Now:HH:mm:ss} - {message}";
        JobLogged?.Invoke(entry);
    }
}
