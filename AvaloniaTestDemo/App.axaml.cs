using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Collections;
using Avalonia.Markup.Xaml;

using AvaloniaTestDemo.Common;
using AvaloniaTestDemo.Services;
using AvaloniaTestDemo.Views;
using AvaloniaTestDemo.Views.WorkFlow;

using Microsoft.Extensions.DependencyInjection;

using NetSparkleUpdater;
using NetSparkleUpdater.Enums;
using NetSparkleUpdater.SignatureVerifiers;

using SukiUI.Dialogs;
using SukiUI.Toasts;

using UpdateTest;

using WorkflowCore.Interface;

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


            // ==========================================
            // WorkflowCore
            // ==========================================

            services.AddWorkflow();
            services.AddLogging();

            services.AddSingleton<WorkFlowViewModel>();


            // ==========================================
            // 自动更新
            // ==========================================

            services.AddSingleton<SparkleUpdater>(_ =>
            {
                const string appCastUrl = "https://gitee.com/tio2evolve/UpdateTest/releases/download/V1.0.1/update.xml";

                var verifier = new Ed25519Checker(
                    SecurityMode.Unsafe,
                    string.Empty,
                    string.Empty);

                var updater = new SparkleUpdater(
                    appCastUrl,
                    verifier)
                {
                    UIFactory = new ModernUpdaterFactory(),
                    UserInteractionMode = UserInteractionMode.DownloadAndInstall,
                };

                return updater;
            });

            services.AddSingleton<UpdateViewModel>();


            // ==========================================
            // 注册 View / ViewModel
            // ==========================================

            var views = ConfigureViews(services);


            // ==========================================
            // 其他服务
            // ==========================================

            var provider = ConfigureServices(services);


            // ==========================================
            // WorkflowCore 注册
            // ==========================================

            var host = provider.GetRequiredService<IWorkflowHost>();

            host.RegisterWorkflow<
                ApprovalWorkflow,
                ApprovalWorkflowData>();

            host.Start();


            // ==========================================
            // Avalonia ViewLocator
            // ==========================================

            DataTemplates.Add(new ViewLocator(views));


            // ==========================================
            // 创建主窗口
            // ==========================================

            desktop.MainWindow =
                views.CreateView<MainWindowViewModel>(provider) as Window;
        }

        base.OnFrameworkInitializationCompleted();
    }


    private static SukiViews ConfigureViews(
        ServiceCollection services)
    {
        return new SukiViews()

            // Main Window
            .AddView<MainWindow, MainWindowViewModel>(services)

            // Pages
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
            .AddView<WorkFlowView, WorkFlowViewModel>(services);
    }


    private static ServiceProvider ConfigureServices(
        ServiceCollection services)
    {
        services.AddSingleton<PageNavigationService>();

        services.AddSingleton<ISukiToastManager, SukiToastManager>();

        services.AddSingleton<ISukiDialogManager, SukiDialogManager>();

        services.AddSingleton<IAvaloniaReadOnlyList<DemoPageBase>>(sp =>
        {
            var pages = sp
                .GetServices<DemoPageBase>()
                .OrderBy(x => x.Index)
                .ThenBy(x => x.DisplayName);

            return new AvaloniaList<DemoPageBase>(pages);
        });

        // 整个应用只在这里创建一次 ServiceProvider
        return services.BuildServiceProvider();
    }
}