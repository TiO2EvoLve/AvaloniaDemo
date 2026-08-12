
using System;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using Material.Icons;


namespace AvaloniaTestDemo.Views;

public partial class TestViewModel() : DemoPageBase("Test", MaterialIconKind.TestTube, 100)
{
    [RelayCommand]
    private void Test()
    {
        Console.WriteLine(TianjinCardNo.GenerateCardNumber("FD163426",false));
    }
    
   
    public static class TianjinCardNo
    {
        // M1卡校验表
        private static readonly int[] CheckTableM1 =
        {
            0, // 占位
            2,3,4,5,6,7,8,9,0,0,1
        };

        // CPU卡校验表
        private static readonly int[] CheckTableCpu =
        {
            0, // 占位
            3,4,5,6,7,8,9,0,1,1,2
        };

        // 变换矩阵对应的重排顺序
        private static readonly int[] Permutation =
        {
            5,4,8,2,1,10,9,3,7,6
        };

        // 编码替换表
        private static readonly int[] ReplaceTable =
        {
            9,2,1,4,3,6,5,8,7,0
        };

        /// <summary>
        /// 计算天津城市一卡通卡面号
        /// </summary>
        /// <param name="csnHex">8位十六进制CSN，例如94397A83</param>
        /// <param name="isCpuCard">true=CPU卡，false=M1卡</param>
        public static string GenerateCardNumber(string csnHex, bool isCpuCard)
        {
            if (string.IsNullOrWhiteSpace(csnHex) || csnHex.Length != 8)
                throw new ArgumentException("CSN必须为8位16进制字符串");

            // CSN转10位十进制
            uint csn = Convert.ToUInt32(csnHex, 16);
            string decimalStr = csn.ToString("D10");

            // 转数字数组
            int[] digits = decimalStr
                .Select(c => c - '0')
                .ToArray();

            // 矩阵变换
            int[] transformed = new int[10];
            for (int i = 0; i < 10; i++)
            {
                transformed[i] = digits[Permutation[i] - 1];
            }

            // 编码替换
            for (int i = 0; i < 10; i++)
            {
                transformed[i] = ReplaceTable[transformed[i]];
            }

            string first10 = string.Concat(transformed);

            // MOD11校验
            int[] weights = { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3 };

            int sum = 0;
            for (int i = 0; i < 10; i++)
            {
                sum += (first10[i] - '0') * weights[i];
            }

            int x = 11 - (sum % 11);

            int[] checkTable = isCpuCard
                ? CheckTableCpu
                : CheckTableM1;

            int checkDigit = checkTable[x];

            return first10 + checkDigit;
        }
    }
}
