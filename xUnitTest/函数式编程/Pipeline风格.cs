using Xunit.Abstractions;

namespace xUnitTest.函数式编程;

public class Pipeline风格(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void Run()
    {
        var result = 5.Pipe(x => x + 1)
            .Pipe(x => x * 10);
        testOutputHelper.WriteLine(result.ToString()); // 60
    }
}

public static class PipeExtensions
{
    public static TResult Pipe<TSource, TResult>(this TSource value, Func<TSource, TResult> func)
    {
        return func(value);
    }
}