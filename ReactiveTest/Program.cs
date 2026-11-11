using Avalonia;
using ReactiveUI.Avalonia;
using System;
using DryIoc;
using ReactiveTest.ViewModels;
using ReactiveUI.Avalonia.Splat;

namespace ReactiveTest;

sealed class Program
{
    
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);
    
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUIWithDryIoc(
                container =>
                {
                    container.Register<MainViewModel>(Reuse.Singleton);
                    container.Register<ReadCardDemoViewModel>(Reuse.Singleton);
                    container.Register<TestViewModel>(Reuse.Singleton);
                },
                withReactiveUIBuilder: rxui =>
                {
                    // Optional ReactiveUI customizations
                })
            .RegisterReactiveUIViewsFromEntryAssembly()
            .UseReactiveUI(_ => { });
}