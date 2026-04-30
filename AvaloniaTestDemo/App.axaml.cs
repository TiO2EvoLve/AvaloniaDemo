using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaTestDemo.Common;
using AvaloniaTestDemo.Services;
using AvaloniaTestDemo.Views;
using Microsoft.Extensions.DependencyInjection;
using Avalonia.Collections;
using DemoPageBase = AvaloniaTestDemo.Views.DemoPageBase;

namespace AvaloniaTestDemo;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var services = new ServiceCollection();
            services.AddSingleton(desktop);
            var views = ConfigureViews(services);
            var provider = ConfigureServices(services);
            DataTemplates.Add(new ViewLocator(views));
            desktop.MainWindow = views.CreateView<MainWindowViewModel>(provider) as Window;
        }
        base.OnFrameworkInitializationCompleted();
    }

    private static SukiViews ConfigureViews(ServiceCollection services)
    {
        return new SukiViews()

            // Add main view
            .AddView<MainWindow, MainWindowViewModel>(services)
            // Add pages
            .AddView<LinQView, LinQViewModel>(services)
            .AddView<SettingView, SettingViewModel>(services)
            .AddView<ReactiveView, ReactiveViewModel>(services)
            .AddView<CancellationTokenView, CancellationTokenViewModel>(services)
            .AddView<Test, TestViewModel>(services)
            .AddView<BindingView, BindingViewModel>(services)
            .AddView<StateMachineView, StateMachineViewModel>(services);
    }

    private static ServiceProvider ConfigureServices(ServiceCollection services)
    {
        services.AddSingleton<PageNavigationService>();
        services.AddSingleton<IAvaloniaReadOnlyList<DemoPageBase>>(sp =>
        {
            var pages = sp.GetServices<DemoPageBase>().OrderBy(x => x.Index).ThenBy(x => x.DisplayName);
            return new AvaloniaList<DemoPageBase>(pages);
        });

        return services.BuildServiceProvider();
    }

    // //禁用 Avalonia 内置的 DataAnnotations 验证功能，以防止与 CommunityToolkit 的冲突。Mvvm 验证
    // private void DisableAvaloniaDataAnnotationValidation()
    // {
    //     // Get an array of plugins to remove
    //     var dataValidationPluginsToRemove =
    //         BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();
    //
    //     // remove each entry found
    //     foreach (var plugin in dataValidationPluginsToRemove)
    //     {
    //         BindingPlugins.DataValidators.Remove(plugin);
    //     }
    // }
}