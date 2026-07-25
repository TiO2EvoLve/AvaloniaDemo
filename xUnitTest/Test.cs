using Xunit.Abstractions;

namespace xUnitTest;

public class Test(ITestOutputHelper TS)
{
    [Fact]
    private void Run()
    {
        TS.WriteLine(CalculateXorMod10("9110112345678").ToString());
    }

    private static int CalculateXorMod10(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            throw new ArgumentException("输入不能为空");
        }

        if (input.Length != 13)
        {
            throw new ArgumentException("输入必须是13位号码段");
        }

        // 初始化异或结果为0
        int xorResult = 0;

        // 遍历每个字符，按16进制进行异或
        foreach (var c in input)
        {
            // 将字符转换为16进制数值
            int hexValue = ConvertCharToHex(c);
            
            // 进行异或运算
            xorResult ^= hexValue;
        }

        // 对10取余
        return xorResult % 10;
    }

    /// <summary>
    /// 将字符转换为16进制数值（0-15）
    /// </summary>
    private static int ConvertCharToHex(char c)
    {
        if (c >= '0' && c <= '9')
        {
            return c - '0';
        }
        else if (c >= 'A' && c <= 'F')
        {
            return c - 'A' + 10;
        }
        else if (c >= 'a' && c <= 'f')
        {
            return c - 'a' + 10;
        }
        else
        {
            throw new ArgumentException($"无效的16进制字符: {c}");
        }
    }
}