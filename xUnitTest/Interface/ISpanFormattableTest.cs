using Xunit.Abstractions;

namespace Test.ConsoleDemo.Interface;

//高性能字符串格式化ISpanFormattable接口测试
public class ISpanFormattableTest(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void Run()
    {
        // 使用示例
        var point = new Point(1.5, 2.5);
        Span<char> buffer = stackalloc char[50];

        if (point.TryFormat(buffer, out var charsWritten, default, null))
            testOutputHelper.WriteLine($"Point: {buffer.Slice(0, charsWritten)}");
    }
}

public readonly struct Point : ISpanFormattable
{
    public double X { get; }
    public double Y { get; }

    public Point(double x, double y)
    {
        (X, Y) = (x, y);
    }

    // 实现ISpanFormattable
    public bool TryFormat(Span<char> destination, out int charsWritten,
        ReadOnlySpan<char> format, IFormatProvider provider)
    {
        // 高性能的格式化实现
        return destination.TryWrite($"({X}, {Y})", out charsWritten);
    }

    // 传统的ToString（回退方法）
    public override string ToString()
    {
        return $"({X}, {Y})";
    }

    // 显式实现IFormattable
    string IFormattable.ToString(string format, IFormatProvider provider)
    {
        return ToString();
    }
}