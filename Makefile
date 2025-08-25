
# -----------------------------------------------------------------------------
# The first "make" target runs as default.
# -----------------------------------------------------------------------------
.PHONY: default
default: help

# -----------------------------------------------------------------------------
# Builds the example snippets so they can be included in the docs XML
# This will build the Senzing.Sdk module without generating the NuGet file
# -----------------------------------------------------------------------------
.PHONY: demo
demo:
	dotnet test Senzing.Sdk.Demo -p:GeneratePackageOnBuild=false

# -----------------------------------------------------------------------------
# Builds Senzing.Sdk for debug including the documentation XML and the NuGet
# package file.  This depends on "demo" target for the example snippets for
# generating the documentation XML file.
# -----------------------------------------------------------------------------
.PHONY: debug
debug: demo
	dotnet build Senzing.Sdk -p:GenerateDocumentationFile=true

# -----------------------------------------------------------------------------
# Builds Senzing.Sdk for release including the documentation XML and the NuGet
# package file.  This depends on "demo" target for the example snippets for
# generating the documentation XML file.
# -----------------------------------------------------------------------------
.PHONY: release
release: demo
	dotnet build -c Release Senzing.Sdk -p:GenerateDocumentationFile=true

# -----------------------------------------------------------------------------
# Runs the unit tests in the Senzing.Sdk.Tests package.  This depends on the
# "debug" target to generate the Senzing.Sdk binaries.
# -----------------------------------------------------------------------------
.PHONY: test
test:
	dotnet test Senzing.Sdk.Tests

# -----------------------------------------------------------------------------
# Runs formatting against Senzing.Sdk
# -----------------------------------------------------------------------------
.PHONY: format-sdk
format-sdk:
	dotnet format --verify-no-changes --verbosity diagnostic Senzing.Sdk

# -----------------------------------------------------------------------------
# Runs formatting against Senzing.Sdk.Demo
# -----------------------------------------------------------------------------
.PHONY: format-demo
format-demo:
	dotnet format --verify-no-changes --verbosity diagnostic Senzing.Sdk.Demo

# -----------------------------------------------------------------------------
# Runs formatting against Senzing.Sdk.Tests
# -----------------------------------------------------------------------------
.PHONY: format-test
format-test:
	dotnet format --verify-no-changes --verbosity diagnostic Senzing.Sdk.Tests
	dotnet format --verify-no-changes --verbosity diagnostic Senzing.Sdk.TestHelpers

# -----------------------------------------------------------------------------
# Runs formatting against all modules
# -----------------------------------------------------------------------------
.PHONY: format
format: format-sdk format-demo format-test

# -----------------------------------------------------------------------------
# Generates the HTML documentation. This depends on the "debug" target to 
# generate the documentation XML file.
# -----------------------------------------------------------------------------
.PHONY: docs
docs: debug
	dotnet docfx docfx.json

# -----------------------------------------------------------------------------
# The clean target since "dotnet clean" never seems to get everything
# -----------------------------------------------------------------------------
clean:
	@dotnet clean Senzing.Sdk
	@dotnet clean -c Release Senzing.Sdk
	@dotnet clean Senzing.Sdk.Tests
	@dotnet clean Senzing.Sdk.TestHelpers
	@dotnet clean Senzing.Sdk.Demo
	@rm -rf Senzing.Sdk/bin
	@rm -rf Senzing.Sdk/obj
	@rm -rf Senzing.Sdk.Tests/bin
	@rm -rf Senzing.Sdk.Tests/obj
	@rm -rf Senzing.Sdk.TestHelpers/bin
	@rm -rf Senzing.Sdk.TestHelpers/obj
	@rm -rf Senzing.Sdk.Demo/bin
	@rm -rf Senzing.Sdk.Demo/obj
	@rm -rf target


# -----------------------------------------------------------------------------
# The help target for the Makefile
# -----------------------------------------------------------------------------
.PHONY: help
help:
	@echo "Makefile targets:"
	@echo "help        : Prints this message"
	@echo "demo        : Generates the example snippets for the docs XML generation"
	@echo "debug       : Builds Senzing.Sdk with debug profile (including docs XML and NuGet package)"
	@echo "release     : Builds Senzing.Sdk with release profile (including docs XML and NuGet package)"
	@echo "test        : Runs unit tests in the Senzing.Sdk.Tests module"
	@echo "format-sdk  : Runs formatting check on Senzing.Sdk"
	@echo "format-test : Runs formatting check on Senzing.Sdk.Tests"
	@echo "format-demo : Runs formatting check on Senzing.Sdk.Demo"
	@echo "format      : Runs formatting on all modules"
	@echo "docs        : Generates the HTML documentation files from the XML"
	@echo "clean       : Cleans up all generated artifacts (including target directory)"
