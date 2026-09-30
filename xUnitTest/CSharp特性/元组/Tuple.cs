using Xunit.Abstractions;

namespace xUnitTest.CSharp特性.元组;

//可以返回多个值
public class Tuple(ITestOutputHelper testOutputHelper)
{
   [Fact]
   public void Run()
   {
      var t1 = (1, "hello");                 // 未命名，字段是 Item1、Item2
      var t2 = (Id: 1, Name: "hello");       // 命名元素
      (int Id, string Name) t3 = (1, "hi");  // 显式类型
      
      int[] data = [1, 2, 3];
      
      var range = GetRange(data);
      testOutputHelper.WriteLine($"{range.Min} - {range.Max}");
   }

   private (int Min, int Max) GetRange(int[] nums)
   {
      return (nums.Min(), nums.Max());
   }
}