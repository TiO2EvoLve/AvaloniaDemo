using LanguageExt;
using Xunit.Abstractions;
using static LanguageExt.Prelude;
namespace xUnitTest.函数式编程.language_ext;

public class Sequence与Traverse(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void Run()
    {
        // Sequence: Seq<Option<int>> -> Option<Seq<int>>
        var seqOfOptions = Seq(Some(1), Some(2), Some(3));

        // 手动实现 sequence: 将 Seq<Option<T>> 转为 Option<Seq<T>>（遇到 None 则返回 None）
        Option<Seq<int>> maybeSeq = seqOfOptions.Fold(Some<Seq<int>>(Seq<int>()), (accOpt, itemOpt) =>
            accOpt.Bind(acc => itemOpt.Map(item => acc.Add(item)))
        );

        maybeSeq.Match(
            Some: s => testOutputHelper.WriteLine($"Sequence 成功: {string.Join(",", s.Map(x => x.ToString()))}"),
            None: () => testOutputHelper.WriteLine("Sequence 返回 None")
        );

        // Traverse: 将 Seq<string> 映射为 Seq<Option<int>> 后再 sequence
        var stringSeq = Seq("1", "2", "x");

        Option<int> TryParse(string s) => int.TryParse(s, out var v) ? Some(v) : Option<int>.None;

        var parsedSeq = stringSeq.Map(TryParse);

        // 使用相同的折叠方法来 sequence
        var traversed = parsedSeq.Fold(Some<Seq<int>>(Seq<int>()), (accOpt, itemOpt) =>
            accOpt.Bind(acc => itemOpt.Map(item => acc.Add(item)))
        );

        traversed.Match(
            Some: s => testOutputHelper.WriteLine($"Traverse 成功: {string.Join(",", s.Map(x => x.ToString()))}"),
            None: () => testOutputHelper.WriteLine("Traverse 返回 None（有某项解析失败）")
        );
    }
}

