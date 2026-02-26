using System.Reactive.Linq;
using Xunit.Abstractions;

namespace xUnitTest.Rx.Net.过滤操作;

public class FilteringOperators(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void Demo()
    {
        testOutputHelper.WriteLine("\n=== 过滤操作符 ===");

        var numbers = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }.ToObservable();

        // 1. Where (Filter) - 条件过滤
        numbers.Where(x => x % 2 == 0)
            .Subscribe(value => testOutputHelper.WriteLine($"Where (Even): {value}"));

        // 2. Skip - 跳过前N个值
        numbers.Skip(3)
            .Subscribe(value => testOutputHelper.WriteLine($"Skip(3): {value}"));

        // 3. Take - 取前N个值
        numbers.Take(3)
            .Subscribe(value => testOutputHelper.WriteLine($"Take(3): {value}"));

        // 4. Distinct - 去重
        new[] { 1, 2, 2, 3, 3, 3, 1, 4 }.ToObservable()
            .Distinct()
            .Subscribe(value => testOutputHelper.WriteLine($"Distinct: {value}"));

        // 5. DistinctUntilChanged - 忽略连续重复值
        new[] { 1, 1, 2, 2, 3, 1, 1, 4 }.ToObservable()
            .DistinctUntilChanged()
            .Subscribe(value => testOutputHelper.WriteLine($"DistinctUntilChanged: {value}"));

        // 6. SkipWhile/TakeWhile - 条件跳过/获取
        numbers.SkipWhile(x => x < 4)
            .Subscribe(value => testOutputHelper.WriteLine($"SkipWhile(<4): {value}"));

        numbers.TakeWhile(x => x < 6)
            .Subscribe(value => testOutputHelper.WriteLine($"TakeWhile(<6): {value}"));
    }
}