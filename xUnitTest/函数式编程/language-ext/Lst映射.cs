using LanguageExt;
using Xunit.Abstractions;
using static LanguageExt.Prelude;

namespace xUnitTest.函数式编程.language_ext;

/// <summary>演示对不可变 Lst 映射，生成转换后的新列表。</summary>
public class Lst映射(ITestOutputHelper output)
{
    [Fact]
    public void Run()
    {
        Lst<int> original = List(1, 2, 3);
        var doubled = original.Map(value => value * 2);

        Assert.Equal(new[] { 2, 4, 6 }, doubled.ToArray());
        Assert.Equal(new[] { 1, 2, 3 }, original.ToArray());
        output.WriteLine($"原列表：{string.Join(", ", original)}");
        output.WriteLine($"映射结果：{string.Join(", ", doubled)}");
    }
}
