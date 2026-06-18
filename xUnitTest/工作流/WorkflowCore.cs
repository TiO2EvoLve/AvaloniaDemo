using Microsoft.Extensions.DependencyInjection;
using WorkflowCore.Interface;
using WorkflowCore.Models;
using Xunit.Abstractions;

namespace xUnitTest.工作流;

public class WorkflowCoreDemo(ITestOutputHelper TS)
{
    
    [Fact]
    public async Task Run()
    {
        var services = new ServiceCollection();

        services.AddLogging();
        services.AddSingleton(TS);
        services.AddWorkflow();

        var provider = services.BuildServiceProvider();

        var host = provider.GetRequiredService<IWorkflowHost>();

        host.RegisterWorkflow<HelloWorldWorkflow>();

        host.Start();

        await host.StartWorkflow("HelloWorld");

        host.Stop();

        TS.WriteLine("工作流执行成功！");
    }
}

public class 步骤1() : StepBody
{
    public override ExecutionResult Run(IStepExecutionContext context)
    {
        Console.WriteLine("步骤一执行");
        return ExecutionResult.Next();
    }
}

public class 步骤2() : StepBody
{
    public override ExecutionResult Run(IStepExecutionContext context)
    {
        Console.WriteLine("步骤二执行");
        return ExecutionResult.Next();
    }
}

public class HelloWorldWorkflow : IWorkflow
{
    public string Id => "HelloWorld";
    public int Version => 1;

    public void Build(IWorkflowBuilder<object> builder)
    {
        builder
            .StartWith<步骤1>()
            .Then<步骤2>();
    }
}