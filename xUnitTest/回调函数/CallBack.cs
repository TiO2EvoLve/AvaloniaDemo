using Xunit.Abstractions;

namespace xUnitTest.回调函数;

public class CallBack(ITestOutputHelper TS)
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