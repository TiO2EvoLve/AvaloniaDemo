using Elsa.Extensions;
using Elsa.Workflows;
using Elsa.Workflows.Activities;
using Microsoft.Extensions.DependencyInjection;

namespace xUnitTest.工作流;

public class Elsa
{
    [Fact]
    public async Task Run()
    {
        var services = new ServiceCollection();
        
        services.AddElsa();

        var serviceProvider = services.BuildServiceProvider();


        var workflow = new Sequence
        {
            Activities =
            {
                new WriteLine("Hello World!"),
                new WriteLine("We can do more than a one-liner!")
            }
        };
        

        var workflowRunner = serviceProvider.GetRequiredService<IWorkflowRunner>();
        
        await workflowRunner.RunAsync(workflow);
    }
}