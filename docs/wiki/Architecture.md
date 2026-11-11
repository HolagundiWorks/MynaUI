# Architecture

- `O.Ui/` the class library: `Themes/` (pure white and pure black themes; per-app accents in `SuiteAccent.<App>.xaml`), controls, `SuiteBrand.cs`.
- `brand/` generated marks, icons, lockups and the AI orb.
- `tools/Brand` generates the brand art; `tools/a11y` contrast checks live in MynaHub.
- `external/` is vendored (WindowsCommunityToolkit, WinUI-Gallery) and never edited.

A change here changes three apps.
