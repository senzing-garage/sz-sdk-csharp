# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
[markdownlint](https://dlaa.me/markdownlint/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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
