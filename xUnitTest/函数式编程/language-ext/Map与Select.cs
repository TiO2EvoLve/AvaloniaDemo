using LanguageExt;
using Xunit.Abstractions;
using static LanguageExt.Prelude;
namespace xUnitTest.函数式编程.language_ext;

//Map与Select本质上是同一个操作，只是命名风格不同
public class Map与Select(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void Run()
    {
        Option<int> opt = Some(5);

        var mapped = opt.Map(x => x * 2);
        var selected = opt.Select(x => x + 3);

        testOutputHelper.WriteLine(mapped.IfNone(0).ToString());   // 10
        testOutputHelper.WriteLine(selected.IfNone(0).ToString()); // 8
    }
}