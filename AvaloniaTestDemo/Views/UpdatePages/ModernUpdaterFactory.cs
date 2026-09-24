using System.Collections.Generic;
using AvaloniaTestDemo.Views.UpdatePages;
using NetSparkleUpdater;
using NetSparkleUpdater.Interfaces;
using NetSparkleUpdater.UI.Avalonia;


namespace UpdateTest;

/// <summary>Bridges NetSparkle's update workflow to this application's Avalonia windows.</summary>
public sealed class ModernUpdaterFactory : UIFactory
{
    public override IUpdateAvailable CreateUpdateAvailableWindow(
        List<AppCastItem> updates,
        ISignatureVerifier? signatureVerifier,
        string appNameTitle,
        string installedVersion,
        bool isUpdateAlreadyDownloaded) => new ModernUpdateWindow(updates[0], appNameTitle);

    public override IDownloadProgress CreateProgressWindow(string title, string initialMessage) =>
        new ModernDownloadProgressWindow(title, initialMessage);

    public override ICheckingForUpdates ShowCheckingForUpdates() => new ModernCheckingWindow();

    public override void ShowVersionIsUpToDate() =>
        ModernMessageWindow.ShowInfo("已是最新版本", "当前安装的版本已是最新。", "无需下载，可以继续使用现有版本。");

    public override void ShowVersionIsSkippedByUserRequest() =>
        ModernMessageWindow.ShowInfo("已跳过此版本", "你之前选择跳过该更新。", "如需安装，可稍后再次手动检查更新。");

    public override void ShowCannotDownloadAppcast(string? appcastUrl) =>
        ModernMessageWindow.ShowError("无法检查更新", "暂时无法获取更新信息。", "请检查网络后重试。");

    public override void ShowUnknownInstallerFormatMessage(string downloadFileName) =>
        ModernMessageWindow.ShowError("安装包格式未知", "无法识别下载的安装文件。", downloadFileName);

    public override void ShowDownloadErrorMessage(string message, string? appcastUrl) =>
        ModernMessageWindow.ShowError("下载失败", "更新文件下载未完成。", message);
}
