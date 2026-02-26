using LanguageExt;
using Xunit.Abstractions;
using static LanguageExt.Prelude;
namespace xUnitTest.函数式编程.language_ext;
//用于处理可能有两种不同结果的核心类型，通常表示成功（Right）或失败（Left）。它是替代异常处理和复杂条件分支的理想选择。
public class Either演示(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void Run()
    {
        Either<string, int> Divide(int x, int y) =>
            y == 0 ? Left("除数不能为0") : Right(x / y);

        var ok = Divide(10, 2);
        var fail = Divide(10, 0);

        testOutputHelper.WriteLine(ok.Match(
            Right: r => $"结果:{r}",
            Left: l => l));

        testOutputHelper.WriteLine(fail.Match(
            Right: r => $"结果:{r}",
            Left: l => l));
    }
}
