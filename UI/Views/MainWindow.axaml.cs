using Avalonia.Controls;
using Avalonia.Input;

namespace UI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // Attach pointer handler to custom title bar so the window can be dragged
        // when using extended client area / custom chrome.
        var titleBar = this.FindControl<Control>("TitleBar");
        if (titleBar != null)
        {
            titleBar.PointerPressed += TitleBar_PointerPressed;
        }
    }

    private void TitleBar_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        // Only react to left button presses
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            // Double-click to toggle maximize/restore
            if (e.ClickCount == 2)
            {
                WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            }
            else
            {
                // Begin window drag (Avalonia Window API)
                BeginMoveDrag(e);
            }
        }
    }
}