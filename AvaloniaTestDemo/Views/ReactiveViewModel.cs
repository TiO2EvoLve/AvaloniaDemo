using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Material.Icons;

namespace AvaloniaTestDemo.Views;

public partial class ReactiveViewModel() : DemoPageBase("Reactive", MaterialIconKind.Abc, int.MinValue),IDisposable
{
    private readonly Subject<string> _textChanged = new();
    [ObservableProperty]private string _time = "点击按钮开始倒计时";
    public ObservableCollection<string> LogContent { get; set; }= [];
    [ObservableProperty]
    private string searchText = "";

    [ObservableProperty]
    private string result = "等待输入…";
    
    [RelayCommand]
    private void Timer()
    {
        const int totalSeconds = 10;
        // 创建一个每秒触发的流
        Observable.Interval(TimeSpan.FromSeconds(1))
            .Take(totalSeconds + 1) // 10 秒 + 0 秒（显示起点）
            .Select(i => totalSeconds - (int)i) // 计算剩余秒数
            .ObserveOn(SynchronizationContext.Current!)
            .Subscribe(
                remaining => { Time = $"剩余 {remaining} 秒"; },
                () => { Time = "倒计时结束！"; });
    }

    [RelayCommand]
    private void Retry()
    {
        LogContent.Clear();  // 清空之前的日志
        var random = new Random();
        var attemptCount = 0;
        const int maxAttempts = 5;// 最大尝试次数
        var hasSuccess = false;

        Observable.Interval(TimeSpan.FromSeconds(1))
            .ObserveOn(SynchronizationContext.Current!)
            .TakeWhile(_ => !hasSuccess && attemptCount < maxAttempts)
            .Do(_ =>
            {
                attemptCount++;
                var randomNumber = random.Next( 11);
                // 确保每次抽取都立即输出
                LogContent.Add($"第{attemptCount}次尝试: 抽到数字 {randomNumber}");
                if (randomNumber == 5)
                {
                    hasSuccess = true;
                    LogContent.Add(" - 成功！");
                    LogContent.Add($"恭喜！在第{attemptCount}次成功抽到数字5");
                }
                else
                {
                    LogContent.Add(" - 失败");
                }
            })
            .Subscribe(
                _ => { },
                ex => LogContent.Add($"最终结果: {ex.Message}"),
                () =>
                {
                    if (!hasSuccess) LogContent.Add($"已达到最大尝试次数{maxAttempts}，未能抽到数字5");
                    LogContent.Add("抽奖过程结束");
                }
            );
        
    }

    [RelayCommand]
    private void Shake()
    {
        _textChanged
            .Throttle(TimeSpan.FromMilliseconds(1000))
            .DistinctUntilChanged()
            .Subscribe(text =>
            {
                Result = $"搜索：{text}";
            });
    }
    [ObservableProperty]
    private bool canClick = true;

    [RelayCommand]
    private void Click()
    {
        if (!CanClick) return;

        CanClick = false;

        Observable.Timer(TimeSpan.FromSeconds(3))
            .Subscribe(_ => CanClick = true);
    }

    partial void OnSearchTextChanged(string value)
    {
        _textChanged.OnNext(value);
    }
    
    public void Dispose()
    {
        _textChanged.Dispose();
    }
    
}