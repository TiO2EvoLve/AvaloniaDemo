using LanguageExt;
using Xunit.Abstractions;
using static LanguageExt.Prelude;

namespace xUnitTest.函数式编程.language_ext;

/// <summary>演示 Seq 通过 Filter 和 Map 组成不可变数据处理管道。</summary>
public class Seq不可变管道(ITestOutputHelper output)
{
    [Fact]
    public void Run()
    {
        Seq<int> numbers = Seq(1, 2, 3, 4, 5, 6);

        var evenSquares = numbers
            .Filter(number => number % 2 == 0)
            .Map(number => number * number);

        Assert.Equal(new[] { 4, 16, 36 }, evenSquares.ToArray());
        output.WriteLine(string.Join(", ", evenSquares));
    }
}
