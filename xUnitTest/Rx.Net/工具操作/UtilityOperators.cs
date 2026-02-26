using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Xunit.Abstractions;

namespace xUnitTest.Rx.Net.工具操作;

public class UtilityOperators(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void Demo()
    {
        testOutputHelper.WriteLine("\n=== 工具操作符 ===");

        // 1. Timestamp - 添加时间戳
        Observable.Interval(TimeSpan.FromMilliseconds(500))
            .Take(3)
            .Timestamp()
            .Subscribe(ts =>
                testOutputHelper.WriteLine($"Timestamp: Value={ts.Value}, Time={ts.Timestamp:HH:mm:ss.fff}"));
        //输出：
        //1

        // 2. TimeInterval - 计算时间间隔
        Observable.Interval(TimeSpan.FromMilliseconds(300))
            .Take(3)
            .TimeInterval()
            .Subscribe(ti =>
                testOutputHelper.WriteLine($"TimeInterval: Value={ti.Value}, Interval={ti.Interval.TotalMilliseconds}ms"));

        // 3. Do - 副作用操作（调试用）
        Observable.Range(1, 3)
            .Do(x => testOutputHelper.WriteLine($"Do - Before processing: {x}"))
            .Where(x => x % 2 == 1)
            .Do(x => testOutputHelper.WriteLine($"Do - After filtering: {x}"))
            .Subscribe(value => testOutputHelper.WriteLine($"Do - Final: {value}"));

        // 4. ObserveOn - 指定观察者执行上下文
        testOutputHelper.WriteLine($"Main thread: {Thread.CurrentThread.ManagedThreadId}");

        Observable.Range(1, 3)
            .ObserveOn(NewThreadScheduler.Default) // 在新线程处理
            .Subscribe(value =>
                testOutputHelper.WriteLine($"ObserveOn - Value: {value}, Thread: {Thread.CurrentThread.ManagedThreadId}"));

        // 5. SubscribeOn - 指定订阅执行上下文
        Observable.Create<int>(observer =>
            {
                testOutputHelper.WriteLine($"SubscribeOn - Created on thread: {Thread.CurrentThread.ManagedThreadId}");
                observer.OnNext(1);
                observer.OnNext(2);
                observer.OnCompleted();
                return Disposable.Empty;
            })
            .SubscribeOn(NewThreadScheduler.Default)
            .Subscribe(value =>
                testOutputHelper.WriteLine($"SubscribeOn - Value: {value}, Thread: {Thread.CurrentThread.ManagedThreadId}"));

        Task.Delay(2000).Wait();
    }
}