using Xunit.Abstractions;
using static LanguageExt.Prelude;
namespace xUnitTest.函数式编程.language_ext;

public class Seq演示(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void Run()
    {
        var seq = Seq(1, 2, 3, 4);

        var result = seq
            .Where(x => x % 2 == 0)
            .Map(x => x * 10);

        foreach (var item in result)
        {
            testOutputHelper.WriteLine(item.ToString());
        }
    }
}