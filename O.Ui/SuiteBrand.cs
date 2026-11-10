namespace O.Ui;

/// <summary>The apps of the suite. Each has its own name and accent colour.</summary>
public enum SuiteApp
{
    /// <summary>MynaHub: the office. Orange. Identifier remains O-Studio.</summary>
    Studio,

    /// <summary>MynaOps: quantities and costing. Wild Sand. Identifier remains O-CoreOp.</summary>
    Metra,

    /// <summary>MynaCAD: drafting. Dark Slate Blue (Mirage). Identifier remains O-Draft.</summary>
    Draft,

    /// <summary>MynaMod: 3D and BIM form. Deep Sea Green. Planned. Identifier remains O-Form.</summary>
    Form,
}

/// <summary>The names of the suite and its apps, so every app and document spells them the same way.</summary>
public static class SuiteBrand
{
    public const string Short = "OSAS";
    public const string FullName = "Open Source Architecture Suit";

    /// <summary>User-visible name of the hub. The install folder and ticket stay <c>O-Studio</c>.</summary>
    public const string Hub = "MynaHub";

    /// <summary>User-visible name of the quantities app. Identifiers stay <c>O-CoreOp</c>.</summary>
    public const string Quantities = "MynaOps";

    /// <summary>User-visible name of the drafting app. Identifiers stay <c>O-Draft</c>.</summary>
    public const string Drafting = "MynaCAD";

    /// <summary>User-visible name of the planned form app. Identifiers stay <c>O-Form</c>.</summary>
    public const string FormName = "MynaMod";

    /// <summary>User-visible name of the launcher. The executable stays <c>O-Studio.Launcher.exe</c>.</summary>
    public const string Launcher = "Myna";

    public static string Name(SuiteApp app) => app switch
    {
        SuiteApp.Studio => Hub,
        SuiteApp.Metra => Quantities,
        SuiteApp.Form => FormName,
        _ => Drafting,
    };

    /// <summary>What the app is for, in a few words.</summary>
    public static string Purpose(SuiteApp app) => app switch
    {
        SuiteApp.Studio => "Office and project management",
        SuiteApp.Metra => "Quantities, estimates and billing",
        SuiteApp.Form => "3D and BIM form",
        _ => "Drafting",
    };

    /// <summary>The name of the accent: the middle part of the SuiteAccent.* and SuiteLight.* file names.</summary>
    public static string Accent(SuiteApp app) => app switch
    {
        SuiteApp.Studio => "Orange",
        SuiteApp.Metra => "Sand",
        SuiteApp.Form => "Sea",
        _ => "Mirage",
    };
}
