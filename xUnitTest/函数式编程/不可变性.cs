using Xunit.Abstractions;

namespace xUnitTest.函数式编程;

public class 不可变性(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void Run()
    {
        var p1 = new Person("Jack", 20);
        // 使用 with 创建修改后的拷贝，不改变原对象
        var p2 = p1 with { Age = 21 };

        testOutputHelper.WriteLine(p1.ToString()); // Person { Name = Jack, Age = 20 }
        testOutputHelper.WriteLine(p2.ToString()); // Person { Name = Jack, Age = 21 }
    }

    // 使用 record 类型（C# 9+）
    private record Person(string Name, int Age);
}