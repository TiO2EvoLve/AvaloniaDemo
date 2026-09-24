using Xunit.Abstractions;
using ZiggyCreatures.Caching.Fusion;
namespace xUnitTest.缓存;

public class FusionCacheTest
{
    private readonly ITestOutputHelper TS;

    public FusionCacheTest(ITestOutputHelper testOutputHelper)
    {
        TS = testOutputHelper;
    }
    

    [Fact]
    private async Task Run()
    {
        var cache = new FusionCache(new FusionCacheOptions());

        TS.WriteLine("第一次查询");
        await GetUserName(1);
        
        TS.WriteLine("第二次查询");
        await GetUserName(1);

        TS.WriteLine("等待缓存过期...");
        await Task.Delay(TimeSpan.FromSeconds(6));
        
        TS.WriteLine("第三次查询");
        await GetUserName(1);

        return;

        async Task GetUserName(int id)
        {
            var start = DateTime.Now;

            //从缓存获取没有则Set
            var userName = await cache.GetOrSetAsync(
                $"User_{id}",
                async _ =>
                {
                    TS.WriteLine("正在查询数据库...");

                    await Task.Delay(3000);

                    return $"用户{id}";
                },
                //过期时间
                TimeSpan.FromSeconds(5));

            TS.WriteLine($"结果: {userName}");

            TS.WriteLine($"耗时: {(DateTime.Now - start).TotalMilliseconds:F0} ms");
        }
    }

    [Fact]
    public async Task Run1()
    {
        var cache = new FusionCache(new FusionCacheOptions());

        var tasks = Enumerable
            .Range(1, 10)
            .Select(_ => cache.GetOrSetAsync(
                "Data",
                async _ =>
                {
                    TS.WriteLine(
                        $"数据库查询 Thread:{Environment.CurrentManagedThreadId}");

                    await Task.Delay(3000);

                    return DateTime.Now.ToString("HH:mm:ss");
                },
                TimeSpan.FromMinutes(1)));

        await Task.WhenAll(tasks.Select(vt => vt.AsTask()));
    }
}