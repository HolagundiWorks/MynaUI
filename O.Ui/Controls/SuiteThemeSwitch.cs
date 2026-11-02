using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace O.Ui.Controls;

/// <summary>Compact light/dark toggle bound to <see cref="SuiteAppearance"/>.</summary>
public sealed class SuiteThemeSwitch : ToggleButton
{
    Window? _window;

    public SuiteThemeSwitch()
    {
        // A checked ToggleButton takes the Windows accent colour (the user's, often cyan). The switch wears the app's accent instead.
        Resources = (ResourceDictionary)Microsoft.UI.Xaml.Markup.XamlReader.Load(
            """
            <ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
                <SolidColorBrush x:Key="ToggleButtonBackgroundChecked" Color="{ThemeResource SuiteAccent}"/>
                <SolidColorBrush x:Key="ToggleButtonBackgroundCheckedPointerOver" Color="{ThemeResource SuiteAccentHover}"/>
                <SolidColorBrush x:Key="ToggleButtonBackgroundCheckedPressed" Color="{ThemeResource SuiteAccentPressed}"/>
                <SolidColorBrush x:Key="ToggleButtonBorderBrushChecked" Color="{ThemeResource SuiteAccentPressed}"/>
                <SolidColorBrush x:Key="ToggleButtonBorderBrushCheckedPointerOver" Color="{ThemeResource SuiteAccentPressed}"/>
                <SolidColorBrush x:Key="ToggleButtonBorderBrushCheckedPressed" Color="{ThemeResource SuiteAccentPressed}"/>
                <SolidColorBrush x:Key="ToggleButtonForegroundChecked" Color="{ThemeResource SuiteOnAccent}"/>
                <SolidColorBrush x:Key="ToggleButtonForegroundCheckedPointerOver" Color="{ThemeResource SuiteOnAccent}"/>
                <SolidColorBrush x:Key="ToggleButtonForegroundCheckedPressed" Color="{ThemeResource SuiteOnAccent}"/>
            </ResourceDictionary>
            """);
        Content = "Theme";
        MinWidth = 72;
        MinHeight = SuiteScale.ControlMinHeight;
        Padding = new Thickness(8, 2, 8, 2);
        AutomationProperties.SetName(this, "Theme");
        ToolTipService.SetToolTip(this, "Switch light or dark theme");
        Click += OnClick;
        SuiteAppearance.Changed += OnAppearanceChanged;
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
        IsChecked = SuiteAppearance.IsDark;
        Content = SuiteAppearance.IsDark ? "Dark" : "Light";
        AutomationProperties.SetName(this, SuiteAppearance.IsDark ? "Theme, dark" : "Theme, light");
    }
}
