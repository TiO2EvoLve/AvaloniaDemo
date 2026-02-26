using System.Reactive.Linq;
using Xunit.Abstractions;

namespace xUnitTest.Rx.Net.错误处理;

public class ErrorHandlingOperators(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void Demo()
    {
        testOutputHelper.WriteLine("\n=== 错误处理操作符 ===");

        // 1. Catch - 捕获错误并继续
        var faultyObservable = Observable.Throw<int>(new Exception("Something went wrong!"));
        var fallbackObservable = Observable.Return(999);

        faultyObservable
            .Catch<int, Exception>(ex =>
            {
                testOutputHelper.WriteLine($"Catch - Error caught: {ex.Message}");
                return fallbackObservable;
            })
            .Subscribe(
                value => testOutputHelper.WriteLine($"Catch - Value: {value}"),
                error => testOutputHelper.WriteLine($"Catch - Should not reach here: {error.Message}")
            );

        // 2. Retry - 重试
        var attempt = 0;
        var retryObservable = Observable.Create<int>(observer =>
        {
            attempt++;
            testOutputHelper.WriteLine($"Retry - Attempt: {attempt}");
            if (attempt < 3)
            {
                observer.OnError(new Exception($"Attempt {attempt} failed"));
            }
            else
            {
                observer.OnNext(42);
                observer.OnCompleted();
            }

            return () => { };
        });

        retryObservable
            .Retry(3) // 最多重试3次
            .Subscribe(
                value => testOutputHelper.WriteLine($"Retry - Success: {value}"),
                error => testOutputHelper.WriteLine($"Retry - Final error: {error.Message}")
            );

        // 3. Finally - 最终清理
        Observable.Range(1, 3)
            .Finally(() => testOutputHelper.WriteLine("Finally - Cleanup completed"))
            .Subscribe(
                value => testOutputHelper.WriteLine($"Finally - Value: {value}"),
                () => testOutputHelper.WriteLine("Finally - Sequence completed")
            );
    }
}