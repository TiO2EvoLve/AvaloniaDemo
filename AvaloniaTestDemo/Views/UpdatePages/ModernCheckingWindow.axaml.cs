using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using NetSparkleUpdater.Interfaces;

namespace AvaloniaTestDemo.Views.UpdatePages;

public partial class ModernCheckingWindow : Window, ICheckingForUpdates
{
    public ModernCheckingWindow()
    {
        InitializeComponent();
        Closing += OnClosing;
    }

    public event EventHandler? UpdatesUIClosing;

    void ICheckingForUpdates.Show() => Show();

    void ICheckingForUpdates.Close() => Close();

    private void Cancel_Click(object? sender, RoutedEventArgs e) => Close();

    private void OnClosing(object? sender, WindowClosingEventArgs e)
    {
        Closing -= OnClosing;
        UpdatesUIClosing?.Invoke(this, EventArgs.Empty);
    }
}
