using LanguageExt;
using Xunit.Abstractions;
using static LanguageExt.Prelude;

namespace xUnitTest.函数式编程.language_ext;

/// <summary>演示使用 Fold 将 Seq 汇总为单个结果。</summary>
public class Seq折叠(ITestOutputHelper output)
{
    [Fact]
    public void Run()
    {
        Seq<int> values = Seq(2, 4, 6, 8);
        var total = values.Fold(0, (sum, value) => sum + value);

        Assert.Equal(20, total);
        output.WriteLine($"总和：{total}");
    }
}
