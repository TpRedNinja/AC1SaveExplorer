# 01-convert-to-sdk-style: Convert projects to SDK-style

Convert the classic csproj(s) to SDK-style project files so they can target modern TFMs and use SDK features.

**Done when**: Project files are SDK-style, the solution loads in Visual Studio without project file format errors, and unit/build tools can read the projects.

---

## Research findings

- Affected project: AC1SaveExplorer\AC1SaveExplorer.csproj (classic, non-SDK-style)
- Project type: WPF (ProjectTypeGuids includes WPF GUID)
- Current TargetFramework: v4.8 (TargetFrameworkVersion)
- Notable files: App.xaml, MainWindow.xaml, Properties\Resources.resx, App.config, Properties\Settings.settings
- References: PresentationFramework, PresentationCore, WindowsBase, System.Xaml (WPF assemblies)
- Packages: No PackageReference entries detected; no packages.config file present (assessment showed 0 NuGet packages)

## Constraints & Recommendations

- Conversion must be format-only: do not change TargetFramework/TargetFrameworkVersion during this step.
- Use the SDK-style conversion tool (do not hand-edit the XML) and convert one project at a time.
- After conversion, build the converted project directly to validate; because this is a WPF project, prefer msbuild.exe for the first build if XAML/markup compilation errors occur.
- Ensure packages/assembly references are preserved; if `packages.config` exists, migrate to PackageReference as a separate step (not part of format-only conversion).

## Next actions

1. Run SDK-style conversion tool on AC1SaveExplorer\AC1SaveExplorer.csproj
2. Build the converted project and capture diagnostics
3. If the project builds, commit/checkpoint the conversion and proceed to next task
