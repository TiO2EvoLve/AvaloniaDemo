using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;

namespace AvaloniaTestDemo.Views.UpdatePages;

public partial class ModernMessageWindow : Window
{
    public ModernMessageWindow() : this("更新提示", "提示", string.Empty, string.Empty, false)
    {
    }

    private ModernMessageWindow(string title, string heading, string detail, string hint, bool isError)
    {
        Title = title;
        Heading = heading;
        Detail = detail;
        Hint = hint;
        IconGlyph = isError ? "!" : "✓";
        AccentBackground = new SolidColorBrush(isError ? Color.Parse("#FEE2E2") : Color.Parse("#D1FAE5"));
        AccentForeground = new SolidColorBrush(isError ? Color.Parse("#B91C1C") : Color.Parse("#065F46"));
        InitializeComponent();
        DataContext = this;
    }

    public string Heading { get; }
    public string Detail { get; }
    public string Hint { get; }
    public string IconGlyph { get; }
    public IBrush AccentBackground { get; }
    public IBrush AccentForeground { get; }

    public static void ShowInfo(string heading, string detail, string hint) =>
        ShowOnUi(new ModernMessageWindow("检查更新", heading, detail, hint, false));

    public static void ShowError(string heading, string detail, string hint) =>
        ShowOnUi(new ModernMessageWindow("更新提示", heading, detail, hint, true));

    private static void ShowOnUi(ModernMessageWindow window)
    {
        if (Dispatcher.UIThread.CheckAccess())
            window.Show();
        else
            Dispatcher.UIThread.Post(window.Show);
    }

    private void Ok_Click(object? sender, RoutedEventArgs e) => Close();
}
