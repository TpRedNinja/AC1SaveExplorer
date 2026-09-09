# .NET Version Upgrade — Report

**Scenario:** Upgrade AC1SaveExplorer solution to net8.0-windows
**Outcome:** ✅ Fully completed
**Projects affected:** 1 (AC1SaveExplorer)
**Tasks:** 5/5 completed

---

## Summary

This scenario converted a single WPF project from .NET Framework 4.8 to .NET 8.0 (net8.0-windows). Work completed: project file conversion to SDK-style, retargeting to net8.0-windows, removal of explicit framework assembly references, remediation of framework-specific configuration entries, and final build and smoke testing. The solution builds and the app launches in a brief smoke test.

---

## What Changed

### Project file changes

- Converted AC1SaveExplorer\AC1SaveExplorer.csproj from legacy (non-SDK) format to SDK-style.
- Retargeted AC1SaveExplorer.csproj to <code>net8.0-windows</code> and enabled nullable reference types (Nullable=enable). ImplicitUsings was disabled to keep migration changes explicit.
- Removed explicit <Reference> entries for framework assemblies (Microsoft.CSharp, System.Data.DataSetExtensions, System.Net.Http) to rely on the target framework's implicit references.

### Configuration

- Removed the .NET Framework-specific <startup>/<supportedRuntime> entry from App.config; retained an empty configuration node for any remaining settings.

### Build & tools

- Added a build-tool decision to scenario-instructions.md: prefer msbuild.exe for WPF/XAML builds during validation.
- Verified project and solution builds with msbuild.exe.

---

## Task Breakdown

| Task | Description | Outcome | Links |
|------|-------------|---------|-------|
| 01-convert-to-sdk-style | Convert projects to SDK-style | ✅ Completed — project converted and built as legacy TFM | [task.md](tasks/01-convert-to-sdk-style/task.md) / [progress](tasks/01-convert-to-sdk-style/progress-details.md) |
| 02-update-target-framework | Change TargetFramework to net8.0-windows | ✅ Completed — retargeted and built (6 warnings initially) | [task.md](tasks/02-update-target-framework/task.md) / [progress](tasks/02-update-target-framework/progress-details.md) |
| 03-update-packages-and-references | Update NuGet packages and assembly references | ✅ Completed — removed explicit framework references; build clean | [task.md](tasks/03-update-packages-and-references/task.md) / [progress](tasks/03-update-packages-and-references/progress-details.md) |
| 04-fix-api-incompatibilities | Address binary/source incompatible APIs | ✅ Completed — removed Framework-specific runtime hints; build clean | [task.md](tasks/04-fix-api-incompatibilities/task.md) / [progress](tasks/04-fix-api-incompatibilities/progress-details.md) |
| 05-validation-and-tests | Build, run smoke tests, and validate | ✅ Completed — full solution build succeeded; app started/stopped in smoke test | [task.md](tasks/05-validation-and-tests/task.md) / [progress](tasks/05-validation-and-tests/progress-details.md) |

---

## Files Modified

- AC1SaveExplorer\AC1SaveExplorer.csproj
- AC1SaveExplorer\App.config
- .github\upgrades\scenarios\dotnet-version-upgrade\scenario-instructions.md
- .github\upgrades\scenarios\dotnet-version-upgrade\assessment.md
- .github\upgrades\scenarios\dotnet-version-upgrade\plan.md
- .github\upgrades\scenarios\dotnet-version-upgrade\tasks.md
- .github\upgrades\scenarios\dotnet-version-upgrade\tasks\01-convert-to-sdk-style\task.md
- .github\upgrades\scenarios\dotnet-version-upgrade\tasks\01-convert-to-sdk-style\progress-details.md
- .github\upgrades\scenarios\dotnet-version-upgrade\tasks\02-update-target-framework\task.md
- .github\upgrades\scenarios\dotnet-version-upgrade\tasks\02-update-target-framework\progress-details.md
- .github\upgrades\scenarios\dotnet-version-upgrade\tasks\03-update-packages-and-references\task.md
- .github\upgrades\scenarios\dotnet-version-upgrade\tasks\03-update-packages-and-references\progress-details.md
- .github\upgrades\scenarios\dotnet-version-upgrade\tasks\04-fix-api-incompatibilities\task.md
- .github\upgrades\scenarios\dotnet-version-upgrade\tasks\04-fix-api-incompatibilities\progress-details.md
- .github\upgrades\scenarios\dotnet-version-upgrade\tasks\05-validation-and-tests\task.md
- .github\upgrades\scenarios\dotnet-version-upgrade\tasks\05-validation-and-tests\progress-details.md

---

## Build & Test Results

- Solution build (Release): ✅ succeeded, 0 warnings, 0 errors
- Smoke test: ✅ application started and was stopped after 3 seconds with no crash observed
- Unit tests: none detected

---

## Decisions Made

- **Flow Mode** — Automatic (user request)
- **Target Framework** — net8.0-windows (user requested)
- **Build Tool** — Prefer `msbuild.exe` for WPF/XAML validation steps

---

## Known Gaps & Follow-up Items

- No unit tests present; consider adding automated tests for regression coverage.
- If runtime execution surfaces APIs missing at runtime, add specific PackageReference items (e.g., System.Configuration.ConfigurationManager, System.Data.DataSetExtensions) as needed.
- Consider migrating application settings from the legacy Settings/App.config model to Microsoft.Extensions.Configuration for long-term maintainability.

---

Report generated at: .github/upgrades/scenarios/dotnet-version-upgrade/final-report.md
