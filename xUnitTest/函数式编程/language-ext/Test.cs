using LanguageExt;
using Xunit.Abstractions;
namespace xUnitTest.函数式编程.language_ext;
using static LanguageExt.Prelude;


public class Test(ITestOutputHelper TS)
{
    [Fact]
    private void Run()
    {
        var opt = Seq(1,2,3,4,5,6);
        var x = opt.Where(x => x > 4);
        
        foreach (var s in x)
        {
            TS.WriteLine(s.ToString());
        }
    }
}