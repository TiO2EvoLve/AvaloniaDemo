using System.ComponentModel.DataAnnotations;
using Xunit.Abstractions;

namespace xUnitTest.Interface;

//数据验证IValidatableObject接口测试
public class IValidatableObjectTest(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void Run()
    {
        // 使用示例
        var order = new Order
        {
            OrderDate = DateTime.Now.AddDays(1), // 无效的未来日期
            TotalAmount = 15000 // 大额订单
        };

        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(order, new ValidationContext(order), results, true);

        if (!isValid)
        {
            testOutputHelper.WriteLine("验证错误:");
            foreach (var error in results) 
                testOutputHelper.WriteLine($" - {error.ErrorMessage}");
        }
    }
}

public class Order : IValidatableObject
{
    public DateTime OrderDate { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public decimal TotalAmount { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // 自定义验证逻辑
        if (OrderDate > DateTime.Now)
            yield return new ValidationResult("订单日期不能是未来时间", new[] { nameof(OrderDate) });

        if (DeliveryDate.HasValue && DeliveryDate < OrderDate)
            yield return new ValidationResult("交付日期不能早于订单日期", new[] { nameof(DeliveryDate) });

        if (TotalAmount <= 0)
            yield return new ValidationResult("订单金额必须大于0", new[] { nameof(TotalAmount) });

        // 复杂业务规则
        if (TotalAmount > 10000 && !DeliveryDate.HasValue)
            yield return new ValidationResult("大额订单必须指定交付日期", new[] { nameof(DeliveryDate) });
    }
}