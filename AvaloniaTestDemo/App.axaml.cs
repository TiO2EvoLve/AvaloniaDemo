using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaTestDemo.Common;
using AvaloniaTestDemo.Services;
using AvaloniaTestDemo.Views;
using Microsoft.Extensions.DependencyInjection;
using Avalonia.Collections;
using NetSparkleUpdater;
using NetSparkleUpdater.Enums;
using NetSparkleUpdater.SignatureVerifiers;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using UpdateTest;
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
            
            //注册自动更新服务
            services.AddSingleton<SparkleUpdater>(_ =>
            {
                //更新地址
                const string appCastUrl = "https://gitee.com/tio2evolve/UpdateTest/releases/download/V1.0.1/update.xml";
                var verifier = new Ed25519Checker(SecurityMode.Unsafe, string.Empty, string.Empty);
                var updater = new SparkleUpdater(appCastUrl, verifier)
                {
                    UIFactory = new ModernUpdaterFactory(),
                    UserInteractionMode = UserInteractionMode.DownloadAndInstall,
                };
                return updater;
            });

            services.AddSingleton<UpdateViewModel>();
            
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
            .AddView<TestView, TestViewModel>(services)
            .AddView<BindingView, BindingViewModel>(services)
            .AddView<StateMachineView, StateMachineViewModel>(services)
            .AddView<QRCodeView, QRCodeViewModel>(services)
            .AddView<SendEmailView, SendEmailViewModel>(services)
            .AddView<DragDropView, DragDropViewModel>(services)
            .AddView<DynamicDataView, DynamicDataViewModel>(services)
            .AddView<HarmonyModView, HarmonyModViewModel>(services)
            .AddView<LineRenderView, LineRenderViewModel>(services)
            .AddView<SqlServerView, SqlServerViewModel>(services)
            .AddView<PhotoDropView, PhotoDropViewModel>(services)
            .AddView<UpdateView, UpdateViewModel>(services)
            .AddView<LiteDBView, LiteDBViewModel>(services)
            .AddView<WorkFlowView, WorkFlowViewModel>(services)
            ;
    }

    private static ServiceProvider ConfigureServices(ServiceCollection services)
    {
        services.AddSingleton<PageNavigationService>();
        services.AddSingleton<ISukiToastManager, SukiToastManager>();
        services.AddSingleton<ISukiDialogManager, SukiDialogManager>();
        services.AddSingleton<PageNavigationService>();
        services.AddSingleton<IAvaloniaReadOnlyList<DemoPageBase>>(sp =>
        {
            var pages = sp.GetServices<DemoPageBase>().OrderBy(x => x.Index).ThenBy(x => x.DisplayName);
            return new AvaloniaList<DemoPageBase>(pages);
        });

        return services.BuildServiceProvider();
    }
}