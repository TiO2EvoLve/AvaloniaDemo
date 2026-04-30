using Xunit.Abstractions;

namespace xUnitTest.函数式编程.language_ext;
using static LanguageExt.Prelude;
//文件更新示例：读取桌面 input.txt，处理后写入 output.txt，使用 Try 来捕获异常并返回结果
public class FileUpdate(ITestOutputHelper testOutputHelper)
{
    [Fact] // 标记为 xUnit 测试方法
    public void Test() // 测试入口方法
    {
        // 用 LINQ 查询语法串联 Try 操作：获取桌面 -> 读文件 -> 处理 -> 写文件 -> 返回输出路径
        var program =
            // 获取桌面路径（Try<string>）
            from desktop in Try(() => Environment.GetFolderPath(Environment.SpecialFolder.Desktop))
            // 组合输入输出路径（let 只是局部绑定，不是副作用）
            let input = Path.Combine(desktop, "input.txt")
            let output = Path.Combine(desktop, "output.txt")
            // 读取文件所有行（若文件不存在或权限不足，Try 将捕获异常）
            from lines in Try(() => File.ReadAllLines(input).ToSeq())
            // 直接在查询里处理每一行：给每行加上日期前缀，返回 Seq<string>
            let processed = lines.Map(line => $"{DateTime.Now:yyyy-MM-dd} {line}")
            // 将处理后的行写回输出文件（将异常包装为 Fail）
            from _ in Try(() => { File.WriteAllLines(output, processed); return unit; })
            // 成功时返回输出文件路径
            select output;

        // 执行 program 并处理结果：成功打印路径，失败打印错误信息
        program.Match(
            Succ: path => testOutputHelper.WriteLine($"处理完成：{path}"),
            Fail: ex => testOutputHelper.WriteLine($"发生错误：{ex.Message}")
        );
    }
}