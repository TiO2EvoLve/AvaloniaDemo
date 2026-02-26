using Xunit.Abstractions;

namespace xUnitTest.函数式编程;

public class 高阶函数(ITestOutputHelper testOutputHelper)
{
    // 函数作为参数
    private static int Calculate(int x, int y, Func<int, int, int> operation)
    {
        return operation(x, y);
    }

    // 函数作为返回值
    private static Func<int, int> Multiplier(int factor)
    {
        return x => x * factor;
    }

    // 使用示例
    [Fact]
    public void Run()
    {
        var result = Calculate(5, 3, (a, b) => a + b);
        var doubleIt = Multiplier(2);
        testOutputHelper.WriteLine(doubleIt(5).ToString()); // 输出 10
    }
}