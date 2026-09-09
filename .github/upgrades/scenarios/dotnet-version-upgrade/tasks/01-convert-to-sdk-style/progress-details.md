# Progress Details — 01-convert-to-sdk-style

Actions performed:

- Converted AC1SaveExplorer\AC1SaveExplorer.csproj from classic (non-SDK) format to SDK-style using the conversion tool.
- Did not change TargetFramework/TargetFrameworkVersion (conversion was format-only).
- Recorded a build tool decision in scenario-instructions.md: prefer msbuild.exe for WPF/XAML builds.
- Built the converted project using msbuild.exe (/restore /t:Build /p:Configuration=Release).

Build result:
- Build succeeded (0 warnings, 0 errors) for the converted project when built as net48 (format-only conversion preserved TFM).

Notes and next steps:
- The project file is now SDK-style and builds successfully under the original TFM (net48).
- Next task: update TargetFramework to net8.0-windows (task 02-update-target-framework) and resolve platform-specific WPF SDK references as needed.

Files changed during this task:
- AC1SaveExplorer\AC1SaveExplorer.csproj
- .github\upgrades\scenarios\dotnet-version-upgrade\scenario-instructions.md
- .github\upgrades\scenarios\dotnet-version-upgrade\tasks\01-convert-to-sdk-style\task.md

