# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
[markdownlint](https://dlaa.me/markdownlint/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [4.1.0] - 2025-10-13

### Changed in 4.1.0

- Added `SzConfigRetryable` attribute and applied it to appropriate
  SDK methods.
- Added `SzConfigRetryableTest` to test methods are properly
  annotated.
- Fixed bugs in `SzConfigManager` initialization and error handling.
- Added `SzEnvironmentDestroyedException` so the condition of the `SzEnvironment`
  being destroyed can be distinguished from other occurrences of `InvalidOperationException`.

## [4.0.0] - 2025-08-11

### Changed in 4.0.0

- Updated documentation to include build-time generated example output.
- Applied changes from documentation review to API documentation.
- Updated error-code/exception mapping to handle new error codes for license.
- Updated unit tests to handle `advSearch` license feature.
- Changed `SzCoreEnvironment.Builder` access level to `internal`

## [4.0.0-beta.3.0] - 2025-04-30

### Changed in 4.0.0-beta.3.0

- Added compiled examples in API documentation with `Senzing.Sdk.Demo` module.
- Refactored `SzConfig` and `SzConfigManager` so that `SzConfig` now
  represents a configuration and is subordinate to `SzConfigManager`.
- Added `WhySearch()` functionality and updated flags
- Updated `SzExceptionMapper` with latest exception mappings.

## [4.0.0-beta.2.0] - 2025-02-14

### Changed in 4.0.0-beta.2.0

- Added `SzDatabaseTransientException` class.
- Updated `SzExceptionMapper` class for mapping `SzDatabaseTransientException`

## [4.0.0-beta.1.1] - 2025-02-04

### Changed in 4.0.0-beta.1.1

- Changed versioning to match major version of Senzing 4.0 product with beta suffix.
- Made changes to return `null` when INFO is **not** requested.
- Added full
- Patched `SzCoreEngine.ReevaluateEntity()` to return place-holder `NoInfo` when
  INFO requested, but entity not found (pending fix to native engine function).
- Added new engine flags:
  - `SzSearchIncludeAllCandidates`
  - `SzSearchIncludeRequest`
  - `SzSearchIncludeRequestDetails`
- Added full exception hierarchy:
  - `SzConfigurationException`
  - `SzRetryableException`
    - `SzDatabaseConnectionLostException`
    - `SzRetryTimeoutExceededException`
  - `SzUnrecoverableException`
    - `SzDatabaseException`
    - `SzLicenseException`
    - `SzNotInitializedException`
    - `SzUnhandledException`
- Added full error code to exception mapping from `szerrors.json`  


## [0.9.1] - 2025-01-10

### Changed in 0.9.1

- Minor update to add auto-detection of `SENZING_DIR`

## [0.9.0] - 2024-12-18

- Initial release in preparation for beta testing of Senzing 4.0
