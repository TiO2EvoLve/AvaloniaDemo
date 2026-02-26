using Xunit.Abstractions;

namespace xUnitTest.Interface;

//可等同性IEquatable接口测试
public class IEquatableTest(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void Run()
    {
        // 使用示例
        var person1 = new Person { Name = "John", Age = 25 };
        var person2 = new Person { Name = "John", Age = 25 };

        Assert.Equal(person1, person2);
    }
}

public class Person : IEquatable<Person>
{
    public string Name { get; set; }
    public int Age { get; set; }

    // 实现IEquatable<T>
    public bool Equals(Person other)
    {
        if (other is null) return false;
        return Name == other.Name && Age == other.Age;
    }

    // 重写Object.Equals保持一致性
    public override bool Equals(object obj)
    {
        return Equals(obj as Person);
    }

    // 重写GetHashCode很重要！
    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Age);
    }
}