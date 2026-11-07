// Draws the suite's marks and writes PNG, ICO and lockup files for each app:   dotnet run --project tools/Brand -- <output folder>
// The mark is the same in every app, redrawn as geometry so it can be made in any colour at any size: a half ring open to the right, a smaller
// half ring below, and a ring at the centre, on a plate in the app's colour (docs/BRANDING.md in O-Studio). The AI agent has its own artwork:
// the ring, lit, turning through all four colours.
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;

var output = args.Length > 0 ? args[0] : Path.Combine(AppContext.BaseDirectory, "brand");
// The suite palette. The AI orb uses the four gold gradient stops.
var mirage = ColorTranslator.FromHtml("#102C80"); // Midnight (Tren)
var flame = ColorTranslator.FromHtml("#B83E16");  // Ember (Kivo)
var sea = ColorTranslator.FromHtml("#166040");    // Grove (Finch)
var sand = ColorTranslator.FromHtml("#5630A0");   // Dusk (Falcon)
var white = Color.White;
var ink = mirage;
var inkSoft = ColorTranslator.FromHtml("#3E56A0");
var glow = new[] { "#F0C840", "#D4980E", "#9B6000", "#6B4000" }.Select(ColorTranslator.FromHtml).ToArray();

var apps = new (string Folder, string Name, Color Plate, Color Glyph)[]
{
    ("studio", "Kivo", flame, white),
    ("draft", "Tren", mirage, white),
    ("form", "Finch", sea, white),
    ("coreop", "Falcon", sand, white),
};

foreach (var (folder, name, plate, glyph) in apps)
{
    var dir = Path.Combine(output, folder);
    Directory.CreateDirectory(dir);
    Mark(1024, plate, glyph).Save(Path.Combine(dir, "mark.png"), ImageFormat.Png);
    Mark(256, plate, glyph).Save(Path.Combine(dir, "mark-256.png"), ImageFormat.Png);
    Ico(Path.Combine(dir, "app.ico"), plate, glyph);
    Lockup(name, plate, glyph).Save(Path.Combine(dir, "lockup.png"), ImageFormat.Png);
    File.WriteAllText(Path.Combine(dir, "mark.svg"), Svg(name, plate, glyph));

    var tiles = Path.Combine(dir, "tiles");
    Directory.CreateDirectory(tiles);
    foreach (var (file, size) in new[] { ("Square44x44Logo.png", 44), ("Square71x71Logo.png", 71), ("Square150x150Logo.png", 150), ("Square310x310Logo.png", 310), ("StoreLogo.png", 50), ("logo.png", 150), ("logo_plate.png", 256) })
        Mark(size, plate, glyph, full: file != "logo_plate.png").Save(Path.Combine(tiles, file), ImageFormat.Png);
    Wide(310, 150, plate, glyph).Save(Path.Combine(tiles, "Wide310x150Logo.png"), ImageFormat.Png);
    Wide(620, 300, plate, glyph).Save(Path.Combine(tiles, "SplashScreen.png"), ImageFormat.Png);
}

var orbDir = Path.Combine(output, "ai");
Directory.CreateDirectory(orbDir);
foreach (var size in new[] { 1024, 256, 64 })
    Orb(size, mirage, glow).Save(Path.Combine(orbDir, $"ai-orb-{size}.png"), ImageFormat.Png);
File.WriteAllText(Path.Combine(orbDir, "ai-orb.svg"), OrbSvg(mirage, glow));
Sheet(apps, mirage, flame, sea, sand, glow).Save(Path.Combine(output, "brand-sheet.png"), ImageFormat.Png);
Console.WriteLine("Written to " + output);

// The same mark as geometry in a 194 by 194 box: a half ring open to the right (radius 69, stroke 18), a smaller half ring below
// the centre (radius 50, stroke 12), and a ring at the centre (radius 18.5, stroke 5), all white, centred at (107, 96).
static string Hex(Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";

static string Svg(string name, Color plate, Color glyph)
{
    var fill = Hex(plate);
    return $"""
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 194 194" width="194" height="194">
  <title>{name}</title>
  <rect width="194" height="194" rx="42" fill="{fill}"/>
  <g fill="none" stroke="{Hex(glyph)}">
    <path d="M107 27A69 69 0 0 0 107 165" stroke-width="18"/>
    <path d="M57 96A50 50 0 0 0 157 96" stroke-width="12"/>
    <circle cx="107" cy="96" r="18.5" stroke-width="5"/>
  </g>
</svg>
""";
}

static Bitmap NewBitmap(int w, int h) => new(w, h, PixelFormat.Format32bppArgb);

static Graphics Canvas(Bitmap bitmap)
{
    var g = Graphics.FromImage(bitmap);
    g.SmoothingMode = SmoothingMode.AntiAlias;
    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
    g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
    g.Clear(Color.Transparent);
    return g;
}

// The glyph is drawn in the coordinates of the original 256 pixel plate (plate 31..224); the transform fits the plate to the target square.
static void DrawMark(Graphics g, float x, float y, float size, Color plate, Color glyph, bool full)
{
    var state = g.Save();
    var s = size / 194f;
    g.TranslateTransform(x, y);
    g.ScaleTransform(s, s);
    g.TranslateTransform(-31, -31);
    using (var brush = new SolidBrush(plate))
    {
        if (full)
        {
            using var path = Rounded(31, 31, 194, 194, 34);
            g.FillPath(brush, path);
        }
        else
            g.FillRectangle(brush, 31, 31, 194, 194);
    }
    using var pen = new Pen(glyph) { StartCap = LineCap.Flat, EndCap = LineCap.Flat };
    pen.Width = 18;
    g.DrawArc(pen, 138 - 69, 127 - 69, 138, 138, 90, 180); // the C: a half ring open to the right
    pen.Width = 12;
    g.DrawArc(pen, 138 - 50, 127 - 50, 100, 100, 0, 180); // the smaller half ring below the centre
    pen.Width = 5;
    g.DrawEllipse(pen, 138 - 18.5f, 127 - 18.5f, 37, 37); // the ring at the centre
    g.Restore(state);
}

static GraphicsPath Rounded(float x, float y, float w, float h, float r)
{
    var path = new GraphicsPath();
    path.AddArc(x, y, 2 * r, 2 * r, 180, 90);
    path.AddArc(x + w - 2 * r, y, 2 * r, 2 * r, 270, 90);
    path.AddArc(x + w - 2 * r, y + h - 2 * r, 2 * r, 2 * r, 0, 90);
    path.AddArc(x, y + h - 2 * r, 2 * r, 2 * r, 90, 90);
    path.CloseFigure();
    return path;
}

static Bitmap Mark(int size, Color plate, Color glyph, bool full = true)
{
    var bitmap = NewBitmap(size, size);
    using var g = Canvas(bitmap);
    DrawMark(g, 0, 0, size, plate, glyph, full);
    return bitmap;
}

static Bitmap Wide(int w, int h, Color plate, Color glyph)
{
    var bitmap = NewBitmap(w, h);
    using var g = Canvas(bitmap);
    using (var brush = new SolidBrush(plate))
        g.FillRectangle(brush, 0, 0, w, h);
    var size = h * 0.62f;
    // the plate is the same colour as the background, so only the glyph shows
    DrawMark(g, (w - size) / 2, (h - size) / 2, size, plate, glyph, full: false);
    return bitmap;
}

static Bitmap Lockup(string name, Color plate, Color glyph)
{
    var bitmap = NewBitmap(1600, 640);
    using var g = Canvas(bitmap);
    DrawMark(g, 40, 40, 560, plate, glyph, full: true);
    using var title = new Font("Segoe UI Semibold", 150, FontStyle.Regular, GraphicsUnit.Pixel);
    using var line = new Font("Segoe UI", 52, FontStyle.Regular, GraphicsUnit.Pixel);
    using var small = new Font("Segoe UI Semibold", 44, FontStyle.Regular, GraphicsUnit.Pixel);
    using var inkBrush = new SolidBrush(ColorTranslator.FromHtml("#102C80"));
    using var softBrush = new SolidBrush(ColorTranslator.FromHtml("#3E56A0"));
    g.DrawString(name, title, inkBrush, 650, 150);
    g.DrawString("Open Source Architecture Suit", line, softBrush, 660, 360);
    g.DrawString("An app of Myna", small, softBrush, 660, 440);
    return bitmap;
}

// An ICO file whose images are PNGs, which Windows has read since Vista.
static void Ico(string path, Color plate, Color glyph)
{
    var sizes = new[] { 16, 24, 32, 48, 64, 128, 256 };
    var images = sizes.Select(s =>
    {
        using var bitmap = Mark(s, plate, glyph);
        using var stream = new MemoryStream();
        bitmap.Save(stream, ImageFormat.Png);
        return stream.ToArray();
    }).ToList();
    using var file = File.Create(path);
    using var w = new BinaryWriter(file);
    w.Write((ushort)0);
    w.Write((ushort)1);
    w.Write((ushort)sizes.Length);
    var offset = 6 + 16 * sizes.Length;
    for (var i = 0; i < sizes.Length; i++)
    {
        w.Write((byte)(sizes[i] == 256 ? 0 : sizes[i]));
        w.Write((byte)(sizes[i] == 256 ? 0 : sizes[i]));
        w.Write((byte)0);
        w.Write((byte)0);
        w.Write((ushort)1);
        w.Write((ushort)32);
        w.Write(images[i].Length);
        w.Write(offset);
        offset += images[i].Length;
    }
    foreach (var image in images)
        w.Write(image);
}

// The AI agent (design file AI Orb.html, kept as ai/ai-orb.html, is the animated master): a dark sphere lit by four soft lobes, one in each app colour
// (Ember upper left, Dusk upper right, Midnight lower right, Grove lower left), the suite mark in white with round ends, and four small coloured
// particles orbiting it. The still image is the first frame.
static void Lobe(Graphics g, float cx, float cy, float rx, float ry, Color c, int alpha)
{
    using var path = new GraphicsPath();
    path.AddEllipse(cx - rx, cy - ry, rx * 2, ry * 2);
    using var brush = new PathGradientBrush(path)
    {
        CenterColor = Color.FromArgb(alpha, c),
        SurroundColors = new[] { Color.FromArgb(0, c) },
    };
    g.FillPath(brush, path);
}

static Bitmap Orb(int size, Color unused, Color[] c)
{
    var bitmap = NewBitmap(size, size);
    using var g = Canvas(bitmap);
    var k = size / 194f;
    using var disc = new GraphicsPath();
    disc.AddEllipse(0, 0, size, size);
    using (var baseBrush = new PathGradientBrush(disc) { CenterColor = ColorTranslator.FromHtml("#130F28"), SurroundColors = new[] { ColorTranslator.FromHtml("#060610") } })
        g.FillPath(baseBrush, disc);
    g.SetClip(disc);
    float r = size * 0.5f;
    Lobe(g, size * 0.34f, size * 0.30f, r * 1.05f, r * 1.0f, ColorTranslator.FromHtml("#D24C1A"), 235);
    Lobe(g, size * 0.68f, size * 0.31f, r * 1.0f, r * 0.95f, ColorTranslator.FromHtml("#5A34A8"), 225);
    Lobe(g, size * 0.65f, size * 0.70f, r * 0.95f, r * 1.0f, ColorTranslator.FromHtml("#122E84"), 235);
    Lobe(g, size * 0.32f, size * 0.70f, r * 1.0f, r * 0.98f, ColorTranslator.FromHtml("#186444"), 225);
    g.ResetClip();
    // a soft glow behind the mark, then the mark; the mark is 74 % of the disc about its centre
    g.TranslateTransform(97 * k, 97 * k);
    g.ScaleTransform(0.74f, 0.74f);
    g.TranslateTransform(-96 * k, -96 * k);
    StrokeMark(g, k, Color.FromArgb(55, Color.White), 12);
    StrokeMark(g, k, Color.White, 0);
    g.ResetTransform();
    // the four particles at their start angles, just inside the rim
    var dots = new[] { ("#E05A28", 0.78f, 0f), ("#7858D0", 0.70f, 90f), ("#2858B8", 0.82f, 180f), ("#22A870", 0.74f, 270f) };
    foreach (var (hex, rad, deg) in dots)
    {
        var a = (deg - 90) * MathF.PI / 180;
        float x = size / 2 + MathF.Cos(a) * rad * size / 2, y = size / 2 + MathF.Sin(a) * rad * size / 2, d = size * 0.032f;
        using var b = new SolidBrush(ColorTranslator.FromHtml(hex));
        g.FillEllipse(b, x - d, y - d, d * 2, d * 2);
    }

    return bitmap;
}

static void StrokeMark(Graphics g, float k, Color c, float extra)
{
    float cx = 107 * k, cy = 96 * k;
    using var p1 = new Pen(c, (18 + extra) * k) { StartCap = LineCap.Round, EndCap = LineCap.Round };
    using var p2 = new Pen(c, (12 + extra) * k) { StartCap = LineCap.Round, EndCap = LineCap.Round };
    using var p3 = new Pen(c, (5 + extra) * k);
    g.DrawArc(p1, cx - 69 * k, cy - 69 * k, 138 * k, 138 * k, 90, 180);
    g.DrawArc(p2, cx - 50 * k, cy - 50 * k, 100 * k, 100 * k, 0, 180);
    g.DrawEllipse(p3, cx - 18.5f * k, cy - 18.5f * k, 37 * k, 37 * k);
}

static string OrbSvg(Color unused, Color[] c)
{
    string Lobe(string id, string col, double cx, double cy) => $"""<radialGradient id="{id}" cx="{cx}" cy="{cy}" r="0.55"><stop offset="0" stop-color="{col}" stop-opacity="0.92"/><stop offset="1" stop-color="{col}" stop-opacity="0"/></radialGradient>""";
    string Dot(string col, int r, int dur, bool ccw) => $"""<g><animateTransform attributeName="transform" type="rotate" from="0 97 97" to="{(ccw ? -360 : 360)} 97 97" dur="{dur}s" repeatCount="indefinite"/><circle cx="97" cy="{97 - r}" r="3.2" fill="{col}"/></g>""";
    return $"""
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 194 194" width="194" height="194" role="img" aria-label="AI agent">
  <title>AI agent</title>
  <defs>
    <radialGradient id="base" cx="0.5" cy="0.5" r="0.5"><stop offset="0" stop-color="#130F28"/><stop offset="1" stop-color="#060610"/></radialGradient>
    {Lobe("ember", "#D24C1A", 0.34, 0.30)}
    {Lobe("dusk", "#5A34A8", 0.68, 0.31)}
    {Lobe("midnight", "#122E84", 0.65, 0.70)}
    {Lobe("grove", "#186444", 0.32, 0.70)}
    <clipPath id="disc"><circle cx="97" cy="97" r="97"/></clipPath>
  </defs>
  <circle cx="97" cy="97" r="97" fill="url(#base)"/>
  <g clip-path="url(#disc)">
    <rect width="194" height="194" fill="url(#ember)"/><rect width="194" height="194" fill="url(#dusk)"/>
    <rect width="194" height="194" fill="url(#midnight)"/><rect width="194" height="194" fill="url(#grove)"/>
  </g>
  <g fill="none" stroke="#FFFFFF" stroke-linecap="round" transform="translate(97 97) scale(0.74) translate(-96 -96)">
    <path d="M107 27A69 69 0 0 0 107 165" stroke-width="18"/><path d="M57 96A50 50 0 0 0 157 96" stroke-width="12"/><circle cx="107" cy="96" r="18.5" stroke-width="5"/>
  </g>
  {Dot("#E05A28", 76, 11, false)}{Dot("#7858D0", 68, 16, true)}{Dot("#2858B8", 80, 20, false)}{Dot("#22A870", 72, 26, true)}
</svg>
""";
}

// The brand sheet: the palette, the mark in each app, the AI agent.
static Bitmap Sheet((string Folder, string Name, Color Plate, Color Glyph)[] apps, Color mirage, Color flame, Color sea, Color sand, Color[] glow)
{
    var bitmap = NewBitmap(1600, 1060);
    using var g = Canvas(bitmap);
    g.Clear(Color.White);
    using var title = new Font("Segoe UI Semibold", 40, FontStyle.Regular, GraphicsUnit.Pixel);
    using var head = new Font("Segoe UI Semibold", 22, FontStyle.Regular, GraphicsUnit.Pixel);
    using var text = new Font("Segoe UI", 15, FontStyle.Regular, GraphicsUnit.Pixel);
    using var mono = new Font("Consolas", 13, FontStyle.Regular, GraphicsUnit.Pixel);
    using var inkBrush = new SolidBrush(mirage);
    using var dimBrush = new SolidBrush(ColorTranslator.FromHtml("#5C6B73"));
    g.DrawString("Myna brand", title, inkBrush, 60, 36);
    g.DrawString("One mark. Four colours. Each app wears one; the AI agent wears all four.", text, dimBrush, 64, 112);
    var swatches = new (string Name, string App, Color Colour, Color On)[]
    {
        ("Midnight", "Tren", mirage, Color.White),
        ("Ember", "Kivo", flame, Color.White),
        ("Grove", "Finch", sea, Color.White),
        ("Dusk", "Falcon", sand, Color.White),
    };
    for (var i = 0; i < swatches.Length; i++)
    {
        var (name, app, colour, on) = swatches[i];
        float x = 60 + i * 380, y = 160;
        using (var path = Rounded(x, y, 350, 230, 18))
        using (var fill = new SolidBrush(colour))
        {
            g.FillPath(fill, path);
        }

        using var onBrush = new SolidBrush(on);
        g.DrawString(app, head, onBrush, x + 22, y + 20);
        g.DrawString(name, text, onBrush, x + 22, y + 150);
        g.DrawString($"HEX {Hex(colour)}    RGB {colour.R}, {colour.G}, {colour.B}", mono, onBrush, x + 22, y + 184);
    }

    g.DrawString("The mark in each app", head, inkBrush, 60, 430);
    for (var i = 0; i < apps.Length; i++)
    {
        var (_, name, plate, glyph) = apps[i];
        using var mark = Mark(256, plate, glyph);
        g.DrawImage(mark, 60 + i * 380, 485, 200, 200);
        g.DrawString(name, head, inkBrush, 60 + i * 380, 700);
    }

    g.DrawString("The AI agent", head, inkBrush, 60, 780);
    using (var orb = Orb(256, mirage, glow))
        g.DrawImage(orb, 60, 830, 190, 190);
    g.DrawString("A dark sphere lit by four lobes, one in each app colour, the suite mark in white, and four coloured\nparticles orbiting. ai/ai-orb.html is the animated master (design file); ai-orb.svg and the PNGs are its first frame.", text, dimBrush, 280, 860);
    g.DrawString("Type: app names in Apfel Grotezk, display in Mazius Display (both SIL OFL, bundled in O.Ui). This sheet is drawn in Segoe UI.", mono, dimBrush, 60, 1030);
    return bitmap;
}
