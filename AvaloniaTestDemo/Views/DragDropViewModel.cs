using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Material.Icons;

namespace AvaloniaTestDemo.Views;

public partial class DragDropViewModel : DemoPageBase
{
    private const double DragThreshold = 4; // 像素阈值

    [ObservableProperty]
    private double gridSize = 20;

    [ObservableProperty]
    private double snapDistance = 8;

    [ObservableProperty]
    private bool showGrid = true;

    [ObservableProperty]
    private bool isSnapEnabled = true;

    private bool _isDragging;
    private bool _dragStarted;
    private Point _startPointer;
    private double _startLeft;
    private double _startTop;

    public DragDropViewModel() : base("拖动", MaterialIconKind.Drag, int.MinValue)
    {
    }

    // Called when view is loaded so viewmodel can compute grid size from container
    public void OnLoaded(Control? canvas)
    {
        if (canvas == null) return;
        if (canvas.Bounds.Height > 0)
            GridSize = canvas.Bounds.Height / 50;
    }

    // When the SnapDistance (slider) changes, update GridSize so the visual grid follows the slider.
    partial void OnSnapDistanceChanged(double value)
    {
        if (value <= 0) value = 1;
        GridSize = value;
    }

    public void PointerPressed(Border? border, Canvas? canvas, PointerPressedEventArgs e)
    {
        if (border == null || canvas == null) return;

        if (!e.GetCurrentPoint(border).Properties.IsLeftButtonPressed)
            return;

        _startPointer = e.GetPosition(canvas);

        _startLeft = Canvas.GetLeft(border);
        _startTop = Canvas.GetTop(border);

        if (double.IsNaN(_startLeft)) _startLeft = 0;
        if (double.IsNaN(_startTop)) _startTop = 0;

        _isDragging = true;
        _dragStarted = false;

        // Capture pointer to ensure we continue receiving events
        e.Pointer.Capture(border);
        e.Handled = true;
    }

    public void PointerMoved(Border? border, Canvas? canvas, PointerEventArgs e)
    {
        if (!_isDragging || border == null || canvas == null) return;

        var pos = e.GetPosition(canvas);

        var dx = pos.X - _startPointer.X;
        var dy = pos.Y - _startPointer.Y;

        if (!_dragStarted)
        {
            if (Math.Abs(dx) < DragThreshold && Math.Abs(dy) < DragThreshold)
            {
                return;
            }
            _dragStarted = true;
        }

        var newLeft = _startLeft + dx;
        var newTop = _startTop + dy;

        // 限制在 Canvas 范围内
        newLeft = Math.Clamp(newLeft, 0, Math.Max(0, canvas.Bounds.Width - border.Bounds.Width));
        newTop = Math.Clamp(newTop, 0, Math.Max(0, canvas.Bounds.Height - border.Bounds.Height));

        if (IsSnapEnabled)
        {
            newLeft = Snap(newLeft);
            newTop = Snap(newTop);
        }

        Canvas.SetLeft(border, newLeft);
        Canvas.SetTop(border, newTop);

        e.Handled = true;
    }

    public void PointerReleased(PointerReleasedEventArgs e)
    {
        _isDragging = false;
        _dragStarted = false;
        e.Pointer.Capture(null);
        e.Handled = true;
    }

    public void PointerCaptureLost()
    {
        _isDragging = false;
        _dragStarted = false;
    }

    private double Snap(double value)
    {
        var target = Math.Round(value / GridSize) * GridSize;
        return Math.Abs(target - value) <= SnapDistance ? target : value;
    }
}