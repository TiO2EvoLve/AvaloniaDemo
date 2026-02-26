using LanguageExt;
using Xunit.Abstractions;
using static LanguageExt.Prelude;
namespace xUnitTest.函数式编程.language_ext;

public class State演示(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void Run()
    {
        var state  = State<int, int> ( s => (s * 2, s + 1));
        var (value, newState) = state.Run(10);
        testOutputHelper.WriteLine($"值:{value}");
        testOutputHelper.WriteLine($"新状态:{newState}");
    }
}