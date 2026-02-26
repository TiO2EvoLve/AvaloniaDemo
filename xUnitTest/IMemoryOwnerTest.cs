using System.Buffers;
using Xunit.Abstractions;

namespace xUnitTest;

//内存管理IMemoryOwner接口测试
public class IMemoryOwnerTest(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void Run()
    {
        Task.Run(async () => await ProcessDataAsync()).GetAwaiter().GetResult();
    }

    private async Task ProcessDataAsync()
    {
        using var buffer = MemoryPool<byte>.Shared.Rent(1024);
        // 使用内存缓冲区
        var memory = buffer.Memory;

        // 模拟异步数据处理
        await ProcessMemoryAsync(memory.Slice(0, 100));

        testOutputHelper.WriteLine("缓冲区处理完成");
    }

    private static async Task ProcessMemoryAsync(Memory<byte> memory)
    {
        // 处理内存数据
        await Task.Delay(100);
    }
}