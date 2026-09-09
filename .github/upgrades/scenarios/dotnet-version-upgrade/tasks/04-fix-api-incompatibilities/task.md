# 04-fix-api-incompatibilities: Address binary/source incompatible APIs

Fix the API incompatibilities identified in assessment.md (e.g., WPF API constructor/behavior differences, configuration APIs). Apply code changes and modernization patterns.

**Done when**: The solution builds with zero errors and no remaining API compatibility diagnostics from the assessor.

---

## Research findings

- Assessment flagged WPF APIs (Application, Window constructors, LoadComponent) and legacy configuration APIs (ApplicationSettingsBase) as potential compatibility challenges.
- Current codebase uses App.xaml with StartupUri and an App.config containing a .NET Framework-specific supportedRuntime entry.
- After retargeting to net8.0-windows, the project builds but the App.config's supportedRuntime was not applicable to .NET 8 and should be removed.

## Actions performed

1. Removed .NET Framework-specific supportedRuntime entry from App.config to avoid conflicting runtime hints.
2. Verified build with msbuild.exe for net8.0-windows; build succeeded with 0 warnings and 0 errors.

## Next steps

- If runtime behavior shows missing APIs at runtime (e.g., Application behavior, settings), consider adding System.Configuration.ConfigurationManager package and migrate Settings usage to Microsoft.Extensions.Configuration where appropriate.
- Proceed to Task 05: validation and tests.
