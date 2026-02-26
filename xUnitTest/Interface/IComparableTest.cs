using Xunit.Abstractions;

namespace xUnitTest.Interface;

//自定义排序IComparable接口测试
public class IComparableTest(ITestOutputHelper testOutputHelper)
{
    //使用示例
    [Fact]
    public void Run()
    {
        // 使用示例
        var products = new List<Product>
        {
            new() { Name = "Laptop", Price = 999.99m },
            new() { Name = "Mouse", Price = 29.99m },
            new() { Name = "Keyboard", Price = 79.99m }
        };

        // 使用自然排序（按价格）
        products.Sort();
        testOutputHelper.WriteLine("按价格排序:");
        products.ForEach(Console.WriteLine);

        // 使用自定义比较器（按名称）
        products.Sort(new ProductNameComparer());
        testOutputHelper.WriteLine("\n按名称排序:");
        products.ForEach(Console.WriteLine);
    }
}

public class Product : IComparable<Product>
{
    public string Name { get; set; }
    public decimal Price { get; set; }

    // 实现自然排序（按价格）
    public int CompareTo(Product other)
    {
        if (other is null) return 1;
        return Price.CompareTo(other.Price);
    }

    public override string ToString()
    {
        return $"{Name}: {Price:C}";
    }
}

// 自定义比较器
public class ProductNameComparer : IComparer<Product>
{
    public int Compare(Product x, Product y)
    {
        if (x is null && y is null) return 0;
        if (x is null) return -1;
        if (y is null) return 1;

        return string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
    }
}