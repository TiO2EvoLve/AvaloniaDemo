using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace xUnitTest.依赖注入.Extensions;

public class DependencyInjection(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void Run()
    {
        // 1. 创建服务集合
        var services = new ServiceCollection();

        // 2. 注册依赖

        // 注册实例
        services.AddSingleton(testOutputHelper);

        // 单例
        services.AddSingleton<ILogger, ConsoleLogger>();

        // 瞬态（默认每次创建新对象）
        services.AddTransient<IUserRepository, UserRepository>();

        // 注册自身
        services.AddTransient<UserService>();

        // 3. 构建服务提供器
        var provider = services.BuildServiceProvider();

        // 4. 解析对象
        var service = provider.GetRequiredService<UserService>();

        // 5. 执行逻辑
        service.Process("TiO2");
    }

    public interface ILogger
    {
        void Log(string message);
    }

    public class ConsoleLogger(ITestOutputHelper testOutputHelper) : ILogger
    {
        public void Log(string message)
        {
            testOutputHelper.WriteLine($"[Log] {message}");
        }
    }

    public interface IUserRepository
    {
        string GetUser(string name);
    }

    public class UserRepository(ILogger logger) : IUserRepository
    {
        public string GetUser(string name)
        {
            logger.Log($"查询用户: {name}");
            return $"User:{name}";
        }
    }

    public class UserService(IUserRepository repo, ILogger logger)
    {
        public void Process(string name)
        {
            logger.Log("开始处理业务");

            var user = repo.GetUser(name);

            logger.Log($"处理完成: {user}");
        }
    }
}