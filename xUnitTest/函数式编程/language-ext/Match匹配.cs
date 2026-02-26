using LanguageExt;
using Xunit.Abstractions;
using static LanguageExt.Prelude;
namespace xUnitTest.函数式编程.language_ext;

public class Match匹配(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void Run()
    {
        Option<int> opt = Some(100);

        var result = opt.Match(
            Some: x => $"值是{x}",
            None: () => "没有值");

        testOutputHelper.WriteLine(result);
    }
}