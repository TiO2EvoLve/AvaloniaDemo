using LanguageExt;
using Xunit.Abstractions;
using static LanguageExt.Prelude;

namespace xUnitTest.函数式编程.language_ext;

/// <summary>演示 Either 通过 LINQ 查询语法组合可能失败的步骤。</summary>
public class Either查询组合(ITestOutputHelper output)
{
    [Fact]
    public void Run()
    {
        Either<string, int> Parse(string text) =>
            int.TryParse(text, out var value) ? Right<string, int>(value) : Left<string, int>("不是有效整数");

        Either<string, int> Positive(int value) =>
            value > 0 ? Right<string, int>(value) : Left<string, int>("数值必须大于零");

        var result =
            from first in Parse("20")
            from second in Parse("22")
            from positive in Positive(first + second)
            select positive;

        Assert.Equal(42, result.Match(Right: value => value, Left: _ => 0));
        output.WriteLine(result.Match(Right: value => $"成功：{value}", Left: error => $"失败：{error}"));
    }
}
