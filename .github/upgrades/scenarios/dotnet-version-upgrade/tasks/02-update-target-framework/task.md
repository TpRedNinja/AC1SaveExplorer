# 02-update-target-framework: Change TargetFramework to net8.0-windows

Update the project TargetFramework to net8.0-windows (or net8.0 with Windows-specific pack if required by WPF). Ensure project builds and Windows-specific SDKs are referenced as needed.

**Done when**: Projects compile targeting net8.0-windows and WPF still launches without immediate runtime failures.

---

## Research findings

- Affected project file (SDK-style): C:\Users\jjdom\OneDrive\AC1SaveExplorer\AC1SaveExplorer\AC1SaveExplorer\AC1SaveExplorer.csproj
- Project was converted to SDK-style in the previous task.
- Project type: WPF; UseWPF=true is present.
- Previous TFM: net48. Retargeting to net8.0-windows is required for WPF on .NET 8.

## Actions performed

1. Updated TargetFramework to net8.0-windows and enabled Nullable (enable) and disabled ImplicitUsings to keep migration changes explicit.
2. Built the project using msbuild.exe (/restore /t:Build /p:Configuration=Release) as WPF builds prefer msbuild.

## Build result

- Build succeeded for net8.0-windows with 6 warnings and 0 errors. Warnings indicate assembly resolution choices for references (Microsoft.CSharp, System.Data.DataSetExtensions, System.Net.Http). These are expected during retargeting and will be addressed when updating package references or replacing framework assemblies.

## Next steps

- Update package references and assembly references (task 03).
- Address API incompatibilities flagged in assessment (task 04).
