using LanguageExt;
using Xunit.Abstractions;
using static LanguageExt.Prelude;

namespace xUnitTest.函数式编程.language_ext;

public class Option基础(ITestOutputHelper TS)
{
    [Fact]
    public void Run()
    {
        Option<int> some = Some(10);
        Option<int> none = Option<int>.None;

        TS.WriteLine($"IsSome: {some.IsSome}");
        TS.WriteLine($"IsNone: {none.IsNone}");

        int value1 = some.IfNone(0);
        int value2 = none.IfNone(0);

        TS.WriteLine($"Some.IfNone: {value1}");
        TS.WriteLine($"None.IfNone: {value2}");

        some.IfSome(x => TS.WriteLine($"IfSome执行: {x}"));
    }
}