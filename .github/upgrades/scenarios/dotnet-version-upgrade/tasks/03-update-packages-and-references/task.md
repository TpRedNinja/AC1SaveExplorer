# 03-update-packages-and-references: Update NuGet packages and assembly references

Replace or update packages to versions compatible with net8.0. Add System.Configuration.ConfigurationManager if legacy config usage remains.

**Done when**: All package references resolve to compatible versions and no package restore errors remain.

---

## Research findings

- The project has no PackageReference entries and no packages.config (assessment found 0 NuGet packages).
- Explicit <Reference> entries for framework assemblies (Microsoft.CSharp, System.Data.DataSetExtensions, System.Net.Http) were present and caused assembly resolution warnings when targeting net8.0.
- For net8.0-windows, these references are provided by the target framework; explicit Reference items were removed to rely on implicit framework references.

## Actions performed

1. Removed explicit <Reference> items for Microsoft.CSharp, System.Data.DataSetExtensions, and System.Net.Http from the csproj.
2. Built the project with msbuild.exe to verify resolution.

## Build result

- Build succeeded for net8.0-windows with 0 warnings and 0 errors after removing explicit references.

## Next steps

- If project code requires APIs previously provided by those assemblies as packages, add PackageReference entries for the specific packages (e.g., System.Data.DataSetExtensions) and re-run restore/build.
- Proceed to Task 04: fix API incompatibilities identified in assessment.md.
