using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Input;

namespace AvaloniaTestDemo.Models;

public class DraggableEllipse : Ellipse
{
    private Point _start;
    private bool _dragging;

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        _dragging = true;
        _start = e.GetPosition(Parent as Visual);

        e.Pointer.Capture(this);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        _dragging = false;
        e.Pointer.Capture(null);
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        if (!_dragging)
            return;

        var pos = e.GetPosition(Parent as Visual);

        var dx = pos.X - _start.X;
        var dy = pos.Y - _start.Y;

        _start = pos;

        var vm = DataContext as NodeViewModel;

        if (vm == null)
            return;

        vm.X += dx;
        vm.Y += dy;
    }
}