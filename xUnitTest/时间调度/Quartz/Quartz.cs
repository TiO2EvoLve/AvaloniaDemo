using Quartz;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace xUnitTest.时间调度.Quartz;

public class PrintJob(ITestOutputHelper output) : IJob
{
    public ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken)
    {
        output.WriteLine($"当前时间：{DateTime.Now:HH:mm:ss}");
        return ValueTask.CompletedTask;
    }
    [Fact]
    public async Task Run()
    {
        var scheduler = await QuartzSchedulerBuilder
            .Create(q => q.UseInMemoryStore())
            .BuildScheduler();

        var job = JobBuilder
            .Create<PrintJob>()
            .WithIdentity("PrintJob")
            .Build();

        var trigger = TriggerBuilder
            .Create()
            .WithIdentity("PrintTrigger")
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithInterval(TimeSpan.FromSeconds(1))
                .RepeatForever())
            .Build();
        
        await scheduler.ScheduleJob(job, trigger);
        await scheduler.Start();
        await Task.Delay(5000);
        await scheduler.Shutdown();
    }

}