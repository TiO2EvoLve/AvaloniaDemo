using Xunit.Abstractions;

namespace xUnitTest.函数式编程;

public class 函数组合(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void Run()
    {
        int Add2(int x) => x + 2;
        int Mul10(int x) => x * 10;
        int Composed(int x) => Mul10(Add2(x));

        testOutputHelper.WriteLine(Composed(3).ToString()); // (3+2)*10 = 50
    }
}