using System;
using System.Threading.Tasks;
using Avalonia.Threading;
using AvaloniaEdit.Document;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Material.Icons;
using NetSparkleUpdater;
using NetSparkleUpdater.Events;

namespace AvaloniaTestDemo.Views;

public partial class UpdateViewModel : DemoPageBase
{
    
    //日志内容
    [ObservableProperty] private TextDocument logDoc = new ();
    [ObservableProperty] public partial double DownloadProgress { get; set; }
    [ObservableProperty] public partial string UpdateStatus { get; set; } = "正在自动检查更新";
    [ObservableProperty] public partial bool IsDownloading { get; set; }
    [ObservableProperty] public partial bool IsChecking { get; set; } = false;
    private readonly SparkleUpdater _updater;


    partial void OnUpdateStatusChanged(string value)
    {
        AddLog(value);
    }


    public UpdateViewModel(SparkleUpdater updater) : base("自动更新", MaterialIconKind.Update, 100)
    {
        _updater = updater;
        _updater.DownloadStarted += OnDownloadStarted;
        _updater.DownloadMadeProgress += OnDownloadMadeProgress;
        _updater.DownloadFinished += OnDownloadFinished;
        _updater.DownloadCanceled += OnDownloadCanceled;
        _updater.DownloadHadError += OnDownloadHadError;
        _updater.UpdateCheckStarted += OnUpdateCheckStarted;
        _updater.UpdateCheckFinished += OnUpdateCheckFinished;
    }

    public void StartAutomaticUpdateChecks()
    {
        _ = _updater.StartLoop(true, TimeSpan.FromHours(24));
    }

    [RelayCommand]
    private async Task Update()
    {
        IsChecking = true;
        UpdateStatus = "正在检查更新...";

        try
        {
            await _updater.CheckForUpdatesAtUserRequest(true);
        }
        catch (Exception ex)
        {
            UpdateStatus = $"检查更新失败：{ex.Message}";
        }
        finally
        {
            IsChecking = false;
        }
    }

    private void OnUpdateCheckStarted(object? sender) => PostToUi(() =>
    {
        IsChecking = true;
        UpdateStatus = "正在检查更新...";
    });

    private void OnUpdateCheckFinished(object? sender, NetSparkleUpdater.Enums.UpdateStatus status) =>
        PostToUi(() =>
        {
            IsChecking = false;
            if (!IsDownloading)
                UpdateStatus = status == NetSparkleUpdater.Enums.UpdateStatus.UpdateAvailable
                    ? "发现新版本，请在更新窗口中确认"
                    : "当前已是最新版本";
        });

    private void OnDownloadStarted(NetSparkleUpdater.AppCastItem item, string path) => PostToUi(() =>
    {
        IsDownloading = true;
        DownloadProgress = 0;
        UpdateStatus = $"正在下载 {item.Version}...";
    });

    private void OnDownloadMadeProgress(object? sender, NetSparkleUpdater.AppCastItem item, ItemDownloadProgressEventArgs args) => PostToUi(() =>
    {
        IsDownloading = true;
        DownloadProgress = args.ProgressPercentage;
        UpdateStatus = args.TotalBytesToReceive > 0
            ? $"正在下载更新：{args.ProgressPercentage}% ({args.BytesReceived / 1024d / 1024d:F1} / {args.TotalBytesToReceive / 1024d / 1024d:F1} MB)"
            : $"正在下载更新：{args.BytesReceived / 1024d / 1024d:F1} MB";
    });

    private void OnDownloadFinished(NetSparkleUpdater.AppCastItem item, string path) => PostToUi(() =>
    {
        IsDownloading = false;
        DownloadProgress = 100;
        UpdateStatus = "更新下载完成，正在准备安装";
    });

    private void OnDownloadCanceled(NetSparkleUpdater.AppCastItem item, string path) => PostToUi(() =>
    {
        IsDownloading = false;
        UpdateStatus = "更新下载已取消";
    });

    private void OnDownloadHadError(NetSparkleUpdater.AppCastItem item, string? path, Exception exception) => PostToUi(() =>
    {
        IsDownloading = false;
        UpdateStatus = $"更新下载失败：{exception.Message}";
    });

    private static void PostToUi(Action action) => Dispatcher.UIThread.Post(action);

    public void Dispose()
    {
        _updater.StopLoop();
        _updater.Dispose();
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