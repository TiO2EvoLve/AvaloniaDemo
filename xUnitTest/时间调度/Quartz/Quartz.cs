using Quartz;
using Quartz.Impl;

namespace xUnitTest.时间调度.Quartz;

public class PrintJob : IJob
{
    public ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }
}