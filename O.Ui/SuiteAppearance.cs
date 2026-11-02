using System.Text.Json;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace O.Ui;

/// <summary>
/// Shared light/dark preference for the suite
/// (<c>%LOCALAPPDATA%\O-Studio\appearance.json</c>), kept in sync with O-Draft's
/// <c>SuiteAppearancePrefs</c> contract.
/// </summary>
public static class SuiteAppearance
{
    static ElementTheme _current = Load();

    public static event Action<ElementTheme>? Changed;

    public static ElementTheme Current => _current;

    public static bool IsDark => _current == ElementTheme.Dark;

    public static void Apply(Window window)
    {
        if (window.Content is FrameworkElement root)
            root.RequestedTheme = _current;
    }

    public static void Set(ElementTheme theme)
    {
        if (theme is not (ElementTheme.Light or ElementTheme.Dark))
            theme = ElementTheme.Light;
        if (_current == theme)
            return;
        _current = theme;
        Save(theme);
        Changed?.Invoke(theme);
    }

    public static void Toggle() =>
        Set(IsDark ? ElementTheme.Light : ElementTheme.Dark);

    /// <summary>
    /// A brush of the suite's current theme. Code-built controls read their colours here; looking in
    /// <c>Application.Resources</c> directly follows the application's theme (the system's), not the light or dark the
    /// person picked, so a control built or re-themed after a switch kept the old colours.
    /// </summary>
    public static Brush? TryBrush(string key)
    {
        try
        {
            var resources = Application.Current?.Resources;
            if (resources is null)
                return null;
            var themeKey = _current == ElementTheme.Dark ? "Dark" : "Light";
            if (Find(resources, key, themeKey) is Brush found)
                return found;
            if (resources.TryGetValue(key, out var value) && value is Brush brush)
                return brush;
        }
        catch
        {
        }
        return null;
    }

    /// <summary>
    /// A resource of the chosen theme: the theme dictionary of this dictionary first (<c>Light</c>, or <c>Dark</c> then <c>Default</c>), then
    /// the plain entries, then the dictionaries merged into it, each searched the same way (an app can merge a bridge file that has
    /// theme dictionaries of its own).
    /// </summary>
    static object? Find(ResourceDictionary dictionary, string key, string themeKey)
    {
        foreach (var name in themeKey == "Dark" ? new[] { "Dark", "Default" } : new[] { "Light" })
        {
            if (dictionary.ThemeDictionaries.TryGetValue(name, out var themed) && themed is ResourceDictionary t && Find(t, key, themeKey) is { } inTheme)
                return inTheme;
        }

        // The merged dictionaries come before this dictionary's own TryGetValue, which reads theme dictionaries in the application's theme.
        foreach (var merged in dictionary.MergedDictionaries)
            if (Find(merged, key, themeKey) is { } found)
                return found;
        return dictionary.TryGetValue(key, out var value) ? value : null;
    }

    static ElementTheme Load()
    {
        try
        {
            var path = PrefPath();
            if (!File.Exists(path))
                return ElementTheme.Dark;
            using var doc = JsonDocument.Parse(File.ReadAllText(path));
            if (doc.RootElement.TryGetProperty("theme", out var theme) &&
                theme.GetString() is { } name)
            {
                if (name.Equals("light", StringComparison.OrdinalIgnoreCase))
                    return ElementTheme.Light;
                if (name.Equals("dark", StringComparison.OrdinalIgnoreCase))
                    return ElementTheme.Dark;
            }
        }
        catch (Exception ex) when (ex is IOException or JsonException or UnauthorizedAccessException)
        {
        }
        return ElementTheme.Dark;
    }

    static void Save(ElementTheme theme)
    {
        try
        {
            var path = PrefPath();
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);
            var json = JsonSerializer.Serialize(
                new { theme = theme == ElementTheme.Dark ? "dark" : "light" },
                new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
        }
    }

    static string PrefPath() => SuitePaths.File("appearance.json");
}
