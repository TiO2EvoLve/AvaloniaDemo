using System.Reactive.Linq;
using Xunit.Abstractions;

namespace xUnitTest.Rx.Net.创建操作;

public class CreationOperators(ITestOutputHelper TS)
{
    [Fact]
    public void Demo()
    {
        TS.WriteLine("=== 创建操作符 ===");

        // 1. Return - 创建发射单个值的Observable
        Observable.Return("Hello Rx.NET")
            .Subscribe(value => TS.WriteLine($"Return: {value}"));

        // 2. Range - 创建发射整数范围的Observable
        Observable.Range(1, 5)
            .Subscribe(value => TS.WriteLine($"Range: {value}"));

        // 3. Interval - 按固定时间间隔发射递增数字
        var intervalSubscription = Observable.Interval(TimeSpan.FromSeconds(1))
            .Take(3) // 只取前3个值
            .Subscribe(value => TS.WriteLine($"Interval: {value}"));

        // 等待Interval完成
        Task.Delay(3500).Wait();

        // 4. Timer - 在指定延迟后发射值
        Observable.Timer(TimeSpan.FromSeconds(1))
            .Subscribe(value => TS.WriteLine($"Timer: {value}"));

        // 5. Empty - 创建立即完成的空Observable
        Observable.Empty<int>()
            .Subscribe(
                value => TS.WriteLine($"Empty - OnNext: {value}"),
                () => TS.WriteLine("Empty - Completed")
            );

        // 6. Never - 创建永不发射值也永不完成的Observable
        var neverSubscription = Observable.Never<string>()
            .Subscribe(
                value => TS.WriteLine($"Never - OnNext: {value}"),
                () => TS.WriteLine("Never - Completed")
            );

        // 7. Throw - 创建立即发射错误的Observable
        Observable.Throw<int>(new Exception("Test error"))
            .Subscribe(
                value => TS.WriteLine($"Throw - OnNext: {value}"),
                ex => TS.WriteLine($"Throw - Error: {ex.Message}"),
                () => TS.WriteLine("Throw - Completed")
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