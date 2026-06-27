using Avalonia;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaTestDemo.Models;

public class ConnectionViewModel : ObservableObject
{
    public NodeViewModel Start { get; }
    public NodeViewModel End { get; }

    private Geometry? _geometry;

    public Geometry? Geometry
    {
        get => _geometry;
        set => SetProperty(ref _geometry, value);
    }

    public ConnectionViewModel(NodeViewModel start, NodeViewModel end)
    {
        Start = start;
        End = end;

        Start.PropertyChanged += (_, _) => Update();
        End.PropertyChanged += (_, _) => Update();

        Update();
    }

    private void Update()
    {
        var start = new Point(Start.CenterX, Start.CenterY);
        var end = new Point(End.CenterX, End.CenterY);

        var dx = end.X - start.X;

        var cp1 = new Point(start.X + dx * 0.5, start.Y);
        var cp2 = new Point(end.X - dx * 0.5, end.Y);

        var geometry = new StreamGeometry();

        using (var ctx = geometry.Open())
        {
            ctx.BeginFigure(start, false);
            ctx.CubicBezierTo(cp1, cp2, end);
        }

        Geometry = geometry;
    }
}