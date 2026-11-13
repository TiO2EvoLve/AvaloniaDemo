using System;
using System.Reactive.Linq;
using ReactiveUI.SourceGenerators;
using Sextant;

namespace ReactiveTest.ViewModels;

/// <summary>
/// 外壳窗口的视图模型：负责通过 Sextant 的 <see cref="IViewStackService"/> 切换页面。
/// </summary>
public partial class MainViewModel(IViewStackService viewStack) : ViewModelBase
{
    /// <summary>
    /// 切换页面。页面视图模型由 Sextant 的 <see cref="IViewModelFactory"/> 从容器中解析。
    /// </summary>
    /// <param name="page">页面标识：readcard / test。</param>
    [ReactiveCommand]
    private void Navigate(string page)
    {
        var navigation = page switch
        {
            "test" => viewStack.PushPage<TestViewModel>(resetStack: true),
            _ => viewStack.PushPage<ReadCardDemoViewModel>(resetStack: true),
        };

        // Splat/Rx 中没有无参的 Subscribe()，这里显式忽略导航完成的通知值。
        navigation.Subscribe(_ => { });
    }
}
