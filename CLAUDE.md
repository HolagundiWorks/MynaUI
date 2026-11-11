# MynaUI: rules for coding agents

`MynaUI` is the shared WinUI 3 library of the Myna suite (assembly `O.Ui`): themes, controls, brand art and the brand generator. It is consumed by MynaHub (MynaHub), MynaOps (MynaOps) and MynaCAD (MynaCAD) through relative project references (`..\..\..\MynaUI`), so the folder must stay named `MynaUI` beside the other repos. Brand guide: `docs/BRANDING.md` in any app repo (identical copies).

## Rules
- **A change here changes three apps.** After editing `O.Ui`, build all three: `dotnet build native/Adms.Desktop/Adms.Desktop.csproj -c Release -p:Platform=x64` (MynaHub), `dotnet build BBSDesktop/BBSApp/BBSApp.csproj -c Release -p:Platform=x64 -p:WindowsPackageType=None` (MynaOps), `dotnet build apps/aad-winui/Aad.WinUi.csproj -c Release -p:Platform=x64` (MynaCAD). Build this library with `dotnet build O.Ui/O.Ui.csproj`.
- **Colours only in `O.Ui/Themes/`.** Two themes: pure white (`#FFFFFF` surfaces, `#000000` text) and pure black; accents are per app (`SuiteAccent.<App>.xaml`). Do not hard-code colours in controls; code-built controls take brushes from theme resources with a fallback equal to the theme value.
- **Brand art is generated.** Marks, icons and lockups come from `tools/Brand` (`dotnet run --project tools/Brand -- <dir>`), then are copied into `brand/` and each app's Assets. Do not hand-edit generated PNG/ICO. Three-circle mark on each app's colour; the orb is the AI agent's symbol.
- **`external/` is vendored** (WindowsCommunityToolkit, WinUI-Gallery): never edit it.
- **Accessibility.** Contrast pairs are checked by `tools/a11y/contrast.mjs` in MynaHub; keep WCAG 2.2 AA (text 4.5:1, component edges 3:1).
- Say only what you built or ran. Git: add files by name, never `git add -A`; the default branch is `master`.
