using System;
using Avalonia;
using Avalonia.Media;

namespace UI_Example.Control;

public class CircularProgressBar : Avalonia.Controls.Control
{
    public static readonly StyledProperty<double> ValueProperty =
        AvaloniaProperty.Register<CircularProgressBar, double>(
            nameof(Value),
            0);

    public double Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    static CircularProgressBar()
    {
        ValueProperty.Changed.AddClassHandler<CircularProgressBar>(
            (x, _) => x.InvalidateVisual());
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        double width = Bounds.Width;
        double height = Bounds.Height;

        double radius = Math.Min(width, height) / 2 - 10;

        var center = new Point(width / 2, height / 2);

        //------------------------------------
        // 背景圆环
        //------------------------------------

        var backgroundPen = new Pen(
            Brushes.Gray,
            8);

        context.DrawEllipse(
            null,
            backgroundPen,
            center,
            radius,
            radius);

        //------------------------------------
        // 进度圆弧
        //------------------------------------

        double angle = Value / 100 * 360;

        DrawArc(
            context,
            center,
            radius,
            -90,
            angle,
            Brushes.DeepSkyBlue);

        //------------------------------------
        // 百分比文字
        //------------------------------------

        var text = new FormattedText(
            $"{Value:0}%",
            System.Globalization.CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            Typeface.Default,
            20,
            Brushes.DodgerBlue);

        context.DrawText(
            text,
            new Point(
                center.X - text.Width / 2,
                center.Y - text.Height / 2));
    }

    private void DrawArc(
        DrawingContext context,
        Point center,
        double radius,
        double startAngle,
        double sweepAngle,
        IBrush brush)
    {
        if (sweepAngle <= 0)
            return;

        var geometry = new StreamGeometry();

        using (var g = geometry.Open())
        {
            double startRad = Math.PI * startAngle / 180;
            double endRad = Math.PI * (startAngle + sweepAngle) / 180;

            var start = new Point(
                center.X + radius * Math.Cos(startRad),
                center.Y + radius * Math.Sin(startRad));

            var end = new Point(
                center.X + radius * Math.Cos(endRad),
                center.Y + radius * Math.Sin(endRad));

            g.BeginFigure(start);

            g.ArcTo(
                end,
                new Size(radius, radius),
                0,
                sweepAngle > 180,
                SweepDirection.Clockwise);
        }

        context.DrawGeometry(
            null,
            new Pen(brush, 8),
            geometry);
    }
}