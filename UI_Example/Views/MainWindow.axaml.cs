using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace UI_Example.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Close(object? sender, PointerPressedEventArgs e)
    {
        Close();
    }

    private void MaxWindow(object? sender, PointerPressedEventArgs e)
    {
        WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    }

    private void MinWindow(object? sender, PointerPressedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }


    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        Web page = new Web();
        page.Show();
    }
}