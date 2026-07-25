namespace xUnitTest.Fsharp;
using Xunit.Abstractions;
using MyFunctionalLib;

public class FsharpTest(ITestOutputHelper TS)
{
    [Fact]
    public void Run()
    {
        var result =
            Math.factorial(0);
        TS.WriteLine(result.ToString());
    }
}