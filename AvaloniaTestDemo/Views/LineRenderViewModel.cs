using System;
using System.Diagnostics;
using Avalonia.Threading;
using AvaloniaTestDemo.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using Material.Icons;


namespace AvaloniaTestDemo.Views;

public partial class LineRenderViewModel : DemoPageBase
{
    public NodeViewModel StartNode { get; }
    public NodeViewModel EndNode { get; }
    public ConnectionViewModel Connection { get; }
    
    [ObservableProperty]
    private double dashOffset;
    
    private readonly Stopwatch _stopwatch = new();
    private double _lastTime;

    public LineRenderViewModel() : base("Line Render", MaterialIconKind.LineScan, 100)
    { 
        
        StartNode = new NodeViewModel
        {
            X = 50,
            Y = 100
        };

        EndNode = new NodeViewModel
        {
            X = 300,
            Y = 200
        };

        Connection = new ConnectionViewModel(
            StartNode,
            EndNode);
        
        _stopwatch.Start();
        
        DispatcherTimer.Run(
            UpdateAnimation,
            TimeSpan.FromMilliseconds(16));
    }
    
    private bool UpdateAnimation()
    {
        var now = _stopwatch.Elapsed.TotalSeconds;

        var delta = now - _lastTime;

        _lastTime = now;

        DashOffset -= 40 * delta;

        return true;
    }
}