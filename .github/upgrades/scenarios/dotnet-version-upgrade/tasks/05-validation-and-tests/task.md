# 05-validation-and-tests: Build, run smoke tests, and validate

Run a full build, launch the WPF app for a basic smoke test, and run unit tests (if present). Fix any remaining issues.

**Done when**: Solution builds cleanly, smoke test executes, and tests pass.

---

## Research findings

- No unit tests detected in the solution.
- The build artifacts are at bin\Release\net8.0-windows\AC1SaveExplorer.exe (apphost) and AC1SaveExplorer.dll.

## Actions planned

1. Run a full solution build with msbuild.exe (/restore /t:Build /p:Configuration=Release).
2. Launch the app for a brief smoke test (start then terminate after a few seconds).
3. Report results and mark task complete if successful.
