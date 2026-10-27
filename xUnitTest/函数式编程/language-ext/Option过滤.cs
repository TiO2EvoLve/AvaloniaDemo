using LanguageExt;
using Xunit.Abstractions;
using static LanguageExt.Prelude;

namespace xUnitTest.函数式编程.language_ext;

/// <summary>演示 Option 过滤：条件不满足时得到 None。</summary>
public class Option过滤(ITestOutputHelper output)
{
    [Fact]
    public void Run()
    {
        Option<int> number = Some(12);
        var accepted = number.Filter(value => value % 2 == 0);
        var rejected = number.Filter(value => value > 20);

        Assert.Equal(12, accepted.IfNone(0));
        Assert.True(rejected.IsNone);
        output.WriteLine($"通过过滤：{accepted.IfNone(0)}；未通过：{rejected.IsNone}");
    }
}
