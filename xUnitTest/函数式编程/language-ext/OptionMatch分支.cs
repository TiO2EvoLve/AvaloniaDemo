using LanguageExt;
using Xunit.Abstractions;
using static LanguageExt.Prelude;

namespace xUnitTest.函数式编程.language_ext;

/// <summary>演示 Option 使用 Match 对 Some 和 None 分别处理。</summary>
public class OptionMatch分支(ITestOutputHelper output)
{
    [Fact]
    public void Run()
    {
        Option<string> present = Some("LanguageExt");
        Option<string> missing = None;

        var presentMessage = present.Match(
            Some: value => $"找到值：{value}",
            None: () => "没有值");
        var missingMessage = missing.Match(
            Some: value => $"找到值：{value}",
            None: () => "没有值");

        Assert.Equal("找到值：LanguageExt", presentMessage);
        Assert.Equal("没有值", missingMessage);
        output.WriteLine(presentMessage);
        output.WriteLine(missingMessage);
    }
}
