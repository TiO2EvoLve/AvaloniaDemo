using Xunit.Abstractions;
using static LanguageExt.Prelude;
namespace xUnitTest.函数式编程.language_ext;

public class Map字典演示(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void Run()
    {
        var map = Map<string, int>()
            .Add("A", 1)
            .Add("B", 2);

        var value = map.Find("A").IfNone(0);

        testOutputHelper.WriteLine($"A={value}");
    }
}