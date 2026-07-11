namespace xUnitTest.Fsharp;
using Xunit.Abstractions;
using MyFunctionalLib;

public class FsharpTest(ITestOutputHelper TS)
{
    [Fact]
    public void Run()
    {
        var result =
            Math.square(10);
        TS.WriteLine(result.ToString());
    }
}