using System;
using System.Threading.Tasks;
using Avalonia.Threading;
using AvaloniaEdit.Document;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Material.Icons;
using Velopack;
using Velopack.Sources;

namespace AvaloniaTestDemo.Views;

public partial class UpdateViewModel() : DemoPageBase("自动更新", MaterialIconKind.Update, 100)
{
    [ObservableProperty] public string version = "V1.0.0";
    [ObservableProperty] public int downloadProgress = 0;
    [ObservableProperty] public bool isUpdating = false;
    //日志内容
    [ObservableProperty] private TextDocument logDoc = new ();
    private UpdateInfo? update;
    private UpdateManager? manager;
    
    [RelayCommand]
    private async Task CheckUpdate()
    {
        try
        {
            await CheckUpdateAsync();
        }
        catch (Exception)
        {
            AddLog("检查更新失败，请查看日志。");
        }
    }

    private async Task CheckUpdateAsync()
    {
        AddLog("开始检查更新。。。");
        try
        {
            AddLog("正在连接更新源: https://github.com/TiO2EvoLve/updateTest");
            manager = new UpdateManager(
                new GithubSource("https://github.com/TiO2EvoLve/updateTest",
                    null,
                    false));

            update = await manager.CheckForUpdatesAsync();

            if (update == null)
            {
                AddLog("检查完成：当前已是最新版本。");
                IsUpdating = false;
                return;
            }

            IsUpdating = true;
            AddLog($"发现新版本: {update.TargetFullRelease.Version}");
        }
        catch (Exception exception)
        {
            AddLog($"检查更新失败: {exception}");
            throw;
        }
    }

    [RelayCommand]
    private async Task ApplyUpdate()
    {
        if (update == null)
        {
            IsUpdating = false;
            return;
        }
        AddLog("开始下载更新。");
        await manager.DownloadUpdatesAsync(
            update,
            progress =>
            {
                DownloadProgress = progress;
            });
        AddLog("更新下载完成，准备应用更新并重启程序。");
        //需要自动安装就打开
        //manager.ApplyUpdatesAndRestart(update);
    }
    // 添加日志
    private void AddLog(string msg)
    {
        Dispatcher.UIThread.Post(() =>
        {
            LogDoc.Insert(LogDoc.TextLength,$"{msg}{Environment.NewLine}");
        });
    }
}