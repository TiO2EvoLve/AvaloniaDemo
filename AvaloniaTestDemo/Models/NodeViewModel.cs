using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaTestDemo.Models;

public partial class NodeViewModel : ObservableObject
{
    [ObservableProperty]
    private double x;

    [ObservableProperty]
    private double y;

    public double CenterX => X + 10;
    public double CenterY => Y + 10;

    partial void OnXChanged(double value)
    {
        OnPropertyChanged(nameof(CenterX));
    }

    partial void OnYChanged(double value)
    {
        OnPropertyChanged(nameof(CenterY));
    }
}