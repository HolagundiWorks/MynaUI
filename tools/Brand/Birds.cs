// Each app's mark is its own bird, drawn in a 194 box on the app's plate.
// The plate is that app's brand colour. The bird is white, with a pale wash for the second tone,
// so the same shapes stay readable at 16 px. Eyes are holes in the plate colour.
using System.Drawing;
using System.Drawing.Drawing2D;

static class Birds
{
    public static void Paint(Graphics g, string name, Color plate)
    {
        var white = Color.White;
        var wash = Mix(plate, Color.White, 0.78f);
        switch (name)
        {
            case "Tren":
                Tren(g, plate, white);
                break;
            case "Falcon":
                Falcon(g, plate, white);
                break;
            case "Finch":
                Finch(g, plate, white, wash);
                break;
            case "Kivo":
                Kivo(g, plate, white, wash);
                break;
            case "Myna":
                Myna(g, plate, white, wash);
                break;
            default:
                Tren(g, plate, white);
                break;
        }
    }

    public static string Svg(string name, Color plate)
    {
        var wash = Hex(Mix(plate, Color.White, 0.78f));
        var fill = Hex(plate);
        var bird = name switch
        {
            "Falcon" => FalconSvg(fill),
            "Finch" => FinchSvg(fill, wash),
            "Kivo" => KivoSvg(fill, wash),
            "Myna" => MynaSvg(fill, wash),
            _ => TrenSvg(fill),
        };
        return $"""
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 194 194" width="194" height="194">
  <title>{name}</title>
  <rect width="194" height="194" rx="42" fill="{fill}"/>
  {bird}
</svg>
""";
    }

    static void Tren(Graphics g, Color plate, Color white)
    {
        Fill(g, white, p =>
        {
            p.AddBezier(16, 118, 12, 52, 70, 22, 124, 42);
            p.AddBezier(124, 42, 158, 54, 172, 78, 154, 100);
            p.AddBezier(154, 100, 126, 76, 80, 80, 30, 128);
            p.AddBezier(30, 128, 16, 138, 12, 128, 16, 118);
        });
        Fill(g, white, p => p.AddPolygon(new[]
        {
            new PointF(148, 82), new PointF(186, 104), new PointF(146, 120),
        }));
        Hole(g, plate, 132, 64, 12, 12);
    }

    static void Falcon(Graphics g, Color plate, Color white)
    {
        Fill(g, white, p =>
        {
            p.AddBezier(26, 152, 18, 72, 62, 22, 118, 38);
            p.AddBezier(118, 38, 152, 48, 166, 78, 150, 104);
            p.AddLine(150, 104, 188, 118);
            p.AddLine(188, 118, 162, 126);
            p.AddLine(162, 126, 148, 152);
            p.AddLine(148, 152, 132, 124);
            p.AddBezier(132, 124, 92, 158, 42, 170, 26, 152);
        });
        Hole(g, plate, 112, 68, 18, 16);
        Fill(g, white, p => p.AddEllipse(116, 71, 5, 5));
    }

    static void Finch(Graphics g, Color plate, Color white, Color wash)
    {
        Fill(g, wash, p =>
        {
            p.AddBezier(22, 132, 30, 108, 78, 102, 118, 118);
            p.AddBezier(118, 118, 100, 158, 46, 164, 24, 146);
            p.AddBezier(24, 146, 16, 140, 16, 136, 22, 132);
        });
        Fill(g, white, p =>
        {
            p.AddBezier(16, 112, 22, 42, 86, 18, 136, 46);
            p.AddBezier(136, 46, 162, 60, 156, 92, 128, 104);
            p.AddBezier(128, 104, 78, 78, 36, 98, 16, 112);
        });
        Fill(g, white, p => p.AddPolygon(new[]
        {
            new PointF(126, 92), new PointF(172, 110), new PointF(124, 122),
        }));
        Hole(g, plate, 114, 72, 14, 14);
    }

    static void Kivo(Graphics g, Color plate, Color white, Color wash)
    {
        Fill(g, wash, p =>
        {
            p.AddBezier(50, 98, 90, 92, 132, 102, 152, 116);
            p.AddBezier(152, 116, 140, 156, 72, 162, 40, 140);
            p.AddBezier(40, 140, 30, 126, 34, 106, 50, 98);
        });
        Fill(g, white, p =>
        {
            p.AddBezier(22, 100, 26, 42, 80, 22, 122, 44);
            p.AddBezier(122, 44, 150, 58, 156, 80, 140, 94);
            p.AddBezier(140, 94, 110, 72, 66, 74, 28, 108);
            p.AddBezier(28, 108, 16, 116, 16, 108, 22, 100);
        });
        Fill(g, white, p => p.AddPolygon(new[]
        {
            new PointF(134, 76), new PointF(188, 98), new PointF(134, 114),
        }));
        Hole(g, plate, 116, 58, 12, 12);
        Fill(g, white, p => p.AddEllipse(119, 61, 4, 4));
    }

    static void Myna(Graphics g, Color plate, Color white, Color wash)
    {
        Fill(g, wash, p =>
        {
            p.AddBezier(20, 118, 16, 96, 48, 88, 92, 96);
            p.AddBezier(92, 96, 132, 104, 140, 124, 124, 140);
            p.AddBezier(124, 140, 96, 172, 40, 170, 22, 146);
            p.AddBezier(22, 146, 12, 134, 14, 124, 20, 118);
        });
        Fill(g, white, p =>
        {
            p.AddBezier(30, 108, 40, 48, 96, 28, 138, 56);
            p.AddBezier(138, 56, 158, 70, 154, 98, 128, 108);
            p.AddBezier(128, 108, 88, 90, 50, 96, 30, 108);
        });
        Fill(g, wash, p =>
        {
            p.AddBezier(58, 88, 96, 70, 136, 78, 150, 100);
            p.AddBezier(150, 100, 132, 116, 90, 116, 58, 104);
            p.AddBezier(58, 104, 48, 98, 48, 92, 58, 88);
        });
        Fill(g, white, p => p.AddPolygon(new[]
        {
            new PointF(140, 88), new PointF(186, 106), new PointF(138, 116),
        }));
        Hole(g, plate, 112, 82, 14, 14);
    }

    static string TrenSvg(string plate) => $"""
<g fill="#FFFFFF">
    <path d="M16 118C12 52 70 22 124 42C158 54 172 78 154 100C126 76 80 80 30 128C16 138 12 128 16 118Z"/>
    <path d="M148 82L186 104L146 120Z"/>
  </g>
  <ellipse cx="138" cy="70" rx="6" ry="6" fill="{plate}"/>
""";

    static string FalconSvg(string plate) => $"""
<g fill="#FFFFFF">
    <path d="M26 152C18 72 62 22 118 38C152 48 166 78 150 104L188 118L162 126L148 152L132 124C92 158 42 170 26 152Z"/>
  </g>
  <ellipse cx="121" cy="76" rx="9" ry="8" fill="{plate}"/>
  <ellipse cx="118.5" cy="73.5" rx="2.5" ry="2.5" fill="#FFFFFF"/>
""";

    static string FinchSvg(string plate, string wash) => $"""
<path fill="{wash}" d="M22 132C30 108 78 102 118 118C100 158 46 164 24 146C16 140 16 136 22 132Z"/>
  <g fill="#FFFFFF">
    <path d="M16 112C22 42 86 18 136 46C162 60 156 92 128 104C78 78 36 98 16 112Z"/>
    <path d="M126 92L172 110L124 122Z"/>
  </g>
  <ellipse cx="121" cy="79" rx="7" ry="7" fill="{plate}"/>
""";

    static string KivoSvg(string plate, string wash) => $"""
<path fill="{wash}" d="M50 98C90 92 132 102 152 116C140 156 72 162 40 140C30 126 34 106 50 98Z"/>
  <g fill="#FFFFFF">
    <path d="M22 100C26 42 80 22 122 44C150 58 156 80 140 94C110 72 66 74 28 108C16 116 16 108 22 100Z"/>
    <path d="M134 76L188 98L134 114Z"/>
  </g>
  <ellipse cx="122" cy="64" rx="6" ry="6" fill="{plate}"/>
  <ellipse cx="121" cy="63" rx="2" ry="2" fill="#FFFFFF"/>
""";

    static string MynaSvg(string plate, string wash) => $"""
<path fill="{wash}" d="M20 118C16 96 48 88 92 96C132 104 140 124 124 140C96 172 40 170 22 146C12 134 14 124 20 118Z"/>
  <path fill="#FFFFFF" d="M30 108C40 48 96 28 138 56C158 70 154 98 128 108C88 90 50 96 30 108Z"/>
  <path fill="{wash}" d="M58 88C96 70 136 78 150 100C132 116 90 116 58 104C48 98 48 92 58 88Z"/>
  <path fill="#FFFFFF" d="M140 88L186 106L138 116Z"/>
  <ellipse cx="119" cy="89" rx="7" ry="7" fill="{plate}"/>
""";

    static void Fill(Graphics g, Color color, Action<GraphicsPath> build)
    {
        using var path = new GraphicsPath();
        build(path);
        path.CloseFigure();
        using var brush = new SolidBrush(color);
        g.FillPath(brush, path);
    }

    static void Hole(Graphics g, Color plate, float x, float y, float w, float h)
    {
        using var brush = new SolidBrush(plate);
        g.FillEllipse(brush, x, y, w, h);
    }

    static Color Mix(Color a, Color b, float t)
    {
        static int C(byte x, byte y, float k) => (int)(x + (y - x) * k);
        return Color.FromArgb(C(a.R, b.R, t), C(a.G, b.G, t), C(a.B, b.B, t));
    }

    static string Hex(Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";
}
