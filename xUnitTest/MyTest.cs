
using Xunit.Abstractions; // xUnit 的特性（[Fact]）
namespace xUnitTest; // 文件作用域命名空间，简洁写法

public class MyTest(ITestOutputHelper TS) // 测试类
{
    [Fact]
    public async Task Test1()
    {
        TS.WriteLine("开始测试！");
        var result = await DelayCallback(() => 100, 1000);
        TS.WriteLine(result.ToString());
        result += 100;
        var result2 = await DelayCallback(() => result, 1000);
        TS.WriteLine(result2.ToString());
    }
    private async Task<T> DelayCallback<T>(Func<T> callback, int delayMilliseconds)
    {
        await Task.Delay(delayMilliseconds);
        return callback();
    }
    
}
