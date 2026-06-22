using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace AvaloniaTestDemo.Views;

public partial class LineRenderView : UserControl
{
    private Point? _start;
    private Point? _current;

    public LineRenderView()
    {
        InitializeComponent();
    }
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        _start = e.GetPosition(this);
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        if (_start != null)
        {
            _current = e.GetPosition(this);
            InvalidateVisual();
        }
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (_start != null && _current != null)
        {
            context.DrawLine(
                new Pen(Brushes.Red, 2),
                _start.Value,
                _current.Value);
        }
    }
}