using Avalonia.Controls;
using Avalonia.Interactivity;
using NetSparkleUpdater;
using NetSparkleUpdater.Enums;
using NetSparkleUpdater.Events;
using NetSparkleUpdater.Interfaces;

namespace AvaloniaTestDemo.Views.UpdatePages;

public partial class ModernUpdateWindow : Window, IUpdateAvailable
{
    private bool _responded;

    public ModernUpdateWindow(AppCastItem item, string appName)
    {
        CurrentItem = item;
        AppName = string.IsNullOrWhiteSpace(appName) ? "应用" : appName;
        Version = item.Version ?? string.Empty;
        Description = string.IsNullOrWhiteSpace(item.Description)
            ? "此版本包含稳定性改进与体验优化。"
            : item.Description;
        InitializeComponent();
        DataContext = this;
        Closing += (_, _) => RespondIfNeeded(UpdateAvailableResult.RemindMeLater);
    }

    public event UserRespondedToUpdate? UserResponded;
    public UpdateAvailableResult Result { get; private set; }
    public AppCastItem CurrentItem { get; }
    public string AppName { get; }
    public string Version { get; }
    public string Description { get; }

    public void HideReleaseNotes() { }
    public void HideRemindMeLaterButton() { }
    public void HideSkipButton() { }
    public void BringToFront() => Activate();

    private void Update_Click(object? sender, RoutedEventArgs e) => Respond(UpdateAvailableResult.InstallUpdate);
    private void Later_Click(object? sender, RoutedEventArgs e) => Respond(UpdateAvailableResult.RemindMeLater);
    private void Skip_Click(object? sender, RoutedEventArgs e) => Respond(UpdateAvailableResult.SkipUpdate);

    private void Respond(UpdateAvailableResult result)
    {
        RespondIfNeeded(result);
        Close();
    }

    private void RespondIfNeeded(UpdateAvailableResult result)
    {
        if (_responded) return;
        _responded = true;
        Result = result;
        UserResponded?.Invoke(this, new UpdateResponseEventArgs(result, CurrentItem));
    }
}
