using System.Collections.Immutable;

namespace xUnitTest.CSharp特性.集合表达式;

public class CollectionExpressions
{
    private void Run()
    {
        //1. 用 [...] 表示一个集合，具体生成什么类型由左边的目标类型决定。这叫"目标类型化"
        int[] a = [1, 2, 3];
        List<string> b = ["x", "y"];
        Span<int> c = [1, 2, 3];

        //2. 方括号里可以混搭任意组合：
        int[] arr = [1, 2, 3];
        List<int> list = [4, 5];

        int[] result =
        [
            0, // 单个元素
            .. arr, // 展开一个集合
            .. list, // 展开另一个集合
            6 // 再来单个元素
        ];
        // [0, 1, 2, 3, 4, 5, 6]

        //3. 同一个 [...] 写法的含义取决于左边：
        int[] x1 = [1, 2, 3]; // 数组
        List<int> x2 = [1, 2, 3]; // List<int>
        Span<int> x3 = [1, 2, 3]; // Span<int>
        ReadOnlySpan<int> x4 = [1, 2, 3];
        IEnumerable<int> x5 = [1, 2, 3]; // 编译器选合适的具体类型
        ImmutableArray<int> x6 = [1, 2, 3]; // 也支持

        //4. 空集合
        int[] empty = [];
        List<int> emptyList = [];

        //5. 展开任意可枚举对象
        IEnumerable<int> GetNums()
        {
            yield return 1;
            yield return 2;
        }
        //可简写为
        int[] r = [0, .. GetNums(), 3]; // [0, 1, 2, 3]

        //展开时元素类型可以隐式转换
        long[] s = [1, 2, .. new int[] { 3, 4 }]; // int 隐式转 long

        //嵌套展开
        int[][] matrix = [[1, 2], [3, 4]];
        int[] flat = [.. matrix[0], .. matrix[1]]; // [1, 2, 3, 4]

        //展开与条件结合
        bool includeExtra = true;
        int[] t = [1, 2, .. includeExtra ? new[] { 3, 4 } : Array.Empty<int>()];
    }

    //一个综合例子
    public static string[] BuiltIn = ["csharp", "dotnet"];
    public string[] Custom = ["unity"];

    public string[] All(bool includeBuiltIn) =>
    [
        .. includeBuiltIn ? BuiltIn : [],
        .. Custom,
        "extra"
    ];
}