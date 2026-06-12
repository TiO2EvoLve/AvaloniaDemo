using System;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;

namespace AvaloniaTestDemo.Services;

public sealed class QuartzService : IDisposable
{
    public IScheduler Scheduler { get; }

    public QuartzService()
    {
        var factory = new StdSchedulerFactory();
        Scheduler = factory.GetScheduler().GetAwaiter().GetResult();
        Scheduler.Start().GetAwaiter().GetResult();
    }

    public void Dispose()
    {
        if (Scheduler.IsStarted)
        {
            Scheduler.Shutdown(waitForJobsToComplete: true).GetAwaiter().GetResult();
        }
    }
}
