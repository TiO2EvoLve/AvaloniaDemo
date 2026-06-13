using DryIoc;
using Xunit.Abstractions;

namespace xUnitTest.依赖注入.DryIoc;

public class DryIoc(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void Run()
    {
        // 1. 创建容器
        var container = new Container();

        // 2. 注册依赖

        // 实例注册
        container.RegisterInstance(testOutputHelper);

        // 单例 ILogger
        container.Register<ILogger, ConsoleLogger>(Reuse.Singleton);

        // 普通注册
        container.Register<IUserRepository, UserRepository>();

        container.Register<UserService>();

        // 3. 解析对象
        var service = container.Resolve<UserService>();

        // 4. 执行逻辑
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