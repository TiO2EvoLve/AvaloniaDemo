using Xunit.Abstractions;
using static LanguageExt.Prelude;
namespace xUnitTest.函数式编程.language_ext;

public class Where过滤(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void Run()
    {
        var opt = Some(10);

        var filtered = opt.Where(x => x > 5);
        var filtered2 = opt.Where(x => x > 20);

        testOutputHelper.WriteLine(filtered.IsSome.ToString());  // True
        testOutputHelper.WriteLine(filtered2.IsNone.ToString()); // True
    }
}