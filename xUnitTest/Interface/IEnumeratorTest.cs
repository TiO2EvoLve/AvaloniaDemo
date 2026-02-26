using System.Collections;
using Xunit.Abstractions;
using System;
using System.Collections;
namespace xUnitTest.Interface;

//可枚举IEnumerator接口测试
public class IEnumeratorTest(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void Run()
    {
        testOutputHelper.WriteLine("=== DaysOfWeek (手动实现IEnumerator) ===");
        var days = new DaysOfWeek();
        foreach (string day in days) Console.WriteLine($"今天是：{day}");

        testOutputHelper.WriteLine("\n=== NumberSequence (使用yield return) ===");
        var numbers = new NumberSequence(10, 5);
        foreach (var num in numbers) Console.WriteLine($"数字：{num}");

        testOutputHelper.WriteLine("\n=== SmartCollection (泛型集合) ===");
        var smartCollection = new SmartCollection<string>("Apple", "Banana", "Cherry");
        smartCollection.Add("Date");

        foreach (var fruit in smartCollection) Console.WriteLine($"水果：{fruit}");

        testOutputHelper.WriteLine("\n=== 与LINQ结合使用 ===");
        var evenNumbers = new NumberSequence(1, 10)
            .Where(n => n % 2 == 0)
            .Select(n => n * 2);

        foreach (var num in evenNumbers) Console.WriteLine($"偶数乘以2：{num}");

        testOutputHelper.WriteLine("\n=== 直接使用枚举器 ===");
        DemonstrateEnumerator();
    }

    private static void DemonstrateEnumerator()
    {
        var collection = new SmartCollection<int>(1, 2, 3, 4, 5);
        var enumerator = collection.GetEnumerator();

        try
        {
            while (enumerator.MoveNext()) Console.WriteLine($"当前元素：{enumerator.Current}");

            // 尝试在结束后访问Current会抛出异常
            // Console.WriteLine(enumerator.Current); // 这会抛出InvalidOperationException
        }
        finally
        {
            enumerator.Dispose(); // 重要：释放资源
        }
    }
}

// 示例1：手动实现IEnumerable（非泛型）
public class DaysOfWeek : IEnumerable
{
    private readonly string[] days = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

    // 实现GetEnumerator方法
    public IEnumerator GetEnumerator()
    {
        return new DaysEnumerator(days);
    }

    // 自定义枚举器
    private class DaysEnumerator : IEnumerator
    {
        private readonly string[] days;
        private int position = -1;

        public DaysEnumerator(string[] daysArray)
        {
            days = daysArray;
        }

        public object Current
        {
            get
            {
                if (position < 0 || position >= days.Length)
                    throw new InvalidOperationException();
                return days[position];
            }
        }

        public bool MoveNext()
        {
            position++;
            return position < days.Length;
        }

        public void Reset()
        {
            position = -1;
        }
    }
}

// 示例2：使用yield return简化实现（泛型）
public class NumberSequence : IEnumerable<int>
{
    private readonly int count;
    private readonly int start;

    public NumberSequence(int start, int count)
    {
        this.start = start;
        this.count = count;
    }

    public IEnumerator<int> GetEnumerator()
    {
        for (var i = 0; i < count; i++) yield return start + i;
    }

    // 显式实现非泛型接口
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

// 示例3：更复杂的自定义集合
public class SmartCollection<T> : IEnumerable<T>
{
    private T[] items;

    public SmartCollection(params T[] initialItems)
    {
        items = initialItems ?? new T[0];
    }

    public IEnumerator<T> GetEnumerator()
    {
        // 使用yield return的简单实现
        foreach (var item in items) yield return item;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(T item)
    {
        Array.Resize(ref items, items.Length + 1);
        items[items.Length - 1] = item;
    }
}