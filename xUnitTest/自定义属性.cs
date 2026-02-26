using System.Reflection;
using Xunit.Abstractions;

namespace xUnitTest;

public class 自定义属性(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void Run()
    {
        //通过反射读取并展示效果
        var type = typeof(MyClass);

        // === 读取类上的属性 ===
        var classAttr = type.GetCustomAttribute<AuthorAttribute>();
        testOutputHelper.WriteLine($"类作者: {classAttr.Name}, Version: {classAttr.Version}");

        // === 读取方法上的属性 ===
        var method = type.GetMethod("DoWork");
        var methodAttr = method.GetCustomAttribute<AuthorAttribute>();
        testOutputHelper.WriteLine($"方法作者: {methodAttr.Name}, Version: {methodAttr.Version}");
    }
}

//定义自定义属性
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorAttribute : Attribute
{
    public AuthorAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; }
    public string Version { get; set; }
}

//使用自定义属性
[Author("TiO2 EvoLve", Version = "1.0")]
public class MyClass
{
    [Author("System", Version = "2.3")]
    public void DoWork()
    {
    }
}