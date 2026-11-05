using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Downloader;

namespace AvaloniaTestDemo.Views;

public partial class DownloaderViewModel : DemoPageBase
{
    private DownloadService? _downloadService;
    private DownloadPackage? _downloadPackage;
    private string _savePath = string.Empty;

    [ObservableProperty]
    private string _url = "https://github.com/flameshot-org/flameshot/releases/download/v14.0.0/Flameshot-14.0.0-win64.msi"; // 测试用大文件

    [ObservableProperty]
    private string _fileName = "Flameshot-14.0.0-win64.msi";

    [ObservableProperty]
    private double _progressPercentage;

    [ObservableProperty]
    private string _speedText = "0 KB/s";

    [ObservableProperty]
    private string _receivedText = "0 B / 0 B";

    [ObservableProperty]
    private string _statusText = "就绪";

    [ObservableProperty]
    private bool _isDownloading;

    [ObservableProperty]
    private bool _canPause;

    [ObservableProperty]
    private bool _canResume;

    [ObservableProperty]
    private bool _canCancel;
    
    public DownloaderViewModel() : base("下载器", Material.Icons.MaterialIconKind.Download, 0)
    {
        // 默认保存到桌面（跨平台路径请按需调整）
        _savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), FileName);
    }
    
     /// <summary>
    /// 开始新下载
    /// </summary>
    [RelayCommand]
    private async Task StartDownloadAsync()
    {
        if (string.IsNullOrWhiteSpace(Url)) return;

        // 更新保存路径
        _savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), FileName);
        
        // 配置下载器：开启自动断点续传、设置分块数量
        var config = new DownloadConfiguration
        {
            ChunkCount = 8,                     // 分块数，提高多线程下载效率
            ParallelDownload = true,
            EnableAutoResumeDownload = true     // 自动保存 .download 文件以便续传
        };

        _downloadService = new DownloadService(config);

        // 订阅事件：进度、完成
        _downloadService.DownloadProgressChanged += OnDownloadProgressChanged;
        _downloadService.DownloadFileCompleted += OnDownloadFileCompleted;

        // 重置 UI 状态
        ProgressPercentage = 0;
        SpeedText = "0 KB/s";
        ReceivedText = "0 B / 0 B";
        IsDownloading = true;
        CanPause = true;
        CanCancel = true;
        CanResume = false;
        StatusText = "下载中...";

        try
        {
            // 启动下载（如果之前有包且未完成，也可通过 DownloadFileTaskAsync(_downloadPackage) 恢复）
            await _downloadService.DownloadFileTaskAsync(Url, _savePath);
        }
        catch (Exception ex)
        {
            StatusText = $"失败：{ex.Message}";
            IsDownloading = false;
            CanPause = CanCancel = CanResume = false;
        }
    }

    /// <summary>
    /// 暂停（快速暂停，保持连接存活）
    /// </summary>
    [RelayCommand]
    private void Pause()
    {
        if (_downloadService == null || !IsDownloading) return;
        _downloadService.Pause();
        StatusText = "已暂停";
        CanPause = false;
        CanResume = true;
    }

    /// <summary>
    /// 继续（从暂停处恢复）
    /// </summary>
    [RelayCommand]
    private void Resume()
    {
        if (_downloadService == null) return;
        _downloadService.Resume();
        StatusText = "下载中...";
        CanPause = true;
        CanResume = false;
    }

    /// <summary>
    /// 取消下载（会保留 Package，可用于后续断点续传）
    /// </summary>
    [RelayCommand]
    private void Cancel()
    {
        if (_downloadService == null) return;
        _downloadService.CancelAsync();
        StatusText = "已取消（可稍后恢复）";
        IsDownloading = false;
        CanPause = CanCancel = false;
        CanResume = true;
    }

    /// <summary>
    /// 基于之前保存的 Package 恢复下载（演示断点续传能力）
    /// </summary>
    [RelayCommand]
    private async Task ResumeFromPackageAsync()
    {
        if (_downloadService?.Package == null || _downloadPackage == null)
        {
            StatusText = "没有可恢复的下载包";
            return;
        }

        IsDownloading = true;
        CanPause = true;
        CanCancel = true;
        CanResume = false;
        StatusText = "恢复下载中...";

        try
        {
            // 使用之前保存的包恢复下载
            await _downloadService.DownloadFileTaskAsync(_downloadPackage);
        }
        catch (Exception ex)
        {
            StatusText = $"恢复失败：{ex.Message}";
            IsDownloading = false;
        }
    }

    private void OnDownloadProgressChanged(object? sender, DownloadProgressChangedEventArgs e)
    {
        // 进度事件在后台线程触发，需调度到 UI 线程
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            ProgressPercentage = e.ProgressPercentage;
            SpeedText = $"{FormatBytes((long)e.BytesPerSecondSpeed)}/s";
            ReceivedText = $"{FormatBytes(e.ReceivedBytesSize)} / {FormatBytes(e.TotalBytesToReceive)}";
        });
    }

    private void OnDownloadFileCompleted(object? sender, AsyncCompletedEventArgs e)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            IsDownloading = false;
            CanPause = CanCancel = CanResume = false;

            if (e.Cancelled)
            {
                StatusText = "下载已取消";
                // 保存包以便恢复
                if (_downloadService?.Package != null)
                {
                    _downloadPackage = _downloadService.Package;
                    CanResume = true;
                }
            }
            else if (e.Error != null)
            {
                StatusText = $"下载失败：{e.Error.Message}";
            }
            else
            {
                StatusText = "下载完成！";
                ProgressPercentage = 100;
            }
        });
    }

    private static string FormatBytes(long bytes)
    {
        string[] suffixes = { "B", "KB", "MB", "GB" };
        int i = 0;
        double d = bytes;
        while (d >= 1024 && i < suffixes.Length - 1)
        {
            d /= 1024;
            i++;
        }
        return $"{d:0.##} {suffixes[i]}";
    }
}