using LanguageExt;
using Xunit.Abstractions;
using static LanguageExt.Prelude;

namespace xUnitTest.函数式编程.language_ext;

/// <summary>演示 Either 分别映射成功值与错误值，保留原有分支。</summary>
public class Either映射(ITestOutputHelper output)
{
    [Fact]
    public void Run()
    {
        Either<string, int> success = Right<string, int>(21);
        Either<string, int> failure = Left<string, int>("输入无效");

        var doubled = success.Map(value => value * 2);
        var contextualFailure = failure.MapLeft(error => $"解析失败：{error}");

        Assert.Equal(42, doubled.Match(Right: value => value, Left: _ => 0));
        Assert.Equal("解析失败：输入无效", contextualFailure.Match(Right: _ => "", Left: error => error));
        output.WriteLine(doubled.Match(Right: value => $"成功：{value}", Left: error => error));
        output.WriteLine(contextualFailure.Match(Right: value => value.ToString(), Left: error => error));
    }
}
