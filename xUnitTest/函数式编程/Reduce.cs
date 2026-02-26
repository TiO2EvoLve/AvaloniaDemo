using Xunit.Abstractions;

namespace xUnitTest.函数式编程;

public class Reduce(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void Run()
    {
        var sum = new[] { 1, 2, 3, 4 }.Aggregate((a, b) => a + b);
        testOutputHelper.WriteLine(sum.ToString()); // 10
    }
}