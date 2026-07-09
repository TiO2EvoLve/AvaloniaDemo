using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace AvaloniaTestDemo.Windows;

public partial class Add : Window
{
    public Add()
    {
        InitializeComponent();
    }

    private void Close(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}