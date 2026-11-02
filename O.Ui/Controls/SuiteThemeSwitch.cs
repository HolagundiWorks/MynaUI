using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace O.Ui.Controls;

/// <summary>
/// The light/dark button of the bottom status row, bound to <see cref="SuiteAppearance"/>. It is the same icon button as
/// O-Draft's (a drawn sun while light is on, a moon while dark is on), so every app's status row reads alike.
/// </summary>
public sealed class SuiteThemeSwitch : Button
{
    readonly SuiteDrawnIcon _icon = new();
    Window? _window;

    public SuiteThemeSwitch()
    {
        Content = _icon;
        Width = 28;
        Height = 26;
        MinWidth = 0;
        MinHeight = 0;
        Padding = new Thickness(0);
        BorderThickness = new Thickness(0);
        Background = new SolidColorBrush(Microsoft.UI.Colors.Transparent);
        CornerRadius = new CornerRadius(4);
        VerticalAlignment = VerticalAlignment.Center;
        Click += OnClick;
        Loaded += (_, _) => SuiteAppearance.Changed += OnAppearanceChanged;
        Unloaded += (_, _) => SuiteAppearance.Changed -= OnAppearanceChanged;
        Sync();
    }

    /// <summary>Apply the current suite theme to the window and keep it in sync.</summary>
    public void Attach(Window window)
    {
        _window = window;
        SuiteAppearance.Apply(window);
        Sync();
    }

    void OnClick(object sender, RoutedEventArgs e)
    {
        SuiteAppearance.Toggle();
        if (_window is not null)
            SuiteAppearance.Apply(_window);
        Sync();
    }

    void OnAppearanceChanged(ElementTheme _)
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            if (_window is not null)
                SuiteAppearance.Apply(_window);
            Sync();
        });
    }

    void Sync()
    {
        // The mark shows the theme that is on, as in O-Draft: sun for light, moon for dark.
        _icon.Kind = SuiteAppearance.IsDark ? "moon" : "sun";
        var tip = SuiteAppearance.IsDark ? "Dark theme — click for light" : "Light theme — click for dark";
        ToolTipService.SetToolTip(this, tip);
        AutomationProperties.SetName(this, SuiteAppearance.IsDark ? "Theme, dark" : "Theme, light");
    }
}
