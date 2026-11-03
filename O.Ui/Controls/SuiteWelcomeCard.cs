using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;

namespace O.Ui.Controls;

/// <summary>
/// The suite's welcome card, as O-Studio shows it: the app's lockup (mark, name, role, suite line), a short note on what the app
/// does here, the developer credit, then the app's own buttons. Every app puts its start choices in <see cref="Actions"/> and
/// <see cref="Extra"/> so the first screen reads alike across the suite.
/// </summary>
public sealed class SuiteWelcomeCard : UserControl
{
    public const string Developer = "Developed by Human Centric Works";

    readonly Border _card = new();
    readonly ScrollViewer _scroll = new() { Padding = new Thickness(32) };
    readonly Image _lockup = new() { HorizontalAlignment = HorizontalAlignment.Left, Stretch = Stretch.Uniform, MaxWidth = 460 };
    readonly TextBlock _note = new() { TextWrapping = TextWrapping.WrapWholeWords, FontSize = SuiteScale.FontBody };
    readonly TextBlock _credit = new() { Text = Developer, FontSize = SuiteScale.FontCaption };

    /// <summary>Buttons in a row under the credit (the primary choice first).</summary>
    public StackPanel Actions { get; } = new() { Orientation = Orientation.Horizontal, Spacing = 8 };

    /// <summary>Anything below the buttons (recent lists, notices).</summary>
    public StackPanel Extra { get; } = new() { Spacing = 8 };

    /// <summary>Fill behind the card, for when it covers the app's own page.</summary>
    public Brush? Backdrop
    {
        get => _scroll.Background;
        set => _scroll.Background = value;
    }

    public SuiteWelcomeCard(string appName, string lockupPath, string note)
    {
        _lockup.Source = new BitmapImage(new Uri(lockupPath));
        AutomationProperties.SetName(_lockup, appName);
        _note.Text = note;

        var stack = new StackPanel { Spacing = 12 };
        stack.Children.Add(_lockup);
        stack.Children.Add(_note);
        stack.Children.Add(_credit);
        stack.Children.Add(Actions);
        stack.Children.Add(Extra);

        _card.CornerRadius = new CornerRadius(4);
        _card.BorderThickness = new Thickness(1);
        _card.Padding = new Thickness(12);
        _card.MaxWidth = 560;
        _card.HorizontalAlignment = HorizontalAlignment.Center;
        _card.VerticalAlignment = VerticalAlignment.Center;
        _card.Child = stack;

        _scroll.Content = _card;
        Content = _scroll;
        AutomationProperties.SetName(this, "Welcome");
        Retheme();
        Loaded += (_, _) => SuiteAppearance.Changed += OnChanged;
        Unloaded += (_, _) => SuiteAppearance.Changed -= OnChanged;
    }

    void OnChanged(ElementTheme _) => DispatcherQueue.TryEnqueue(Retheme);

    void Retheme()
    {
        _card.Background = Brush("SurfaceBackgroundBrush", "#FFFFFF");
        _card.BorderBrush = Brush("BorderSubtleBrush", "#D8DEE8");
        _note.Foreground = Brush("TextPrimaryBrush", "#102C80");
        _credit.Foreground = Brush("TextSecondaryBrush", "#3E56A0");
    }

    static Brush Brush(string key, string hex)
    {
        if (SuiteAppearance.TryBrush(key) is Brush b)
            return b;
        var h = hex.TrimStart('#');
        return new SolidColorBrush(Windows.UI.Color.FromArgb(255, Convert.ToByte(h[..2], 16), Convert.ToByte(h[2..4], 16), Convert.ToByte(h[4..6], 16)));
    }
}
