using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ReactiveTest.ViewModels;
using ReactiveTest.Views;
using ReactiveTest.Views.UserControl;
using Sextant;
using Sextant.Avalonia;
using Splat;

namespace ReactiveTest;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var resolver = AppLocator.CurrentMutable;
        var navigationView = new NavigationView();

        // Sextant 注册：
        // 1. 每个页面把「视图 + 视图模型」注册给 ReactiveUI 的 ViewLocator，
        //    NavigationView 之后就能根据视图模型找到对应视图；
        // 2. RegisterViewModelFactory 让 PushPage<TViewModel>() 能够从容器解析视图模型；
        // 3. RegisterNavigationView 创建导航视图并注册 IViewStackService。
        resolver.RegisterViewForNavigation<ReadCardDemo, ReadCardDemoViewModel>(
            () => new ReadCardDemo(),
            () => new ReadCardDemoViewModel());
        resolver.RegisterViewForNavigation<Test, TestViewModel>(
            () => new Test(),
            () => new TestViewModel());
        resolver.RegisterViewModelFactory(() => new DefaultViewModelFactory());
        resolver.RegisterNavigationView(() => navigationView);

        var viewStack = AppLocator.Current.GetService<IViewStackService>()
            ?? throw new InvalidOperationException("IViewStackService 注册失败，请检查 Sextant 的注册顺序。");

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // 外壳窗口：顶部导航按钮固定，页面区域交给 Sextant 的导航视图。
            var mainViewModel = new MainViewModel(viewStack);
            var mainWindow = new MainWindow { DataContext = mainViewModel };
            mainWindow.NavigationHost.Content = navigationView;
            desktop.MainWindow = mainWindow;

            // 推入根页面（Splat/Rx 没有无参 Subscribe()，显式忽略通知值）。
            viewStack.PushPage<ReadCardDemoViewModel>(resetStack: true).Subscribe(_ => { });
        }

        base.OnFrameworkInitializationCompleted();
    }
}
