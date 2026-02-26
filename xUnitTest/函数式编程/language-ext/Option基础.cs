using LanguageExt;
using Xunit.Abstractions;
using static LanguageExt.Prelude;

namespace xUnitTest.函数式编程.language_ext;

public class Option基础(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void Run()
    {
        Option<int> some = Some(10);
        Option<int> none = Option<int>.None;

        testOutputHelper.WriteLine($"IsSome: {some.IsSome}");
        testOutputHelper.WriteLine($"IsNone: {none.IsNone}");

        int value1 = some.IfNone(0);
        int value2 = none.IfNone(0);

        testOutputHelper.WriteLine($"Some.IfNone: {value1}");
        testOutputHelper.WriteLine($"None.IfNone: {value2}");

        some.IfSome(x => testOutputHelper.WriteLine($"IfSome执行: {x}"));
    }
}