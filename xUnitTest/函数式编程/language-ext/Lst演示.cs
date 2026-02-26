using Xunit.Abstractions;
using static LanguageExt.Prelude;
namespace xUnitTest.函数式编程.language_ext;
// 演示不可变列表
public class Lst演示(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void Run()
    {
        var list = List(1, 2, 3);

        var newList = list.Add(4);

        testOutputHelper.WriteLine($"原始长度: {list.Count}");
        testOutputHelper.WriteLine($"新列表长度: {newList.Count}");
    }
}