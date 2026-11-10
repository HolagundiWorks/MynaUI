using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace O.Ui.Controls;

/// <summary>
/// O-Draft bottom status row: full width, status surface, hairline top border, scrollable left cluster and fixed right cluster.
/// </summary>
public sealed class SuiteBottomStatusBar : UserControl
{
    readonly Border _frame;
    readonly TextBlock _center;

    public StackPanel Left { get; } = new() { Orientation = Orientation.Horizontal, Spacing = SuiteScale.StackGap };

    public StackPanel Right { get; } = new() { Orientation = Orientation.Horizontal, Spacing = SuiteScale.StackGap, VerticalAlignment = VerticalAlignment.Center };

    public string StatusText
    {
        get => _center.Text;
        set => _center.Text = value ?? "";
    }

    public SuiteBottomStatusBar()
    {
        _frame = new Border();
        ApplyChrome(_frame);

        var row = new Grid
        {
            MinHeight = SuiteScale.StatusBarHeight,
            ColumnSpacing = SuiteScale.StackGap,
        };
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        var leftScroll = new ScrollViewer
        {
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            VerticalScrollBarVisibility = ScrollBarVisibility.Disabled,
            Content = Left,
        };
        _center = new TextBlock
        {
            VerticalAlignment = VerticalAlignment.Center,
            FontSize = SuiteScale.FontCaption,
            TextTrimming = TextTrimming.CharacterEllipsis,
            MaxWidth = 420,
        };
        ApplyStatusTextBrush(_center);

        row.Children.Add(leftScroll);
        Grid.SetColumn(_center, 1);
        row.Children.Add(_center);
        Grid.SetColumn(Right, 2);
        row.Children.Add(Right);
        _frame.Child = row;
        Content = _frame;

        AutomationProperties.SetName(this, "Status");

        // Colours are read when the bar is built; read them again whenever the person switches light and dark.
        Loaded += (_, _) => SuiteAppearance.Changed += OnAppearanceChanged;
        Unloaded += (_, _) => SuiteAppearance.Changed -= OnAppearanceChanged;
    }

    void OnAppearanceChanged(ElementTheme _) => DispatcherQueue.TryEnqueue(Retheme);

    public void Retheme()
    {
        ApplyChrome(_frame);
        ApplyStatusTextBrush(_center);
    }

    static void ApplyChrome(Border frame)
    {
        frame.Background = ThemeBrush("SurfaceStatusBrush", "SidebarBackgroundBrush", "#FFFFFF");
        frame.BorderBrush = ThemeBrush("BorderSubtleBrush", "BorderDefaultBrush", "#D9D9D9");
        frame.BorderThickness = new Thickness(0, 1, 0, 0);
        frame.Padding = SuiteScale.StatusBarPadding;
    }

    static void ApplyStatusTextBrush(TextBlock block)
    {
        block.Foreground = ThemeBrush("TextSecondaryBrush", "TextPrimaryBrush", "#64748B");
    }

    static Brush ThemeBrush(string primary, string fallback, string lightHex)
    {
        if (SuiteAppearance.TryBrush(primary) is Brush b) return b;
        if (SuiteAppearance.TryBrush(fallback) is Brush f) return f;
        return new SolidColorBrush(ParseColor(lightHex));
    }

    static Windows.UI.Color ParseColor(string hex)
    {
        hex = hex.TrimStart('#');
        return Windows.UI.Color.FromArgb(255,
            Convert.ToByte(hex[..2], 16),
            Convert.ToByte(hex[2..4], 16),
            Convert.ToByte(hex[4..6], 16));
    }
}
