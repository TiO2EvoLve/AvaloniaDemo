using LanguageExt;
using Xunit.Abstractions;
using static LanguageExt.Prelude;
namespace xUnitTest.函数式编程.language_ext;

public class Bind与SelectMany(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void Run()
    {
        Option<int> Parse(string s) =>
            int.TryParse(s, out var i) ? Some(i) : None;

        var result =
            from x in Parse("10")
            from y in Parse("20")
            select x + y;

        testOutputHelper.WriteLine(result.IfNone(0).ToString()); // 30
    }
}