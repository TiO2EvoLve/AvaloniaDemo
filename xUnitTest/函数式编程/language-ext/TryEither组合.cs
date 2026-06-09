using LanguageExt;
using Xunit.Abstractions;
using static LanguageExt.Prelude;
namespace xUnitTest.函数式编程.language_ext;

public class TryEither组合(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void Run()
    {
        Try<int> t = () => { int a = 10;int b = 0; return a / b; };

        var either = t.Match(
            Succ: v => Right<string, int>(v),
            Fail: ex => Left<string, int>(ex.Message)
        );

        testOutputHelper.WriteLine(either.Match(r => $"Right:{r}", l => $"Left:{l}"));

        // 将 Either 转回 Try（将 Left 作为异常抛出）
        Try<int> t2 = () => either.Match(
            Right: r => r,
            Left: l => throw new Exception(l)
        );

        var res = t2.Match(Succ: v => $"Try Succ:{v}", Fail: ex => $"Try Fail:{ex.Message}");
        testOutputHelper.WriteLine(res);
    }
}

