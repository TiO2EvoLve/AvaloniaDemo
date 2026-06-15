using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace xUnitTest.中介者模式;

public class MediatR(ITestOutputHelper TS)
{
    [Fact]
    public async Task Run()
    {
        // 1. 创建容器
        var services = new ServiceCollection();

        services.AddLogging();

        // 让 handler 能用到 TestOutputHelper
        services.AddSingleton(TS);

        // 2. 注册 MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<UserCreatedEvent>();
        });

        var provider = services.BuildServiceProvider();

        var mediator = provider.GetRequiredService<IMediator>();

        // 3. 发布事件（关键点：Publish，不是 Send）
        await mediator.Publish(new UserCreatedEvent("TiO2"));
    }
    
}
//定义事件
public record UserCreatedEvent(string UserName) : INotification;
//监听者1：日志
public class LogHandler(ITestOutputHelper TS) : INotificationHandler<UserCreatedEvent>
{
    public Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
    {
        TS.WriteLine($"[Log] 用户创建：{notification.UserName}");
        return Task.CompletedTask;
    }
}
//监听者2：邮件
public class EmailHandler(ITestOutputHelper TS) : INotificationHandler<UserCreatedEvent>
{
    public Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
    {
        TS.WriteLine($"[Email] 发送欢迎邮件：{notification.UserName}");
        return Task.CompletedTask;
    }
}
//监听者3：积分系统
public class ScoreHandler(ITestOutputHelper TS) : INotificationHandler<UserCreatedEvent>
{

    public Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
    {
        TS.WriteLine($"[Score] 赠送积分：{notification.UserName}");
        return Task.CompletedTask;
    }
}
