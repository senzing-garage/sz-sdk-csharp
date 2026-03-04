# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Test Commands

All commands run from the repository root. Use `gmake` (GNU make) for Makefile targets.

```bash
# Build demo snippets (must run before docs build)
dotnet test Senzing.Sdk.Demo -p:GeneratePackageOnBuild=false
gmake demo

# Build SDK (debug)
dotnet build Senzing.Sdk -p:GenerateDocumentationFile=true
gmake debug

# Build SDK (release)
dotnet build -c Release Senzing.Sdk -p:GenerateDocumentationFile=true
gmake release

# Run all tests
dotnet test Senzing.Sdk.Tests
gmake test

# Run a single test class
dotnet test Senzing.Sdk.Tests --filter "FullyQualifiedName~SzCoreEngineReadTest"

# Run a single test method
dotnet test Senzing.Sdk.Tests --filter "FullyQualifiedName~SzCoreEngineReadTest.TestGetEntity"

# Verify code formatting (fails if changes needed)
dotnet format --verify-no-changes --verbosity diagnostic Senzing.Sdk
dotnet format --verify-no-changes --verbosity diagnostic Senzing.Sdk.Tests
dotnet format --verify-no-changes --verbosity diagnostic Senzing.Sdk.Demo
gmake format

# Generate API docs
dotnet docfx docfx.json
gmake docs

# Clean
gmake clean
```

### Prerequisites

Tests require the native Senzing library:
- **Linux:** `export LD_LIBRARY_PATH=/opt/senzing/er/lib:$LD_LIBRARY_PATH`
- **macOS:** `export DYLD_LIBRARY_PATH=/opt/homebrew/lib:$DYLD_LIBRARY_PATH`
- **All platforms:** `export SENZING_PATH=<senzing-install-dir>`

## Architecture

This is the Senzing C# SDK — a wrapper around the native Senzing entity resolution C library (`libSz`).

### Three-Layer Design

**Layer 1 — Public Interfaces** (`Senzing.Sdk/`):
`SzEnvironment`, `SzEngine`, `SzConfig`, `SzConfigManager`, `SzProduct`, `SzDiagnostic` define the public API. `SzFlag`/`SzFlags`/`SzFlagUsageGroup` define bitwise operation flags.

**Layer 2 — Core Implementations** (`Senzing.Sdk/core/SzCore*.cs`):
`SzCoreEnvironment` is the central singleton managing SDK lifecycle (Active → Destroying → Destroyed). All operations go through its thread-safe `Execute<T>()` method. Other `SzCore*` classes implement the public interfaces by delegating to native bindings.

**Layer 3 — Native P/Invoke Bindings** (`Senzing.Sdk/core/Native*.cs`):
Each native API has an interface (`NativeEngine`, `NativeConfig`, etc.) and a `*Extern` implementation using `[DllImport("Sz")]`. Strings are marshaled as UTF-8 byte arrays via `Utilities.StringToUTF8Bytes()`. Return codes are `long` (0 = success). Errors are retrieved via `GetLastException()`/`GetLastExceptionCode()` and mapped to typed exceptions by `SzExceptionMapper` (auto-generated from `sz-sdk-errors`).

### Exception Hierarchy

`SzException` is the base, with specific subtypes: `SzBadInputException`, `SzDatabaseException`, `SzNotFoundException`, `SzNotInitializedException`, `SzLicenseException`, etc. Error code → exception mapping is in `SzExceptionMapper.cs` (auto-generated, do not hand-edit).

### Test Structure

- **Senzing.Sdk.Tests/** — NUnit tests (`[TestFixture]`, `[OneTimeSetUp]`/`[OneTimeTearDown]`)
  - `core/` — Tests for each `SzCore*` implementation
  - `AbstractTest` base class manages test repos and tracks pass/fail
- **Senzing.Sdk.Demo/** — Generates code snippet XMLs embedded in API documentation
- **Senzing.Sdk.TestHelpers/** — Shared test utilities

### Build Targets

- **Senzing.Sdk** → `netstandard2.0`, C# 7.3, generates NuGet package (v4.2.0)
- **Senzing.Sdk.Tests** → `net8.0`, NUnit 3.14.0, code coverage via coverlet (85% min, 90% target)
- **Senzing.Sdk.Demo** → `net8.0`, extracts examples for docfx

## Code Style

Enforced by `.editorconfig` and build-time analyzers (`AnalysisMode=All`, `CodeAnalysisTreatWarningsAsErrors=true`). Use `dotnet format` to auto-fix. Style diagnostics are treated as errors.
