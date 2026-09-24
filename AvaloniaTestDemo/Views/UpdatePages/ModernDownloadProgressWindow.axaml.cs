using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Interactivity;
using NetSparkleUpdater.Events;
using NetSparkleUpdater.Interfaces;

namespace AvaloniaTestDemo.Views.UpdatePages;

public partial class ModernDownloadProgressWindow : Window, IDownloadProgress, INotifyPropertyChanged
{
    private string _heading = string.Empty;
    private string _detail = string.Empty;
    private double _progress;
    private bool _isReady;
    private bool _hasError;
    private string _errorText = string.Empty;

    public ModernDownloadProgressWindow(string title, string initialMessage)
    {
        Heading = title;
        Detail = initialMessage;
        InitializeComponent();
        DataContext = this;
    }

    public event DownloadInstallEventHandler? DownloadProcessCompleted;
    public string Heading { get => _heading; set => SetField(ref _heading, value); }
    public string Detail { get => _detail; set => SetField(ref _detail, value); }
    public double Progress { get => _progress; set => SetField(ref _progress, value); }
    public bool IsReady { get => _isReady; set => SetField(ref _isReady, value); }
    public bool HasError { get => _hasError; set => SetField(ref _hasError, value); }
    public string ErrorText { get => _errorText; set => SetField(ref _errorText, value); }
    public string ActionText => "立即安装";

    public void SetDownloadAndInstallButtonEnabled(bool enabled) => IsReady = enabled;
    public void OnDownloadProgressChanged(object? sender, ItemDownloadProgressEventArgs e)
    {
        Progress = e.ProgressPercentage;
        Detail = e.TotalBytesToReceive > 0
            ? $"已下载 {e.BytesReceived / 1024d / 1024d:F1} / {e.TotalBytesToReceive / 1024d / 1024d:F1} MB"
            : $"已下载 {e.BytesReceived / 1024d / 1024d:F1} MB";
    }

    public void FinishedDownloadingFile(bool isValid)
    {
        IsReady = isValid;
        Heading = isValid ? "更新已准备好" : "更新文件无效";
        Detail = isValid ? "点击“立即安装”以完成更新。" : "请稍后重新检查更新。";
    }

    public bool DisplayErrorMessage(string message)
    {
        HasError = true;
        ErrorText = message;
        Heading = "下载遇到问题";
        return true;
    }

    private void Install_Click(object? sender, RoutedEventArgs e) => Complete(true);
    private void Cancel_Click(object? sender, RoutedEventArgs e) => Complete(false);

    private void Complete(bool shouldInstall)
    {
        DownloadProcessCompleted?.Invoke(this, new DownloadInstallEventArgs(shouldInstall));
        Close();
    }

    public new event PropertyChangedEventHandler? PropertyChanged;

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (Equals(field, value)) return;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
