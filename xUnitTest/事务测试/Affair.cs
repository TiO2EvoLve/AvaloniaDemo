using System.Transactions;
using Xunit.Abstractions;

namespace xUnitTest.事务测试;

public class Affair(ITestOutputHelper testOutputHelper)
{
    [Fact]
    private void Run()
    {
        try
        {
            // 创建一个TransactionScope
            using (var scope = new TransactionScope())
            {
                // 执行一些操作
                PerformOperation1();
                PerformOperation2();

                // 如果所有操作都成功，则提交事务
                scope.Complete();
            }
            testOutputHelper.WriteLine("事务成功提交！");
        }
        catch (Exception ex)
        {
            // 如果出现异常，事务会自动回滚
            testOutputHelper.WriteLine("事务回滚，原因: " + ex.Message);
        }
    }
    private static void PerformOperation1()
    {
        // 模拟一个操作
        Console.WriteLine("执行操作1...");
        // 如果这里抛出异常，事务会回滚
        //throw new Exception("操作1失败");
    }

    private static void PerformOperation2()
    {
        // 模拟另一个操作
        Console.WriteLine("执行操作2...");
        // 如果这里抛出异常，事务会回滚
        throw new Exception("操作2失败");
    }

}