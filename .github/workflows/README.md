# Workflows

## add-labels-standardized.yaml

When issues are opened,
this action adds appropriate labels to the issue.
(e.g. "triage", "customer-submission")

- [Add Labels Standardized GitHub Action]
  - Uses: [senzing-factory/build-resources/.../add-labels-to-issue.yaml]

## add-to-project-garage-dependabot.yaml

When a Dependabot Pull Request (PR) is made against `main` branch,
this action adds the PR to the "Garage" project board as "In Progress".

- [Add to Project Garage Dependabot GitHub Action]
  - Uses: [senzing-factory/build-resources/.../add-to-project-dependabot.yaml]

## add-to-project-garage.yaml

When an issue is created,
this action adds the issue to the "Garage" board as "Backlog".

- [Add to Project Garage GitHub Action]
  - Uses: [senzing-factory/build-resources/.../add-to-project.yaml]

## dependabot-approve-and-merge.yaml

When a Dependabot Pull Request (PR) is made against the `main` branch,
this action determines if it should be automatically approved and merged into the `main` branch.
Once this action occurs [move-pr-to-done-dependabot.yaml] moves the PR on the "Garage" project board to "Done".

- [Dependabot Approve and Merge GitHub Action]
  - Uses: [senzing-factory/build-resources/.../dependabot-approve-and-merge.yaml]

## dotnet-csharpier.yaml

When a change is committed to GitHub or a Pull Request is opened against the main branch, this action runs [csharpier].

- [dotnet-csharpier.yaml]
  - Uses:
    - [actions/checkout]
    - [actions/setup-dotnet]
    - [csharpier]

## dotnet-darwin.yaml

When a Pull Request is opened against the main branch, this action runs `dotnet test` on the Darwin/macOS platform to perform unit tests and code coverage.

- [dotnet-darwin.yaml]
  - Uses:
    - [actions/checkout]
    - [actions/setup-dotnet]
    - [senzing-factory/github-action-install-senzing-api]
    - [actions/upload-artifact]
    - [senzing-factory/build-resources/.../csharp-coverage-comment.yaml]

## dotnet-format.yaml

When a change is committed to GitHub or a Pull Request is opened against the main branch, this action runs [dotnet format].

- [dotnet-format.yaml]
  - Uses:
    - [actions/checkout]
    - [actions/setup-dotnet]

## dotnet-linux.yaml

When a change is committed to GitHub or a Pull Request is opened against the main branch, this action runs `dotnet test` on the Linux platform to perform unit tests and code coverage.

- [dotnet-linux.yaml]
  - Uses:
    - [actions/checkout]
    - [actions/setup-dotnet]
    - [senzing-factory/github-action-install-senzing-api]
    - [actions/upload-artifact]
    - [senzing-factory/build-resources/.../csharp-coverage-comment.yaml]

## dotnet-windows.yaml

When a Pull Request is opened against the main branch, this action runs `dotnet test` on the Darwin/macOS platform to perform unit tests and code coverage.

- [dotnet-windows.yaml]
  - Uses:
    - [actions/checkout]
    - [actions/setup-dotnet]
    - [senzing-factory/github-action-install-senzing-api]
    - [actions/upload-artifact]
    - [senzing-factory/build-resources/.../csharp-coverage-comment.yaml]

## lint-workflows.yaml

When a change is committed to GitHub or a Pull Request is made against the `main` branch,
this action runs [super-linter] to run multiple linters against the code.

- [Lint Workflows GitHub Action]
  - Configuration:
    - [.jscpd.json]
    - [.yaml-lint.yml]
  - Uses: [senzing-factory/build-resources/.../lint-workflows.yaml]

## move-pr-to-done-dependabot.yaml

When a Pull Request is merged into the `main` branch,
this action moves the PR on the "Garage" project board to "Done".

- [Move PR to Done Dependabot GitHub Action]
  - Uses: [senzing-factory/build-resources/.../move-pr-to-done-dependabot.yaml]

[.jscpd.json]: ../linters/README.md#jscpdjson
[.yaml-lint.yml]: ../linters/README.md#yaml-lintyml
[actions/checkout]: https://github.com/actions/checkout
[actions/setup-dotnet]: https://github.com/actions/setup-dotnet
[actions/upload-artifact]: https://github.com/actions/upload-artifact
[Add Labels Standardized GitHub Action]: add-labels-standardized.yaml
[Add to Project Garage Dependabot GitHub Action]: add-to-project-garage-dependabot.yaml
[Add to Project Garage GitHub Action]: add-to-project-garage.yaml
[csharpier]: https://csharpier.com/docs/ContinuousIntegration
[Dependabot Approve and Merge GitHub Action]: dependabot-approve-and-merge.yaml
[dotnet-csharpier.yaml]: dotnet-csharpier.yaml
[dotnet-darwin.yaml]: dotnet-darwin.yaml
[dotnet-format.yaml]: dotnet-format.yaml
[dotnet-linux.yaml]: dotnet-linux.yaml
[dotnet-windows.yaml]: dotnet-windows.yaml
[dotnet format]: https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-format
[Lint Workflows GitHub Action]: lint-workflows.yaml
[Move PR to Done Dependabot GitHub Action]: move-pr-to-done-dependabot.yaml
[move-pr-to-done-dependabot.yaml]: move-pr-to-done-dependabotyaml
[senzing-factory/build-resources/.../add-labels-to-issue.yaml]: https://github.com/senzing-factory/build-resources/blob/main/.github/workflows/add-labels-to-issue.yaml
[senzing-factory/build-resources/.../add-to-project-dependabot.yaml]: https://github.com/senzing-factory/build-resources/blob/main/.github/workflows/add-to-project-dependabot.yaml
[senzing-factory/build-resources/.../add-to-project.yaml]: https://github.com/senzing-factory/build-resources/blob/main/.github/workflows/add-to-project.yaml
[senzing-factory/build-resources/.../csharp-coverage-comment.yaml]: https://github.com/senzing-factory/build-resources/blob/main/.github/workflows/csharp-coverage-comment.yaml
[senzing-factory/build-resources/.../dependabot-approve-and-merge.yaml]: https://github.com/senzing-factory/build-resources/blob/main/.github/workflows/dependabot-approve-and-merge.yaml
[senzing-factory/build-resources/.../lint-workflows.yaml]: https://github.com/senzing-factory/build-resources/blob/main/.github/workflows/lint-workflows.yaml
[senzing-factory/build-resources/.../move-pr-to-done-dependabot.yaml]: https://github.com/senzing-factory/build-resources/blob/main/.github/workflows/move-pr-to-done-dependabot.yaml
[senzing-factory/github-action-install-senzing-api]: https://github.com/senzing-factory/github-action-install-senzing-api
[super-linter]: https://github.com/super-linter/super-linter
