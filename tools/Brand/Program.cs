// Draws the suite's marks and writes PNG, ICO and lockup files for each app:   dotnet run --project tools/Brand -- <output folder>
// The mark is the same in every app, redrawn as geometry so it can be made in any colour at any size: a half ring open to the right, a smaller
// half ring below, and a ring at the centre, on a plate in the app's colour (docs/BRANDING.md in O-Studio). The AI agent has its own artwork:
// the ring, lit, turning through all four colours.
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;

var output = args.Length > 0 ? args[0] : Path.Combine(AppContext.BaseDirectory, "brand");
// The suite palette. The glow tones are the four colours as they read on a Mirage disc, for the AI orb.
var mirage = ColorTranslator.FromHtml("#16232B"); // Dark Slate Blue
var flame = ColorTranslator.FromHtml("#FF5B04");  // Flame Orange
var sea = ColorTranslator.FromHtml("#075056");    // Deep Sea Green
var sand = ColorTranslator.FromHtml("#E4EEF0");   // Wild Sand
var white = Color.White;
var ink = mirage;
var inkSoft = ColorTranslator.FromHtml("#4A5A63");
var glow = new[] { "#FF5B04", "#E4EEF0", "#1496A0", "#4F7C93" }.Select(ColorTranslator.FromHtml).ToArray();

var apps = new (string Folder, string Name, Color Plate, Color Glyph)[]
{
    ("studio", "O-Studio", flame, white),
    ("draft", "O-Draft", mirage, white),
    ("form", "O-Form", sea, white),
    ("coreop", "O-CoreOp", sand, mirage),
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
    using var inkBrush = new SolidBrush(ColorTranslator.FromHtml("#16232B"));
    using var softBrush = new SolidBrush(ColorTranslator.FromHtml("#4A5A63"));
    g.DrawString(name, title, inkBrush, 650, 150);
    g.DrawString("Open Source Architecture Suit", line, softBrush, 660, 360);
    g.DrawString("An app of the O-Studio suite", small, softBrush, 660, 440);
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

// The AI agent: the logo itself on a round Mirage tile, its three circles (the big C, the bowl, the ring) each in a different colour of the suite. Still,
// the circles are Flame Orange, Wild Sand and Deep Sea Green on the Mirage disc, so all four colours are in it; in the app they pass the colours on, two
// seconds a step. ai-orb.svg is the animated master (every circle wears all four colours in eight seconds, always three different ones at a time).
static void StrokeMark(Graphics g, float k, Color c1, Color c2, Color c3, float extra)
{
    float cx = 107 * k, cy = 96 * k;
    using var p1 = new Pen(c1, (18 + extra) * k) { StartCap = LineCap.Flat, EndCap = LineCap.Flat };
    using var p2 = new Pen(c2, (12 + extra) * k) { StartCap = LineCap.Flat, EndCap = LineCap.Flat };
    using var p3 = new Pen(c3, (5 + extra) * k);
    g.DrawArc(p1, cx - 69 * k, cy - 69 * k, 138 * k, 138 * k, 90, 180);
    g.DrawArc(p2, cx - 50 * k, cy - 50 * k, 100 * k, 100 * k, 0, 180);
    g.DrawEllipse(p3, cx - 18.5f * k, cy - 18.5f * k, 37 * k, 37 * k);
}

static Bitmap Orb(int size, Color disc, Color[] glow)
{
    var bitmap = NewBitmap(size, size);
    using var g = Canvas(bitmap);
    var k = size / 194f;
    using (var brush = new SolidBrush(disc))
        g.FillEllipse(brush, 0, 0, size, size);
    // the mark fills a little less than the disc: scaled to 74 % about its centre (96, 96)
    g.TranslateTransform(97 * k, 97 * k);
    g.ScaleTransform(0.74f, 0.74f);
    g.TranslateTransform(-96 * k, -96 * k);
    StrokeMark(g, k, Color.FromArgb(70, glow[0]), Color.FromArgb(70, glow[1]), Color.FromArgb(70, glow[2]), 16);
    StrokeMark(g, k, glow[0], glow[1], glow[2], 0);
    return bitmap;
}

static string OrbSvg(Color disc, Color[] glow)
{
    string Cycle(int offset) => string.Join(";", Enumerable.Range(0, 5).Select(i => Hex(glow[(offset + i) % 4])));
    string Anim(int offset) => $"<animate attributeName=\"stroke\" values=\"{Cycle(offset)}\" dur=\"8s\" repeatCount=\"indefinite\"/>";
    string Circles(double extra, int dim) => $"""
      <path d="M107 27A69 69 0 0 0 107 165" stroke="{Hex(glow[0])}" stroke-width="{18 + extra}">{Anim(0)}</path>
      <path d="M57 96A50 50 0 0 0 157 96" stroke="{Hex(glow[1])}" stroke-width="{12 + extra}">{Anim(1)}</path>
      <circle cx="107" cy="96" r="18.5" stroke="{Hex(glow[2])}" stroke-width="{5 + extra}">{Anim(2)}</circle>
""";
    return $"""
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 194 194" width="194" height="194" role="img" aria-label="AI agent">
  <title>AI agent</title>
  <defs><filter id="glow" x="-50%" y="-50%" width="200%" height="200%"><feGaussianBlur stdDeviation="6"/></filter></defs>
  <circle cx="97" cy="97" r="97" fill="{Hex(disc)}"/>
  <g fill="none" stroke-linecap="butt" transform="translate(97 97) scale(0.74) translate(-96 -96)">
    <g filter="url(#glow)" opacity="0.6">
      <animate attributeName="opacity" values="0.35;0.85;0.35" dur="4s" repeatCount="indefinite"/>
{Circles(16, 1)}    </g>
    <g>
{Circles(0, 0)}    </g>
  </g>
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
    g.DrawString("O-Studio suite brand", title, inkBrush, 60, 36);
    g.DrawString("One mark. Four colours. Each app wears one; the AI agent wears all four.", text, dimBrush, 64, 112);
    var swatches = new (string Name, string App, Color Colour, Color On)[]
    {
        ("Dark Slate Blue (Mirage)", "O-Draft", mirage, Color.White),
        ("Flame Orange", "O-Studio", flame, Color.White),
        ("Deep Sea Green", "O-Form", sea, Color.White),
        ("Wild Sand", "O-CoreOp", sand, mirage),
    };
    for (var i = 0; i < swatches.Length; i++)
    {
        var (name, app, colour, on) = swatches[i];
        float x = 60 + i * 380, y = 160;
        using (var path = Rounded(x, y, 350, 230, 18))
        using (var fill = new SolidBrush(colour))
        {
            g.FillPath(fill, path);
            if (colour == sand)
            {
                using var edge = new Pen(ColorTranslator.FromHtml("#C9D8DC"));
                g.DrawPath(edge, path);
            }
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
    g.DrawString("The ring of the mark, lit. It turns through Flame Orange, Wild Sand, Deep Sea Green and Mirage\n(as glow tones on a Mirage disc) while the agent runs; grey and still when it does not.\nai/ai-orb.svg is the animated master.", text, dimBrush, 280, 860);
    g.DrawString("Type: app names in Studio Grotesk, display in Mazius Display. This sheet uses Segoe UI until the fonts are installed.", mono, dimBrush, 60, 1030);
    return bitmap;
}
