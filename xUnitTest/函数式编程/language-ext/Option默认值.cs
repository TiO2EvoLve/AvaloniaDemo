using LanguageExt;
using Xunit.Abstractions;
using static LanguageExt.Prelude;

namespace xUnitTest.函数式编程.language_ext;

/// <summary>演示 Option 缺失时使用 IfNone 提供默认值。</summary>
public class Option默认值(ITestOutputHelper output)
{
    [Fact]
    public void Run()
    {
        Option<string> configuredName = Some("小明");
        Option<string> missingName = None;

        var first = configuredName.IfNone("匿名用户");
        var second = missingName.IfNone("匿名用户");

        Assert.Equal("小明", first);
        Assert.Equal("匿名用户", second);
        output.WriteLine($"配置值：{first}；缺失值：{second}");
    }
}
