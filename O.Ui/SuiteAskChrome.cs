using Microsoft.UI.Xaml;

namespace O.Ui;

/// <summary>
/// Shared layout for the floating Ask bar at the bottom of O-Studio, O-CoreOp and O-Draft (pill + optional reply card above).
/// </summary>
public static class SuiteAskChrome
{
    public const double BarMaxWidth = 720;
    public const double BarCornerRadius = BarHeight / 2; // a full pill: a radius over half the height distorts the ends
    public const double ReplyCornerRadius = 12;
    public const double BarHeight = 40;
    public const double AvatarSize = 28;
    public const double SendButtonSize = 32;
    public const double BottomMargin = 18;
    public const double HorizontalMargin = 16;
    public static Thickness BarMargin => new(HorizontalMargin, 0, HorizontalMargin, BottomMargin);
    public static Thickness BarPadding => new(6, 4, 6, 4);
    public static Thickness ReplyPadding => new(12, 8, 12, 8);
}
