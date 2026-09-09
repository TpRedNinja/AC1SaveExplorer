# .NET Version Upgrade Plan

## Overview

**Target**: AC1SaveExplorer solution → net8.0
**Scope**: 1 WPF project (net48) → convert to SDK-style and retarget to net8.0-windows; update packages and fix API incompatibilities identified in assessment.md

## Tasks

### 01-convert-to-sdk-style: Convert projects to SDK-style

Convert the classic csproj(s) to SDK-style project files so they can target modern TFMs and use SDK features.

**Done when**: Project files are SDK-style, the solution loads in Visual Studio without project file format errors, and unit/build tools can read the projects.

---

### 02-update-target-framework: Change TargetFramework to net8.0-windows

Update the project TargetFramework to net8.0-windows (or net8.0 with Windows-specific pack if required by WPF). Ensure project builds and Windows-specific SDKs are referenced as needed.

**Done when**: Projects compile targeting net8.0-windows and WPF still launches without immediate runtime failures.

---

### 03-update-packages-and-references: Update NuGet packages and assembly references

Replace or update packages to versions compatible with net8.0. Add System.Configuration.ConfigurationManager if legacy config usage remains.

**Done when**: All package references resolve to compatible versions and no package restore errors remain.

---

### 04-fix-api-incompatibilities: Address binary/source incompatible APIs

Fix the API incompatibilities identified in assessment.md (e.g., WPF API constructor/behavior differences, configuration APIs). Apply code changes and modernization patterns.

**Done when**: The solution builds with zero errors and no remaining API compatibility diagnostics from the assessor.

---

### 05-validation-and-tests: Build, run smoke tests, and validate

Run a full build, launch the WPF app for a basic smoke test, and run unit tests (if present). Fix any remaining issues.

**Done when**: Solution builds cleanly, smoke test executes, and tests pass.

