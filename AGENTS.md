Based on https://github.com/microsoft/skills/blob/main/Agents.md (MIT License)

# Agent Guidelines

This file provides instructions for AI coding agents working with this repository.

## Repository Overview

**AuroraScienceHub.Integrations** — .NET clients for third-party space weather data sources (NOAA SWPC, DTU Space).
All packages are published under the `AuroraScienceHub.Integrations.*` namespace and distributed via NuGet.org.

This is a **library repository** shared by several downstream applications (SWeather-core and others).
It produces NuGet packages, not a runnable application. The only executables are samples and benchmarks.

## Language

This is a shared public package, so **everything that lands in the repository or on GitHub is in English**:
code, XML docs, README files, CHANGELOG, commit messages, PR titles and bodies, review comments.
Chat with the user may be in the user's language; that does not change the language of the artifacts.

---

## Core Principles

These principles reduce common LLM coding mistakes. Apply them to every task.

### 1. Think Before Coding

**Don't assume. Don't hide confusion. Surface tradeoffs.**

- State assumptions explicitly. If uncertain, ask.
- If multiple interpretations exist, present them — don't pick silently.
- If a simpler approach exists, say so. Push back when warranted.
- Verify the external source before modelling it: request the live endpoint and look at real payloads, error codes and gaps.

### 2. Simplicity First

**Minimum code that solves the problem. Nothing speculative.**

- No features beyond what was asked.
- No abstractions for single-use code.
- No "flexibility" or "configurability" that wasn't requested.
- No error handling for impossible scenarios.

**The test:** Would a senior engineer say this is overcomplicated? If yes, simplify.

### 3. Surgical Changes

**Touch only what you must. Clean up only your own mess.**

- Don't "improve" adjacent code, comments, or formatting.
- Match existing style, even if you'd do it differently.
- If you notice unrelated dead code or stale docs, mention it — don't change it.
- Remove imports/variables/functions that YOUR changes made unused.

**The test:** Every changed line should trace directly to the user's request.

### 4. Goal-Driven Execution (TDD)

**Define success criteria. Loop until verified.**

| Instead of... | Transform to... |
|---------------|-----------------|
| "Add a client" | "Write tests against a real sample payload, then make them pass" |
| "Fix the bug" | "Write a test that reproduces it, then make it pass" |
| "Refactor X" | "Ensure tests pass before and after" |

---

## Project Structure

```
src/
├── <Source>Client/                  # One NuGet package per data provider (NoaaClient, DtuSpaceClient)
│   ├── <Source>Client.csproj
│   ├── <Source>ClientOptions.cs     # Options bound from the "<Source>" configuration section
│   ├── ServiceCollectionExtensions.cs  # Add<Source>Clients(IServiceCollection, IConfiguration)
│   ├── README.md                    # Package README, shipped inside the nupkg
│   └── <Product>/                   # One folder per data product (Rtsw, KpIndex, PcIndex, ...)
│       ├── I<Product>Client.cs      # Public interface
│       ├── <Product>Client.cs       # internal sealed implementation (typed HttpClient)
│       ├── Responses/               # Public response records
│       └── Extensions/              # internal parsers and helpers
└── Directory.Build.props            # Packable settings: IsPackable, IsAotCompatible, package metadata
tests/
└── UnitTests/                       # xUnit v3 + Moq + Shouldly, mirrors src/ by package and product
samples/
└── <Source>ClientSample/            # Interactive Spectre.Console app for every package
benchmarks/                          # BenchmarkDotNet, only when performance matters and was requested

Directory.Build.props                # Root build properties and PackageBaseVersion
Directory.Packages.props             # Central NuGet version management
Integrations.slnx                    # Solution file
CHANGELOG.md                         # Keep a Changelog
global.json                          # SDK version pinning
```

Each package under `src/` maps to a NuGet package named `AuroraScienceHub.Integrations.<Source>Client`.
Refer to the `README.md` in each package directory for its configuration and usage.

---

## Development Workflow

### Prerequisites

- .NET 11 SDK (see `global.json` for the exact version and roll-forward policy)

### Source of Truth

- `Directory.Build.props`, `src/Directory.Build.props` and `global.json` are authoritative for SDK, target frameworks, nullable, warnings, AOT and packaging.
- If README files or the GitHub Actions workflow diverge from them, follow the build configuration and update the docs or workflow to match.

### Commands

```bash
dotnet restore
dotnet build                             # Warnings are errors, code style is enforced in build
dotnet test                              # Microsoft.Testing.Platform runner
dotnet pack                              # Fails with NU5019 if a package README.md is missing
dotnet format --verify-no-changes        # Run dotnet format before every commit
```

### Branches and PRs

- Branch from an up-to-date `main`; merge `main` into the branch when it moves.
- Every PR bumps the version and updates `CHANGELOG.md` (see [Package Versioning](#package-versioning)).
- A PR body explains why, what changed, limitations and how it was tested.

---

## Adding a Client

Follow the existing `NoaaClient` / `DtuSpaceClient` structure:

1. **Package** — `src/<Source>Client/<Source>Client.csproj` with `AssemblyName` = `PackageId` = `AuroraScienceHub.Integrations.<Source>Client`, per-package `PackageTags`, and a `README.md`. Add it to `Integrations.slnx`.
2. **Options** — `public sealed class <Source>ClientOptions` with `const string OptionKey`, `Uri? ServerUrl`, `bool UseProxy`, and `Uri RequiredServerUrl` that throws a descriptive `ArgumentNullException` when the URL is not configured. No URLs hard-coded in client code; relative paths are constants.
3. **Registration** — `public static Add<Source>Clients(this IServiceCollection, IConfiguration)`: bind the options section and `ProxyOptions`, register each product as a typed client (`AddHttpClient<IClient, Client>()`), and route through `ConfigurePrimaryHttpProxyMessageHandler` when `UseProxy` is set. Annotate generic implementation parameters with `[DynamicallyAccessedMembers(PublicConstructors)]`.
4. **Client** — `public interface I<Product>Client` with XML docs that state the data semantics (time zone, cadence, lag, missing values, what an empty result means); `internal sealed class <Product>Client`. Every method takes a `CancellationToken`, uses `ConfigureAwait(false)` and `EnsureSuccess()` from `AuroraScienceHub.Framework.Http`.
5. **Responses** — `public sealed record` types in `Responses/`; times in UTC (`DateTimeKind.Utc`); missing provider values (`NaN`, fill values) map to `null`.
6. **Parsing** — prefer BCL (`System.Text.Json` source generation, span-based parsing) over new dependencies. Parsers are `internal static`, tolerant to the provider's quirks (CRLF, trailing commas, malformed rows) and covered by tests on real payloads.
7. **Tests** — see [Testing](#testing).
8. **Sample** — `samples/<Source>ClientSample` modelled on `NoaaClientSample`: Spectre.Console menu with one entry per method, "Execute All Requests", `appsettings.json`, `README.md`. Add it to `Integrations.slnx`.
9. **Docs** — root `README.md` (section and `dotnet add package` line), package `README.md` (configuration, usage, semantics, data license), `CHANGELOG.md`.

### NativeAOT

Packages are `IsAotCompatible` (set in `src/Directory.Build.props`), and AOT/trim warnings break the build:

- JSON: `System.Text.Json` source generation via an internal `JsonSerializerContext`; no reflection-based serialization.
- Configuration: binding through `EnableConfigurationBindingGenerator`.
- No `dynamic`, no reflection where generics or interfaces suffice.

---

## Conventions

### Project settings (inherited from `Directory.Build.props`)

- **Target frameworks:** `net10.0;net11.0` for packages; tests, samples and benchmarks on `net11.0`
- **Runtime async:** enabled for `net11.0`
- **Language version:** `latest`; nullable and implicit usings enabled
- **Warnings as errors** and `EnforceCodeStyleInBuild` — `.editorconfig` violations break the build
- **PDB:** embedded, all sources embedded
- **License:** MIT

### API Design

- Follow the [.NET Library Design Guidelines](https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/).
- `internal sealed` by default; expose only interfaces, options, response records and the registration extension.
- XML documentation on all public types and members.
- Public APIs are versioned contracts for downstream consumers: prefer additive changes, evaluate source/binary compatibility, and never rename, remove or change the behavior of a published API without explicit approval. Breaking changes are marked **Breaking:** in `CHANGELOG.md`.
- Method names describe the data, not the transport or the provider's file naming (`GetPcnAsync`, not `GetPcnQuickLookCsvAsync`).

### Dependencies

- All package versions live in `Directory.Packages.props`; never put `Version` on a `PackageReference`.
- Minimize external dependencies; prefer BCL types. Justify every new dependency in the PR.

---

## Package Versioning

- The base version lives in **one place**: `Directory.Build.props` → `<PackageBaseVersion>`.
- **Every PR bumps it** following SemVer: new functionality → minor (`1.5.0 → 1.6.0`), fixes → patch (`1.2.1 → 1.2.2`).
- CI computes the package version from `PackageBaseVersion` and passes it via `-p:MinVerVersionOverride`; never set `<Version>` or `<PackageVersion>` in a `.csproj`.

### CHANGELOG

Follows [Keep a Changelog](https://keepachangelog.com/en/1.0.0/):

- The version in progress is a section `## [X.Y.Z] - Unreleased`, where `X.Y.Z` equals `PackageBaseVersion`. Add every user-visible change to it under `Added` / `Changed` / `Fixed` / `Removed`, grouped by package (`#### <Source>Client Package`, `#### Dependencies`).
- Replace `Unreleased` with the release date (`YYYY-MM-DD`) when the PR is merged to `main`.
- The repository has no release tags; the link at the bottom of the file points to the merge commit of each version (`compare/<previous merge commit>...main` for the version in progress).

---

## Testing

- **Framework:** xUnit v3 on Microsoft.Testing.Platform
- **Assertions:** Shouldly
- **Mocking:** Moq; `FakeTimeProvider` for time

### Conventions

- Tests mirror `src/`: `tests/UnitTests/<Source>Client/<Product>/<Class>Tests.cs`.
- Test method naming: `{Method}_{Scenario}_{ExpectedBehavior}`, with a readable `[Fact(DisplayName = "...")]`.
- Arrange-Act-Assert with `// Arrange`, `// Act`, `// Assert` comments.
- HTTP is stubbed with a test `HttpMessageHandler`; tests never call real servers.
- Parsers are tested against real provider payloads stored as embedded resources (`<Product>/Samples/`, registered in `UnitTests.csproj`), including the provider's quirks.
- Cover the error contract: URL building, success, "no data" responses (`404`, empty body), and HTTP errors that must throw.
- Use `TestContext.Current.CancellationToken` for async calls.

---

## CI/CD

GitHub Actions workflow (`.github/workflows/dotnet.yml`):

| Trigger | Behavior |
|---------|----------|
| Push to `main` | Build → Test → Pack → Push stable `X.Y.Z.<run>` to **NuGet.org** (Trusted Publishing via OIDC) |
| Push to other branches | Build → Test → Pack → Push pre-release `X.Y.Z-<branch>.<run>` to **GitHub Packages** |
| Semver tag (`*.*.*`) | Produces stable `X.Y.Z` packages |

A downstream repository can consume the branch pre-release to verify a change before merge; switch it to the stable version once the PR is merged.

---

## Do's and Don'ts

### Do

- ✅ Follow the existing client structure and naming
- ✅ Check the live provider before modelling its data
- ✅ Bump `PackageBaseVersion` and update `CHANGELOG.md` in every PR
- ✅ Write tests and a sample for every new client method
- ✅ Keep packages focused — one data provider per package
- ✅ Run `dotnet format` before committing

### Don't

- ❌ Write repository artifacts (docs, commits, PRs, reviews) in a language other than English
- ❌ Hard-code server URLs outside options and tests
- ❌ Suppress warnings with `#pragma` or `<NoWarn>` without justification
- ❌ Add `Version` attributes to individual `.csproj` files
- ❌ Add dependencies that duplicate BCL functionality
- ❌ Leave empty catch blocks or swallow exceptions silently
- ❌ Break public API contracts without explicit approval
