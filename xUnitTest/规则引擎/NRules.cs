using NRules;
using NRules.Fluent;
using NRules.Fluent.Dsl;
using Xunit.Abstractions;

namespace xUnitTest.规则引擎;

public class NRules(ITestOutputHelper TS)
{
    [Fact]
    public void Run()
    {
        // 创建规则引擎
        var repository = new RuleRepository();
        repository.Load(x => x.From(typeof(YoungDriverSurchargeRule).Assembly));

        var factory = repository.Compile();
        var session = factory.CreateSession();

        // 创建领域模型实例
        var quote = new InsuranceQuote
        {
            Driver = new Driver { Age = 22 },
            BasePremium = 500
        };

        // 将实例插入规则引擎
        session.Insert(quote);

        // 执行规则
        session.Fire();

        // 输出结果
        TS.WriteLine($"Base Premium: {quote.BasePremium}, Final Premium: {quote.FinalPremium}");
    }
}
// 假设这是你的领域模型
public class InsuranceQuote {
    public Driver Driver { get; set; }
    public decimal BasePremium { get; set; }
    public decimal FinalPremium { get; set; } = 0;
    public void ApplySurcharge(decimal amount) => FinalPremium += amount;
}
public class Driver {
    public int Age { get; set; }
}

// 定义规则：若司机年龄小于25岁，则增加100元附加费
public class YoungDriverSurchargeRule : Rule
{
    public override void Define()
    {
        InsuranceQuote quote = default!;

        When()
            .Match(() => quote, q => q.Driver.Age < 25); // 匹配条件

        Then()
            .Do(ctx => quote.ApplySurcharge(100)); // 执行动作
    }
}