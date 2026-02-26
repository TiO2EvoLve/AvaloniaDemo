using LanguageExt;
using Xunit.Abstractions;
using static LanguageExt.Prelude;
namespace xUnitTest.函数式编程.language_ext;

public class Reader演示(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void Run()
    {
        var reader =
            Reader<string, string>(env => $"当前环境:{env}");

        var result = reader.Run("开发环境");

        testOutputHelper.WriteLine(result.ToString());
    }
}