# .NET Version Upgrade: Progress

## Overview

Upgrade the AC1SaveExplorer solution from .NET Framework 4.8 to .NET 8.0. Approach: convert projects to SDK-style, retarget to net8.0-windows, update packages, fix API issues, validate.

**Progress**: 5/5 tasks complete <progress value="100" max="100"></progress> 100%

## Tasks

- ✅ 01-convert-to-sdk-style: Convert projects to SDK-style ([Content](tasks/01-convert-to-sdk-style/task.md), [Progress](tasks/01-convert-to-sdk-style/progress-details.md))
- ✅ 02-update-target-framework: Change TargetFramework to net8.0-windows ([Content](tasks/02-update-target-framework/task.md), [Progress](tasks/02-update-target-framework/progress-details.md))
- ✅ 03-update-packages-and-references: Update NuGet packages and assembly references ([Content](tasks/03-update-packages-and-references/task.md), [Progress](tasks/03-update-packages-and-references/progress-details.md))
- ✅ 04-fix-api-incompatibilities: Address binary/source incompatible APIs ([Content](tasks/04-fix-api-incompatibilities/task.md), [Progress](tasks/04-fix-api-incompatibilities/progress-details.md))
- ✅ 05-validation-and-tests: Build, run smoke tests, and validate ([Content](tasks/05-validation-and-tests/task.md), [Progress](tasks/05-validation-and-tests/progress-details.md))
