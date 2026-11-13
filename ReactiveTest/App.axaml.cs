using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ReactiveTest.ViewModels;
using ReactiveTest.Views;
using Splat;

namespace ReactiveTest;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
#if DEBUG
        this.AttachDeveloperTools();
#endif
    }

    public override void OnFrameworkInitializationCompleted()
    {
        //从容器中获取MainViewModel实例
        var mainViewModel = AppLocator.Current.GetService<MainViewModel>();
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = mainViewModel,
            };
        }
        
        base.OnFrameworkInitializationCompleted();
    }
}