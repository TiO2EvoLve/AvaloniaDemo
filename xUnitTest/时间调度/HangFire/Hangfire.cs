using Hangfire;
using Hangfire.MemoryStorage;
using Xunit;

namespace xUnitTest.时间调度.HangFire;

public static class HangfireJob
{
    public static void Print(string message)
    {
        Console.WriteLine($"{DateTime.Now:HH:mm:ss} -> {message}");
    }
}

public class HangfireTests : IDisposable
{
    private static readonly BackgroundJobServer Server;

    static HangfireTests()
    {
        GlobalConfiguration.Configuration
            .UseMemoryStorage();

        Server = new BackgroundJobServer();
    }

    /// <summary>
    /// 立即执行
    /// </summary>
    [Fact]
    public void Enqueue_Test()
    {
        BackgroundJob.Enqueue(
            () => HangfireJob.Print("立即执行"));

        Thread.Sleep(1000);
    }

    /// <summary>
    /// 延迟执行
    /// </summary>
    [Fact]
    public void Delay_Test()
    {
        BackgroundJob.Schedule(
            () => HangfireJob.Print("3秒后执行"),
            TimeSpan.FromSeconds(3));

        Thread.Sleep(5000);
    }

    /// <summary>
    /// 指定时间执行
    /// </summary>
    [Fact]
    public void ScheduleAt_Test()
    {
        var runAt = DateTimeOffset.Now.AddSeconds(3);

        BackgroundJob.Schedule(
            () => HangfireJob.Print($"计划时间：{runAt:HH:mm:ss}"),
            runAt);

        Thread.Sleep(5000);
    }

    /// <summary>
    /// 周期任务
    /// </summary>
    [Fact]
    public void Recurring_Test()
    {
        RecurringJob.AddOrUpdate(
            "demo",
            () => HangfireJob.Print("周期任务"),
            "*/5 * * * * *"); // 每5秒

        Thread.Sleep(12000);

        RecurringJob.RemoveIfExists("demo");
    }

    /// <summary>
    /// 延续任务
    /// </summary>
    [Fact]
    public void Continuation_Test()
    {
        var parentId = BackgroundJob.Enqueue(
            () => HangfireJob.Print("父任务"));

        BackgroundJob.ContinueJobWith(
            parentId,
            () => HangfireJob.Print("子任务"));

        Thread.Sleep(2000);
    }

    /// <summary>
    /// 删除未执行任务
    /// </summary>
    [Fact]
    public void Delete_Test()
    {
        var jobId = BackgroundJob.Schedule(
            () => HangfireJob.Print("不会执行"),
            TimeSpan.FromMinutes(1));

        var result = BackgroundJob.Delete(jobId);

        Console.WriteLine($"删除结果: {result}");
    }

    public void Dispose()
    {
    }
}