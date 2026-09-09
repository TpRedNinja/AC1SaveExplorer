# Progress Details — 03-update-packages-and-references

Actions performed:

- Removed explicit framework assembly <Reference> entries for Microsoft.CSharp, System.Data.DataSetExtensions, and System.Net.Http from the SDK-style csproj.
- Built the project using msbuild.exe (/restore /t:Build /p:Configuration=Release) to verify that implicit framework references resolve correctly.

Build result:

- Build succeeded for net8.0-windows with 0 warnings and 0 errors after the change.

Files changed during this task:
- C:\Users\jjdom\OneDrive\AC1SaveExplorer\AC1SaveExplorer\AC1SaveExplorer\AC1SaveExplorer.csproj
- .github\upgrades\scenarios\dotnet-version-upgrade\tasks\03-update-packages-and-references\task.md
- .github\upgrades\scenarios\dotnet-version-upgrade\tasks\03-update-packages-and-references\progress-details.md

Notes and next steps:

- If APIs from the removed references are missing at runtime or compile-time, add targeted PackageReference entries (e.g., System.Data.DataSetExtensions) and re-run restore/build.
- Proceed to Task 04 — fix API incompatibilities identified in assessment.md.
