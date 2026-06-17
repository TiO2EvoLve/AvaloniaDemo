using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace AvaloniaTestDemo.Views;

public partial class DragDropView : UserControl
{
    private Canvas? _canvas;
    private Border? _border;
    private Canvas? _gridLayer;
    private DragDropViewModel? _vm;

    public DragDropView()
    {
        InitializeComponent();

        _canvas = this.FindControl<Canvas>("RootCanvas");
        _border = this.FindControl<Border>("DraggableBorder");
        _gridLayer = this.FindControl<Canvas>("GridLayer");

        Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        _vm = this.DataContext as DragDropViewModel ?? _vm;
        _vm?.OnLoaded(_canvas);
        // Subscribe to ViewModel property changes to redraw grid when needed
        if (_vm != null)
        {
            _vm.PropertyChanged += Vm_PropertyChanged;
        }

        // Redraw grid initially
        DrawGrid();

        // Redraw when canvas size changes
        if (_canvas != null)
        {
            _canvas.PropertyChanged += Canvas_PropertyChanged;
        }
    }

    private void Vm_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(DragDropViewModel.ShowGrid) or nameof(DragDropViewModel.GridSize))
        {
            DrawGrid();
        }
    }

    private void Canvas_PropertyChanged(object? sender, Avalonia.AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == Canvas.BoundsProperty)
            DrawGrid();
    }

    private void Border_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        _vm = this.DataContext as DragDropViewModel ?? _vm;
        _vm?.PointerPressed(_border, _canvas, e);
    }

    private void Border_PointerMoved(object? sender, PointerEventArgs e)
    {
        _vm = this.DataContext as DragDropViewModel ?? _vm;
        _vm?.PointerMoved(_border, _canvas, e);
    }

    private void Border_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _vm = this.DataContext as DragDropViewModel ?? _vm;
        _vm?.PointerReleased(e);
    }

    private void Border_PointerCaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        _vm = this.DataContext as DragDropViewModel ?? _vm;
        _vm?.PointerCaptureLost();
    }
    
    private void DrawGrid()
    {
        if (_gridLayer == null || _canvas == null || _vm == null) return;
        _gridLayer.Children.Clear();
        if (!_vm.ShowGrid) return;

        var step = _vm.GridSize;
        if (step <= 0) return;

        var width = _canvas.Bounds.Width;
        var height = _canvas.Bounds.Height;

        var stroke = Brushes.LightGray;

        for (double x = 0; x <= width; x += step)
        {
            var line = new Line
            {
                StartPoint = new Point(x, 0),
                EndPoint = new Point(x, height),
                Stroke = stroke,
                StrokeThickness = 0.5,
                Opacity = 0.6
            };
            _gridLayer.Children.Add(line);
        }

        for (double y = 0; y <= height; y += step)
        {
            var line = new Line
            {
                StartPoint = new Point(0, y),
                EndPoint = new Point(width, y),
                Stroke = stroke,
                StrokeThickness = 0.5,
                Opacity = 0.6
            };
            _gridLayer.Children.Add(line);
        }
    }
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}