using System.Reactive.Linq;
using Xunit.Abstractions;

namespace xUnitTest.Rx.Net.创建操作;

public class CreationOperators(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void Demo()
    {
        testOutputHelper.WriteLine("=== 创建操作符 ===");

        // 1. Return - 创建发射单个值的Observable
        Observable.Return("Hello Rx.NET")
            .Subscribe(value => testOutputHelper.WriteLine($"Return: {value}"));

        // 2. Range - 创建发射整数范围的Observable
        Observable.Range(1, 5)
            .Subscribe(value => testOutputHelper.WriteLine($"Range: {value}"));

        // 3. Interval - 按固定时间间隔发射递增数字
        var intervalSubscription = Observable.Interval(TimeSpan.FromSeconds(1))
            .Take(3) // 只取前3个值
            .Subscribe(value => testOutputHelper.WriteLine($"Interval: {value}"));

        // 等待Interval完成
        Task.Delay(3500).Wait();

        // 4. Timer - 在指定延迟后发射值
        Observable.Timer(TimeSpan.FromSeconds(1))
            .Subscribe(value => testOutputHelper.WriteLine($"Timer: {value}"));

        // 5. Empty - 创建立即完成的空Observable
        Observable.Empty<int>()
            .Subscribe(
                value => testOutputHelper.WriteLine($"Empty - OnNext: {value}"),
                () => testOutputHelper.WriteLine("Empty - Completed")
            );

        // 6. Never - 创建永不发射值也永不完成的Observable
        var neverSubscription = Observable.Never<string>()
            .Subscribe(
                value => testOutputHelper.WriteLine($"Never - OnNext: {value}"),
                () => testOutputHelper.WriteLine("Never - Completed")
            );

        // 7. Throw - 创建立即发射错误的Observable
        Observable.Throw<int>(new Exception("Test error"))
            .Subscribe(
                value => testOutputHelper.WriteLine($"Throw - OnNext: {value}"),
                ex => testOutputHelper.WriteLine($"Throw - Error: {ex.Message}"),
                () => testOutputHelper.WriteLine("Throw - Completed")
            );

        // 8. Create - 自定义创建Observable
        var observable = Observable.Create<int>(observer =>
        {
            observer.OnNext(1);
            observer.OnNext(2);
            observer.OnNext(3);
            observer.OnCompleted();
            return () => Console.WriteLine("Create - Subscription disposed");
        });

        var createSubscription = observable.Subscribe(value =>
            Console.WriteLine($"Create: {value}"));

        Task.Delay(1000).Wait();
        createSubscription.Dispose();
        neverSubscription.Dispose();
        intervalSubscription.Dispose();
    }
}