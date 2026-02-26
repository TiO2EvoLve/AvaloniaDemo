using LanguageExt;
using Xunit.Abstractions;
using static LanguageExt.Prelude;
namespace xUnitTest.函数式编程.language_ext;

public class Try演示(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void Run()
    {
        
        Try<int> tryFunc = () => 10 / 2;

        var result = tryFunc.Match(
            Succ: x => $"成功:{x}",
            Fail: ex => $"异常:{ex.Message}");

        testOutputHelper.WriteLine(result);
    }
}