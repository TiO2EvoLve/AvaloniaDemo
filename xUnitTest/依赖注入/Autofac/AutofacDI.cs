using Autofac;
using Xunit.Abstractions;

namespace xUnitTest.依赖注入.Autofac;

public class AutofacDI(ITestOutputHelper testOutputHelper)
{


    [Fact]
    public void Run()
    {
        // 1. 创建容器构建器
        var builder = new ContainerBuilder();

        // 2. 注册依赖
        // 注册 ITestOutputHelper（使用实例）
        builder.RegisterInstance(testOutputHelper)
            .As<ITestOutputHelper>();
        
        builder.RegisterType<ConsoleLogger>()
            .As<ILogger>()
            .SingleInstance(); // 单例

        builder.RegisterType<UserRepository>()
            .As<IUserRepository>();

        builder.RegisterType<UserService>();

        // 3. 构建容器
        var container = builder.Build();

        // 4. 解析对象
        var service = container.Resolve<UserService>();
        
        // 5. 执行逻辑
        service.Process("TiO2");
        
    }
    
    #region 业务代码（测试用）

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
    #endregion
}