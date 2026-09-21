using LanguageExt;
using Xunit.Abstractions;
using static LanguageExt.Prelude;
namespace xUnitTest.函数式编程.language_ext;

public class Bind与SelectMany(ITestOutputHelper TS)
{
    [Fact]
    public void Run()
    {
        Option<int> Parse(string s) => int.TryParse(s, out var i) ? Some(i) : None;

        var result =
            from x in Parse("10")
            from y in Parse("10")
            select x + y;

        TS.WriteLine(result.IfNone(0).ToString());
    }
}