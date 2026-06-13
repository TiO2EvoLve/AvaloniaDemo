using Quartz;
using Quartz.Impl;

namespace xUnitTest.时间调度.Quartz;

public class PrintJob : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        var message = context.JobDetail.JobDataMap.GetString("msg");

        Console.WriteLine(
            $"{DateTime.Now:HH:mm:ss} -> {message}");

        return Task.CompletedTask;
    }
}

public class Quartz
{
    /// <summary>
    /// 立即执行
    /// </summary>
    [Fact]
    public async Task RunNow_Test()
    {
        var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
        await scheduler.Start();

        var job = JobBuilder.Create<PrintJob>()
            .UsingJobData("msg", "立即执行")
            .Build();

        await scheduler.ScheduleJob(
            job,
            TriggerBuilder.Create()
                .StartNow()
                .Build());

        await Task.Delay(1000);

        await scheduler.Shutdown();
    }

    /// <summary>
    /// 延迟执行
    /// </summary>
    [Fact]
    public async Task Delay_Test()
    {
        var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
        await scheduler.Start();

        var job = JobBuilder.Create<PrintJob>()
            .UsingJobData("msg", "3秒后执行")
            .Build();

        await scheduler.ScheduleJob(
            job,
            TriggerBuilder.Create()
                .StartAt(DateTimeOffset.Now.AddSeconds(3))
                .Build());

        await Task.Delay(5000);

        await scheduler.Shutdown();
    }

    /// <summary>
    /// 指定时间执行
    /// </summary>
    [Fact]
    public async Task ScheduleAt_Test()
    {
        var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
        await scheduler.Start();

        var runAt = DateTimeOffset.Now.AddSeconds(5);

        var job = JobBuilder.Create<PrintJob>()
            .UsingJobData("msg", $"计划时间 {runAt:HH:mm:ss}")
            .Build();

        await scheduler.ScheduleJob(
            job,
            TriggerBuilder.Create()
                .StartAt(runAt)
                .Build());

        await Task.Delay(7000);

        await scheduler.Shutdown();
    }

    /// <summary>
    /// 周期执行
    /// </summary>
    [Fact]
    public async Task Repeat_Test()
    {
        var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
        await scheduler.Start();

        var job = JobBuilder.Create<PrintJob>()
            .UsingJobData("msg", "每2秒执行一次")
            .Build();

        await scheduler.ScheduleJob(
            job,
            TriggerBuilder.Create()
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(2)
                    .RepeatForever())
                .Build());

        await Task.Delay(10000);

        await scheduler.Shutdown();
    }

    /// <summary>
    /// Cron表达式
    /// </summary>
    [Fact]
    public async Task Cron_Test()
    {
        var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
        await scheduler.Start();

        var job = JobBuilder.Create<PrintJob>()
            .UsingJobData("msg", "Cron触发")
            .Build();

        await scheduler.ScheduleJob(
            job,
            TriggerBuilder.Create()
                .WithCronSchedule("0/5 * * * * ?")
                .Build());

        await Task.Delay(15000);

        await scheduler.Shutdown();
    }
}