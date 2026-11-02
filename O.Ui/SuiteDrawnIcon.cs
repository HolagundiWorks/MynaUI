using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Windows.Foundation;
using Windows.UI;

namespace O.Ui;

/// <summary>
/// The line-drawn marks of the status row, the same drawings O-Draft uses in its bottom bar (24 x 24, stroke 1.5, shown at 16):
/// <c>sun</c> (light theme is on), <c>moon</c> (dark theme is on) and <c>agent</c> (the AI assistant: three circles).
/// The ink follows the suite theme.
/// </summary>
public sealed class SuiteDrawnIcon : ContentControl
{
    public const double DisplaySize = 16;
    const double Stroke = 1.5;

    public static readonly DependencyProperty KindProperty = DependencyProperty.Register(
        nameof(Kind), typeof(string), typeof(SuiteDrawnIcon), new PropertyMetadata("", (d, _) => ((SuiteDrawnIcon)d).Redraw()));

    public SuiteDrawnIcon()
    {
        Width = DisplaySize;
        Height = DisplaySize;
        IsTabStop = false;
        IsHitTestVisible = false;
        HorizontalContentAlignment = HorizontalAlignment.Center;
        VerticalContentAlignment = VerticalAlignment.Center;
        Loaded += (_, _) =>
        {
            SuiteAppearance.Changed += OnTheme;
            Redraw();
        };
        Unloaded += (_, _) => SuiteAppearance.Changed -= OnTheme;
    }

    public string Kind
    {
        get => (string)GetValue(KindProperty);
        set => SetValue(KindProperty, value);
    }

    void OnTheme(ElementTheme _) => DispatcherQueue.TryEnqueue(Redraw);

    void Redraw() => Content = string.IsNullOrEmpty(Kind) ? null : Draw(Kind);

    static UIElement Draw(string kind)
    {
        var ink = SuiteAppearance.TryBrush("TextPrimaryBrush") ?? new SolidColorBrush(Color.FromArgb(255, 0x10, 0x2C, 0x80));
        var canvas = new Canvas { Width = 24, Height = 24 };
        switch (kind)
        {
            case "sun":
                var gold = new SolidColorBrush(Color.FromArgb(255, 230, 180, 60));
                Disc(canvas, 12, 12, 4, gold);
                for (var i = 0; i < 8; i++)
                {
                    var a = i * 45.0 * Math.PI / 180.0;
                    Line(canvas, 12 + 6 * Math.Cos(a), 12 + 6 * Math.Sin(a), 12 + 9 * Math.Cos(a), 12 + 9 * Math.Sin(a), gold);
                }

                break;
            case "moon":
                Arc(canvas, 11, 12, 7, 70, 290, ink);
                Arc(canvas, 14, 12, 5.5, 100, 260, ink);
                break;
            case "agent":
                foreach (var r in new[] { 3.25, 6.5, 9.75 })
                    Ring(canvas, 12, 12, r, ink);
                break;
        }

        return new Viewbox { Width = DisplaySize, Height = DisplaySize, Child = canvas, Stretch = Stretch.Uniform };
    }

    static void Disc(Canvas canvas, double x, double y, double r, Brush fill)
    {
        var e = new Ellipse { Width = r * 2, Height = r * 2, Fill = fill };
        Canvas.SetLeft(e, x - r);
        Canvas.SetTop(e, y - r);
        canvas.Children.Add(e);
    }

    static void Ring(Canvas canvas, double x, double y, double r, Brush stroke)
    {
        var e = new Ellipse { Width = r * 2, Height = r * 2, Stroke = stroke, StrokeThickness = Stroke };
        Canvas.SetLeft(e, x - r);
        Canvas.SetTop(e, y - r);
        canvas.Children.Add(e);
    }

    static void Line(Canvas canvas, double x1, double y1, double x2, double y2, Brush stroke) =>
        canvas.Children.Add(new Line { X1 = x1, Y1 = y1, X2 = x2, Y2 = y2, Stroke = stroke, StrokeThickness = Stroke, StrokeStartLineCap = PenLineCap.Round, StrokeEndLineCap = PenLineCap.Round });

    static void Arc(Canvas canvas, double x, double y, double r, double start, double end, Brush stroke)
    {
        var steps = Math.Max(2, (int)Math.Ceiling(Math.Abs(end - start) / 10.0));
        var points = new PointCollection();
        for (var i = 0; i <= steps; i++)
        {
            var a = (start + (end - start) * i / steps) * Math.PI / 180.0;
            points.Add(new Point(x + r * Math.Cos(a), y - r * Math.Sin(a)));
        }

        canvas.Children.Add(new Polyline
        {
            Points = points, Stroke = stroke, StrokeThickness = Stroke, StrokeLineJoin = PenLineJoin.Round,
            StrokeStartLineCap = PenLineCap.Round, StrokeEndLineCap = PenLineCap.Round,
        });
    }
}
