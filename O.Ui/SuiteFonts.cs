using Microsoft.UI.Xaml.Media;

namespace O.Ui;

/// <summary>
/// The suite's two typefaces (docs/BRANDING.md in O-Studio): the app names are set in <b>Apfel Grotezk</b> (free, OFL; stands in for the commercial Studio Grotesk), everything shown large (page and
/// section titles, big numbers) in <b>Mazius Display</b>. Both fonts are bundled with O.Ui. When the font files are in
/// <c>Assets\Fonts</c> beside the app (<c>ApfelGrotezk*.otf</c>/<c>.ttf</c>, <c>MaziusDisplay*.otf</c>/<c>.ttf</c>) they are used from there; otherwise the
/// installed font of that name is used; otherwise the fallback (Segoe UI Variable Display, Sitka Display), so an app always starts.
/// </summary>
public static class SuiteFonts
{
    public const string NamesFamily = "Apfel Grotezk";
    public const string DisplayFamily = "Mazius Display";

    /// <summary>For the names of the apps and the suite.</summary>
    public static FontFamily Names { get; } = Build(NamesFamily, "ApfelGrotezk", "Segoe UI Variable Display, Segoe UI");

    /// <summary>For large display text.</summary>
    public static FontFamily Display { get; } = Build(DisplayFamily, "MaziusDisplay", "Sitka Display, Georgia");

    static FontFamily Build(string family, string filePrefix, string fallback)
    {
        string? file = null;
        try
        {
            var folder = Path.Combine(AppContext.BaseDirectory, "Assets", "Fonts");
            if (Directory.Exists(folder))
            {
                file = Directory.EnumerateFiles(folder, filePrefix + "*")
                    .Where(f => f.EndsWith(".otf", StringComparison.OrdinalIgnoreCase) || f.EndsWith(".ttf", StringComparison.OrdinalIgnoreCase))
                    .OrderBy(f => f.Contains("Regular", StringComparison.OrdinalIgnoreCase) ? 0 : 1)
                    .FirstOrDefault();
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
        }

        return new FontFamily(file != null
            ? $"ms-appx:///Assets/Fonts/{Path.GetFileName(file)}#{family}, {family}, {fallback}"
            : $"{family}, {fallback}");
    }
}
