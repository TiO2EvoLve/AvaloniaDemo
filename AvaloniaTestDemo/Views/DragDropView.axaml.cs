using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace AvaloniaTestDemo.Views;

public partial class DragDropView : UserControl
{
   private const double DragThreshold = 4;//拖动像素阈值
   private double GridSize = 20;
   private const double SnapDistance = 8;
   
    private bool _isDragging;
    private bool _dragStarted;

    private Point _startPointer;

    private double _startLeft;
    private double _startTop;

    private Border? _border;
    private Canvas? _canvas;
    
    public DragDropView()
    {
        InitializeComponent();

        _canvas = this.FindControl<Canvas>("RootCanvas");
        _border = this.FindControl<Border>("DraggableBorder");

        Loaded += OnLoaded;
    }
    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        GridSize = _canvas.Bounds.Height / 50; // 根据Canvas高度动态设置网格大小
    }

    private void Border_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (_border == null || _canvas == null)
            return;

        if (!e.GetCurrentPoint(_border)
              .Properties.IsLeftButtonPressed)
            return;

        _startPointer = e.GetPosition(_canvas);

        _startLeft = Canvas.GetLeft(_border);
        _startTop = Canvas.GetTop(_border);

        if (double.IsNaN(_startLeft))
            _startLeft = 0;

        if (double.IsNaN(_startTop))
            _startTop = 0;

        _isDragging = true;
        _dragStarted = false;

        e.Pointer.Capture(_border);

        e.Handled = true;
    }

    private void Border_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_isDragging || _border == null || _canvas == null)
            return;

        var pos = e.GetPosition(_canvas);

        var dx = pos.X - _startPointer.X;
        var dy = pos.Y - _startPointer.Y;

        if (!_dragStarted)
        {
            if (Math.Abs(dx) < DragThreshold &&
                Math.Abs(dy) < DragThreshold)
            {
                return;
            }

            _dragStarted = true;
        }

        var newLeft = _startLeft + dx;
        var newTop = _startTop + dy;

        // 限制在Canvas范围内
        newLeft = Math.Clamp(
            newLeft,
            0,
            Math.Max(0, _canvas.Bounds.Width - _border.Bounds.Width));

        newTop = Math.Clamp(
            newTop,
            0,
            Math.Max(0, _canvas.Bounds.Height - _border.Bounds.Height));

        newLeft = Snap(newLeft);
        newTop = Snap(newTop);

        Canvas.SetLeft(_border, newLeft);
        Canvas.SetTop(_border, newTop);

        e.Handled = true;
    }

    private void Border_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _isDragging = false;
        _dragStarted = false;

        e.Pointer.Capture(null);

        e.Handled = true;
    }

    private void Border_PointerCaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        _isDragging = false;
        _dragStarted = false;
    }

    private double Snap(double value)
    {
        var target =
            Math.Round(value / GridSize) * GridSize;

        return Math.Abs(target - value) <= SnapDistance ? target : value;
    }
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}