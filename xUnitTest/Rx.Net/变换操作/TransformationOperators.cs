using System.Reactive.Linq;
using Xunit.Abstractions;

namespace xUnitTest.Rx.Net.变换操作;

public class TransformationOperators(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void Demo()
    {
        testOutputHelper.WriteLine("\n=== 变换操作符 ===");

        // 1. Select (Map) - 投影转换
        Observable.Range(1, 5)
            .Select(x => x * x)
            .Subscribe(value => testOutputHelper.WriteLine($"Select: {value}"));

        // 2. SelectMany - 扁平化转换
        Observable.Range(1, 3)
            .SelectMany(x => Observable.Range(1, x))
            .Subscribe(value => testOutputHelper.WriteLine($"SelectMany: {value}"));

        // 3. Buffer - 缓冲多个值
        Observable.Interval(TimeSpan.FromMilliseconds(500))
            .Take(10)
            .Buffer(3) // 每3个值缓冲一次
            .Subscribe(buffer =>
                testOutputHelper.WriteLine($"Buffer: [{string.Join(", ", buffer)}]"));

        // 4. Window - 窗口化（类似Buffer但返回Observable）
        Observable.Interval(TimeSpan.FromMilliseconds(400))
            .Take(8)
            .Window(3)
            .Subscribe(window =>
            {
                window.Subscribe(value => testOutputHelper.WriteLine($"Window:{value} "));
            });

        // 5. GroupBy - 分组
        var words = new[] { "apple", "banana", "cat", "dog", "elephant" };
        words.ToObservable()
            .GroupBy(word => word.Length)
            .Subscribe(group =>
            {
                testOutputHelper.WriteLine($"GroupBy (Length {group.Key}): ");
                group.Subscribe(word => testOutputHelper.WriteLine($"{word} "));
            });

        Task.Delay(3000).Wait();
    }
}