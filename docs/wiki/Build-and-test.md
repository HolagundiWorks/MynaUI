# Build and test

```
dotnet build O.Ui/O.Ui.csproj
dotnet run --project tools/Brand -- <outdir>
```

After editing `O.Ui`, build all three apps:

```
dotnet build native/Adms.Desktop/Adms.Desktop.csproj -c Release -p:Platform=x64                  # MynaHub
dotnet build BBSDesktop/BBSApp/BBSApp.csproj -c Release -p:Platform=x64 -p:WindowsPackageType=None  # MynaOps
dotnet build apps/aad-winui/Aad.WinUi.csproj -c Release -p:Platform=x64                           # MynaCAD
```
