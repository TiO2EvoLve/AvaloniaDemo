using Xunit.Abstractions;

namespace xUnitTest.Interface;

//异步释放资源IAsyncDisposable接口测试
public class IAsyncDisposableTest(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public async Task Run()
    {
        // 使用示例 - C# 8.0+
        await using (new DatabaseConnection())
        {
            // 使用连接
            testOutputHelper.WriteLine("使用数据库连接中...");
        } // 自动调用DisposeAsync()
        // 或者手动调用
        var resource = new DatabaseConnection();
        await resource.DisposeAsync();
    }
}
public class DatabaseConnection : IAsyncDisposable, IDisposable
{
    private bool _disposed;

    public async ValueTask DisposeAsync()
    {
        if (!_disposed)
        {
            // 异步清理资源
            await CloseConnectionAsync();
            _disposed = true;
        }

        GC.SuppressFinalize(this);
    }

    // 同步版本
    public void Dispose()
    {
        DisposeAsync().AsTask().Wait();
    }
    
    private async Task CloseConnectionAsync()
    {
        await Task.Delay(100); // 模拟异步操作
        Console.WriteLine("数据库连接已异步关闭");
    }
}