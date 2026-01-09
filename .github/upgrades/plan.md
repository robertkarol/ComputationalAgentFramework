# .NET 10.0 Upgrade Plan

## Table of Contents

- [Executive Summary](#executive-summary)
- [Migration Strategy](#migration-strategy)
- [Detailed Dependency Analysis](#detailed-dependency-analysis)
- [Implementation Timeline](#implementation-timeline)
- [Project-by-Project Migration Plans](#project-by-project-migration-plans)
  - [ComputationalAgentFramework.csproj](#computationalagentframeworkcsproj)
  - [Examples\Examples.csproj](#examplesexamplescsproj)
- [Breaking Changes Catalog](#breaking-changes-catalog)
- [Risk Management](#risk-management)
- [Testing & Validation Strategy](#testing--validation-strategy)
- [Complexity & Effort Assessment](#complexity--effort-assessment)
- [Source Control Strategy](#source-control-strategy)
- [Success Criteria](#success-criteria)

---

## Executive Summary

### Scenario Description

This plan outlines the migration of the ComputationalAgentFramework solution from **.NET Framework 4.7.2** to **.NET 10.0 (Long Term Support)**. The solution consists of 2 projects requiring conversion from classic .NET Framework project format to modern SDK-style projects.

### Scope

**Projects Affected:** 2
- `ComputationalAgentFramework.csproj` - Core library (475 LOC)
- `Examples\Examples.csproj` - Example/demo application (192 LOC)

**Current State:**
- All projects targeting .NET Framework 4.7.2 (net472)
- Classic (non-SDK-style) project format
- Zero NuGet package dependencies
- Clean API compatibility (340 APIs analyzed, all compatible)

**Target State:**
- All projects targeting .NET 10.0 (net10.0)
- Modern SDK-style project format
- Full compatibility with modern .NET ecosystem

### Complexity Assessment

**Classification:** ?? **Simple Solution**

**Discovered Metrics:**
- **Project Count:** 2 projects
- **Dependency Depth:** 1 level (linear: Examples ? ComputationalAgentFramework)
- **Total Codebase:** 667 lines of code
- **Risk Level:** Low
  - ? No security vulnerabilities
  - ? No package compatibility issues (0 external packages)
  - ? All APIs compatible (100% compatibility rate)
  - ? No circular dependencies
  - ? Straightforward dependency structure

### Selected Strategy

**All-At-Once Strategy** - All projects upgraded simultaneously in a single coordinated operation.

**Rationale:**
1. **Small Solution Size:** Only 2 projects (well below 5-project threshold)
2. **Simple Dependency Structure:** Linear dependency chain with no complexity
3. **Zero Package Dependencies:** No external package upgrade coordination required
4. **100% API Compatibility:** All 340 APIs analyzed are compatible with .NET 10.0
5. **Low Risk Profile:** No high-risk indicators identified
6. **Efficient Timeline:** Fastest path to completion without intermediate states

### Critical Issues

? **No Critical Issues Identified**

- No security vulnerabilities detected
- No breaking API changes
- No incompatible packages
- No complex refactoring requirements

### Recommended Approach

Execute a single atomic upgrade operation:
1. Convert both projects to SDK-style format simultaneously
2. Update target frameworks to net10.0 in coordinated fashion
3. Build and validate entire solution as single unit
4. Single source control commit for complete upgrade

### Iteration Strategy

**Fast Batch Approach** - 2-3 detail iterations covering all projects together due to simple structure and low complexity.

**Expected Remaining Iterations:** 4-5 iterations total
- Phase 2: Foundation (3 iterations - dependency analysis, strategy, project stubs)
- Phase 3: Detailed specifications (1-2 iterations - batch both projects together)

---

## Migration Strategy

### Approach Selection

**Selected Strategy: All-At-Once**

All projects in the solution will be upgraded simultaneously in a single coordinated operation. Both project files will be converted to SDK-style format and updated to target .NET 10.0 at the same time, creating a unified upgrade without intermediate states.

### Justification

#### Why All-At-Once is Optimal

**Solution Characteristics Favor All-At-Once:**

1. **Small Scale:** 2 projects (well below 30-project threshold for All-At-Once)
2. **Simple Dependency Structure:** Single linear dependency chain with no complexity
3. **Homogeneous Codebase:** Both projects are .NET Framework 4.7.2, same starting point
4. **Zero External Dependencies:** No NuGet packages to coordinate upgrades
5. **100% API Compatibility:** All 340 APIs analyzed are compatible - no breaking changes expected
6. **Low Total Codebase:** 667 LOC total (well under 50k threshold)

**Risk Assessment:**
- ? Low initial risk due to API compatibility
- ? Small testing surface (2 projects, 19 files)
- ? No multi-targeting complexity needed
- ? Fast completion timeline
- ? Clean dependency resolution (no external packages)

**Not Recommended Conditions (None Apply):**
- ? Large solutions (30+ projects) - We have 2 projects
- ? Mix of .NET Framework and modern .NET - Both are .NET Framework 4.7.2
- ? Limited test coverage - Can be validated quickly with small codebase
- ? High external dependency complexity - Zero external packages

### All-At-Once Strategy Rationale and Considerations

**Strategic Approach:**
- **Atomic Operation:** All project files converted and updated in single coordinated batch
- **No Intermediate States:** Solution moves directly from net472 to net10.0
- **Unified Testing:** Single comprehensive validation after all changes complete
- **Single Commit:** All changes committed together for clean history

**Advantages in This Context:**
- ? **Fastest Completion:** No phased rollout delays
- ? **Simple Coordination:** All developers work with same target at same time
- ? **Clean Dependency Resolution:** No multi-targeting confusion
- ? **Reduced Complexity:** No need to maintain compatibility with intermediate versions
- ? **Clear Success Criteria:** Solution either builds completely or doesn't

**Challenges (Minimal for This Solution):**
- Small coordination window during conversion (mitigated by small team/codebase)
- All tests must pass together (mitigated by low complexity)
- Entire solution temporarily non-functional during conversion (mitigated by fast operation)

### Dependency-Based Ordering Principles

While All-At-Once executes simultaneously, internal operation order respects dependencies:

**Logical Operation Sequence:**
1. SDK-style conversion (both projects prepared)
2. Framework target updates (both projects updated)
3. Dependency restoration (solution-wide)
4. Build validation (entire solution)

**Dependency Awareness:**
- ComputationalAgentFramework has no dependencies ? Can be converted independently
- Examples depends on ComputationalAgentFramework ? Conversion aware of dependency
- Both converted in same operation ? Dependency relationship maintained throughout

### Execution Model

**Parallel Processing:**
- Project file conversions can occur simultaneously (no sequential requirement)
- Framework updates applied to both projects in parallel
- Build and validation performed on complete solution

**Sequential Validation:**
1. All project files converted and updated
2. Solution restored (all dependencies resolved)
3. Solution built (all projects compile together)
4. Tests executed (if available)
5. Success criteria validated

### Phase Definitions

Given the All-At-Once approach and simple structure, migration is organized into logical phases for clarity, not as separate task boundaries:

#### Phase 0: Preparation
- Verify .NET 10.0 SDK installed
- Backup current state (already on `upgrade-to-NET10` branch)
- Document pre-migration baseline

#### Phase 1: Atomic Upgrade
- **Scope:** All projects simultaneously
- **Operations:**
  - Convert both projects to SDK-style format
  - Update both TargetFramework properties to net10.0
  - Remove obsolete classic project elements (AssemblyInfo, packages.config if present)
  - Restore dependencies
  - Build entire solution
  - Fix any compilation errors discovered
- **Deliverable:** Solution builds with 0 errors

#### Phase 2: Validation
- **Scope:** Complete solution
- **Operations:**
  - Execute any existing tests
  - Validate Examples application runs correctly
  - Verify API surface area maintained
  - Confirm no warnings or issues
- **Deliverable:** All validation criteria pass

### Risk Management Alignment

**All-At-Once Risk Factors (All Low):**
- ? Solution size: 2 projects (low risk)
- ? API compatibility: 100% (low risk)
- ? Package dependencies: 0 (no risk)
- ? Codebase size: 667 LOC (low risk)
- ? Dependency complexity: Single chain (low risk)

**Mitigation Strategies:**
- Git branch isolation (`upgrade-to-NET10`) allows easy rollback
- Small codebase enables quick validation
- Zero external packages eliminates version conflict risks
- 100% API compatibility minimizes code changes

---

## Implementation Timeline

### Phase 0: Preparation

**Duration:** Quick validation (minutes)

**Prerequisites:**
- ? .NET 10.0 SDK installed on development machine
- ? Git branch `upgrade-to-NET10` created and active
- ? Solution baseline documented in assessment.md

**Activities:**
- Verify SDK installation: `dotnet --list-sdks` (should show 10.0.x)
- Confirm branch isolation
- Document current solution state

**Deliverables:**
- Environment ready for upgrade
- Baseline established

---

### Phase 1: Atomic Upgrade

**Scope:** All projects upgraded simultaneously

**Operations** (performed as single coordinated batch):
1. **Convert to SDK-Style** - Both projects converted from classic format to modern SDK-style
2. **Update Target Frameworks** - Both projects updated from net472 to net10.0
3. **Clean Up Legacy Elements** - Remove classic project artifacts (AssemblyInfo attributes duplicated by SDK)
4. **Restore Dependencies** - Solution-wide dependency restoration
5. **Build Solution** - Complete solution compilation
6. **Fix Compilation Errors** - Address any issues discovered during build

**Projects Included:**
- ComputationalAgentFramework.csproj (core library)
- Examples\Examples.csproj (demo application)

**Deliverables:**
- ? Both projects use SDK-style format
- ? Both projects target net10.0
- ? Solution builds with 0 errors
- ? Solution builds with 0 warnings

---

### Phase 2: Validation

**Scope:** Complete solution testing and verification

**Operations:**
1. **Build Verification** - Confirm clean build across solution
2. **Application Testing** - Run Examples application and verify functionality
3. **API Validation** - Confirm ComputationalAgentFramework API surface maintained
4. **Warning Check** - Verify no build warnings or runtime issues

**Deliverables:**
- ? Examples application runs successfully
- ? ComputationalAgentFramework API functional
- ? No errors, warnings, or runtime issues
- ? All validation criteria met

---

### Phase 3: Finalization

**Operations:**
1. **Documentation Update** - Update README.md with new framework requirements
2. **Commit Changes** - Single commit containing all upgrade changes
3. **Branch Review** - Verify all changes captured correctly

**Deliverables:**
- ? Documentation reflects .NET 10.0 requirements
- ? Clean commit history with descriptive message
- ? Branch ready for review/merge

---

## Project-by-Project Migration Plans

### ComputationalAgentFramework.csproj

**Project Type:** ClassicClassLibrary  
**Current State:** net472, Classic project format  
**Target State:** net10.0, SDK-style project format  

**Metrics:**
- Lines of Code: 475
- Files: 14
- Dependencies: 0 (leaf node)
- Dependants: 1 (Examples.csproj)
- NuGet Packages: 0
- API Compatibility: 245 APIs analyzed, all compatible

**Risk Level:** ?? Low

#### Migration Steps

##### 1. Prerequisites

**Required Tools:**
- .NET 10.0 SDK installed
- Visual Studio 2022 (version 17.12+) or compatible editor
- Git for source control

**Pre-Migration Checklist:**
- [ ] Verify project builds successfully on net472
- [ ] Document current project structure
- [ ] Backup original .csproj file (via Git)

##### 2. SDK-Style Conversion

**Operation:** Convert classic .NET Framework project to SDK-style format

**Approach:**
Use automated SDK-style conversion tool, then manually review and adjust.

**Expected Changes:**
- Replace verbose classic .csproj XML with concise SDK-style format
- Remove explicit file includes (SDK-style uses globbing by default)
- Remove `packages.config` if present (SDK-style uses PackageReference inline)
- Simplify project references

**Classic Project Elements to Remove:**
```xml
<!-- These are auto-generated by SDK-style and should be removed from AssemblyInfo.cs -->
[assembly: AssemblyVersion]
[assembly: AssemblyFileVersion]
[assembly: AssemblyTitle]
[assembly: AssemblyDescription]
[assembly: AssemblyCompany]
[assembly: AssemblyProduct]
[assembly: AssemblyCopyright]
```

---

### Examples\Examples.csproj

**Project Type:** ConsoleApplication
**Current State:** net472, Classic project format
**Target State:** net10.0, SDK-style project format

**Metrics:**
- Lines of Code: 192
- Files: 5
- Dependencies: 1 (ComputationalAgentFramework)
- Dependants: 0
- NuGet Packages: 0
- API Compatibility: 95 APIs analyzed, all compatible

**Risk Level:** ?? Low

#### Migration Steps

##### 1. Prerequisites

**Required Tools:**
- .NET 10.0 SDK installed
- Visual Studio 2022 (version 17.12+) or compatible editor
- Git for source control

**Pre-Migration Checklist:**
- [ ] Verify project builds successfully on net472
- [ ] Document current project structure
- [ ] Backup original .csproj file (via Git)

##### 2. SDK-Style Conversion

**Operation:** Convert classic .NET Framework project to SDK-style format

**Approach:**
Use automated SDK-style conversion tool, then manually review and adjust.

**Expected Changes:**
- Replace verbose classic .csproj XML with concise SDK-style format
- Remove explicit file includes (SDK-style uses globbing by default)
- Remove `packages.config` if present (SDK-style uses PackageReference inline)
- Simplify project references

**Classic Project Elements to Remove:**
```xml
<!-- These are auto-generated by SDK-style and should be removed from AssemblyInfo.cs -->
[assembly: AssemblyVersion]
[assembly: AssemblyFileVersion]
[assembly: AssemblyTitle]
[assembly: AssemblyDescription]
[assembly: AssemblyCompany]
[assembly: AssemblyProduct]
[assembly: AssemblyCopyright]
```

---

## Breaking Changes Catalog

### Summary

**Total Breaking Changes Expected:** 0

The assessment analysis confirms 100% API compatibility across all 340 APIs analyzed in the solution. No binary incompatible, source incompatible, or behavioral change issues were detected.

### Assessment Analysis Results

| Project | APIs Analyzed | Binary Incompatible | Source Incompatible | Behavioral Changes | Compatible |
|---------|---------------|---------------------|---------------------|-------------------|------------|
| ComputationalAgentFramework.csproj | 245 | 0 | 0 | 0 | 245 (100%) |
| Examples.csproj | 95 | 0 | 0 | 0 | 95 (100%) |
| **Total** | **340** | **0** | **0** | **0** | **340 (100%)** |

### Framework-Level Changes (.NET Framework 4.7.2 ? .NET 10.0)

While no API-level breaking changes were detected, developers should be aware of general framework differences:

#### 1. Runtime Platform Change

**Change:** Moving from .NET Framework (Windows-only) to modern .NET (cross-platform)

**Impact:** Low - code remains compatible, but deployment context changes
- .NET Framework runs on Windows only
- .NET 10.0 runs on Windows, Linux, macOS

**Action Required:** None for codebase - awareness only

#### 2. Default Encodings

**Change:** Default encoding in .NET 10.0 is UTF-8 without BOM (vs. platform-dependent in .NET Framework)

**Impact:** Low - affects file I/O, text operations
**Likelihood in This Solution:** Unknown (depends on file operations in code)

**Action Required:** Review file I/O operations if any

#### 3. Culture and Globalization

**Change:** Globalization implementation differs (ICU-based on .NET vs. Windows NLS on .NET Framework)

**Impact:** Low - affects string comparison, sorting, formatting
**Likelihood in This Solution:** Low (no globalization-specific code identified)

**Action Required:** Test if solution uses culture-sensitive operations

#### 4. Nullable Reference Types

**Change:** .NET 10.0 SDK enables nullable reference type warnings by default (if `<Nullable>enable</Nullable>`)

**Impact:** Low - compilation warnings only, not runtime breaking changes
**Likelihood:** Medium (modern SDK encourages nullable annotations)

**Action Required:** 
- Option 1: Address nullable warnings gradually
- Option 2: Disable initially with `<Nullable>disable</Nullable>`

#### 5. App.config vs. runtimeconfig.json

**Change:** Runtime configuration moves from App.config `<startup>` section to auto-generated `runtimeconfig.json`

**Impact:** Low - SDK handles automatically
**Projects Affected:** Examples.csproj

**Action Required:** Remove `<startup>` section from App.config (see Examples project migration plan)

### SDK-Style Conversion Changes

#### AssemblyInfo.cs Attribute Duplication

**Change:** SDK-style projects auto-generate assembly attributes from project properties

**Impact:** Medium - causes build errors if duplicated
**Projects Affected:** Both projects

**Action Required:** Remove duplicate attributes from `Properties\AssemblyInfo.cs` (detailed in per-project plans)

**Attributes to Remove:**
- `AssemblyTitle`, `AssemblyDescription`, `AssemblyCompany`, `AssemblyProduct`
- `AssemblyCopyright`, `AssemblyVersion`, `AssemblyFileVersion`

**Attributes to Keep:**
- `InternalsVisibleTo`, `CLSCompliant`, custom attributes

### Package-Level Breaking Changes

**None** - Zero NuGet packages in solution, so no package-related breaking changes.

### Confirmed Non-Issues

? **No API Removals:** All APIs used in the solution are available in .NET 10.0  
? **No Signature Changes:** No method signature or parameter changes detected  
? **No Namespace Moves:** All namespaces remain consistent  
? **No Obsolete APIs:** No deprecated API usage detected  
? **No Package Conflicts:** Zero package dependencies eliminate conflict risks

---

## Testing & Validation Strategy

### Multi-Level Testing Approach

Given the solution's characteristics (small size, no test projects), the validation strategy focuses on build validation and manual application testing.

### Level 1: Per-Project Validation

Performed immediately after each project conversion:

#### ComputationalAgentFramework.csproj Validation

**Build Validation:**
```bash
# Build project individually
dotnet build ComputationalAgentFramework.csproj --configuration Debug
dotnet build ComputationalAgentFramework.csproj --configuration Release

# Expected: Build succeeds with 0 errors
```

**Success Criteria:**
- [ ] Builds without errors
- [ ] Builds without warnings (or only nullable warnings)
- [ ] Assembly generated: `bin\Debug\net10.0\ComputationalAgentFramework.dll`
- [ ] Assembly generated: `bin\Release\net10.0\ComputationalAgentFramework.dll`

**API Surface Validation:**
- Public types accessible
- No missing dependencies
- Framework references resolve correctly

#### Examples.csproj Validation

**Build Validation:**
```bash
# Build project individually
dotnet build Examples\Examples.csproj --configuration Debug
dotnet build Examples\Examples.csproj --configuration Release

# Expected: Build succeeds with 0 errors
```

**Success Criteria:**
- [ ] Builds without errors
- [ ] Builds without warnings
- [ ] Executable generated: `bin\Debug\net10.0\Examples.exe`
- [ ] ProjectReference to ComputationalAgentFramework resolves
- [ ] Dependency assembly copied to output

### Level 2: Solution-Wide Validation

Performed after all projects converted (All-At-Once atomic operation):

**Solution Build:**
```bash
# Build entire solution
dotnet build ComputationalAgentFramework.sln --configuration Debug
dotnet build ComputationalAgentFramework.sln --configuration Release

# Expected: All projects build successfully
```

**Success Criteria:**
- [ ] Solution builds without errors
- [ ] Solution builds without warnings
- [ ] All project dependencies resolved
- [ ] Build output clean (no unexpected messages)

**Restore Validation:**
```bash
# Verify dependency resolution
dotnet restore ComputationalAgentFramework.sln

# Expected: Restores successfully (even with zero packages)
```

### Level 3: Functional Validation

Manual testing of the Examples application to validate ComputationalAgentFramework functionality:

**Execution Testing:**
```bash
# Run Examples application
cd Examples\bin\Debug\net10.0
Examples.exe

# Or using dotnet run
dotnet run --project Examples\Examples.csproj
```

**Test Scenarios:**
1. **Application Startup:** Verify application starts without errors
2. **Framework Integration:** Confirm ComputationalAgentFramework loads correctly
3. **Demo Scenarios:** Execute all example scenarios provided in the application
4. **Expected Output:** Verify output matches pre-migration behavior
5. **Error Handling:** Confirm no unhandled exceptions or crashes

**Success Criteria:**
- [ ] Application starts successfully
- [ ] No runtime errors or exceptions
- [ ] All demo scenarios execute
- [ ] Output behavior consistent with net472 version
- [ ] ComputationalAgentFramework API calls functional

### Level 4: Quality Validation

**Code Quality Checks:**
- [ ] No new compiler warnings introduced
- [ ] AssemblyInfo.cs files cleaned (no duplicate attributes)
- [ ] Project files minimal and clean (SDK-style benefits)
- [ ] No unnecessary dependencies or references

**Configuration Validation:**
- [ ] App.config correctly migrated (Examples project)
- [ ] Runtime configuration generated (`.runtimeconfig.json`)
- [ ] Build configurations (Debug/Release) both functional

**Documentation Validation:**
- [ ] README updated with .NET 10.0 requirements
- [ ] Build instructions updated if needed
- [ ] Dependencies documented correctly

### Testing Checklist Summary

**Phase 1: Atomic Upgrade Validation**
- [ ] ComputationalAgentFramework.csproj builds (Debug and Release)
- [ ] Examples.csproj builds (Debug and Release)
- [ ] Solution builds completely (Debug and Release)
- [ ] All project dependencies resolve
- [ ] No build errors or warnings

**Phase 2: Functional Validation**
- [ ] Examples.exe runs successfully
- [ ] All demo scenarios execute
- [ ] Output behavior matches expectations
- [ ] No runtime errors or exceptions
- [ ] ComputationalAgentFramework integration works

**Phase 3: Quality Validation**
- [ ] Code quality maintained
- [ ] Configuration files correct
- [ ] Documentation updated
- [ ] No security warnings
- [ ] Clean commit ready

### Validation Tools

**Build Validation:**
- `dotnet build` - Primary build tool
- `dotnet restore` - Dependency restoration
- Visual Studio build output - Detailed diagnostics

**Runtime Validation:**
- `dotnet run` - Execute application
- Manual testing - Functional verification
- Console output review - Behavioral validation

### Rollback Criteria

If any of the following occur, consider rolling back and reassessing:
- ? Solution fails to build after multiple fix attempts
- ? Critical runtime errors that block Examples application
- ? Loss of functionality compared to net472 version
- ? Unexpected breaking changes not identified in assessment

**Rollback Process:**
```bash
# Discard changes and return to pre-migration state
git checkout main
git branch -D upgrade-to-NET10
```

---

## Source Control Strategy

### Branching Model

**Current Branch:** `upgrade-to-NET10`  
**Source Branch:** `main`  
**Merge Target:** `main` (after validation complete)

**Branch Characteristics:**
- Dedicated upgrade branch isolates changes
- Preserves main branch stability during migration
- Enables easy rollback if needed
- Clean history for review

### Commit Strategy

**All-At-Once Approach: Single Commit Recommended**

Given the atomic nature of the All-At-Once strategy and small solution size, a single comprehensive commit is recommended:

**Recommended Commit Structure:**

```bash
# Single commit after all changes complete and validated
git add .
git commit -m "Migrate solution to .NET 10.0

- Convert ComputationalAgentFramework.csproj to SDK-style, target net10.0
- Convert Examples.csproj to SDK-style, target net10.0  
- Clean up AssemblyInfo.cs duplicate attributes
- Update App.config (remove .NET Framework startup section)
- Verify: Solution builds successfully with 0 errors
- Verify: Examples application runs and all scenarios pass
- Verify: 100% API compatibility maintained (340 APIs)

Migration Details:
- Source Framework: .NET Framework 4.7.2 (net472)
- Target Framework: .NET 10.0 (net10.0)
- Strategy: All-At-Once atomic upgrade
- Breaking Changes: None (100% API compatibility)
- Package Updates: None (zero external dependencies)

See .github/upgrades/assessment.md and plan.md for complete details."
```

**Rationale for Single Commit:**
- Atomic operation reflected in atomic commit
- All changes interdependent (projects must be migrated together)
- No intermediate stable states during SDK-style conversion
- Easier to review as cohesive changeset
- Simpler to rollback if needed (single revert)
- Clean history showing complete migration in one step

**Alternative: Multi-Commit Approach (If Preferred)**

If team prefers granular commits:

```bash
# Commit 1: Convert ComputationalAgentFramework
git add ComputationalAgentFramework.csproj Properties/AssemblyInfo.cs
git commit -m "Convert ComputationalAgentFramework to SDK-style, target net10.0"

# Commit 2: Convert Examples
git add Examples/ 
git commit -m "Convert Examples to SDK-style, target net10.0"

# Commit 3: Documentation
git add README.md .github/upgrades/
git commit -m "Update documentation for .NET 10.0 migration"
```

**Note:** Multi-commit approach doesn't align perfectly with All-At-Once strategy since projects aren't individually stable until all are migrated.

### Review and Merge Process

**Pre-Merge Checklist:**
- [ ] All validation criteria met (see Testing & Validation Strategy)
- [ ] Solution builds successfully
- [ ] Examples application tested and functional
- [ ] Documentation updated
- [ ] Commit message descriptive and complete
- [ ] No unintended files included (check `git status`)

**Pull Request (if using PR workflow):**

**PR Title:** `Migrate solution to .NET 10.0`

**PR Description Template:**
```markdown
## Migration Summary
Upgrades ComputationalAgentFramework solution from .NET Framework 4.7.2 to .NET 10.0.

## Changes
- ? Convert both projects to SDK-style format
- ? Update target framework from net472 to net10.0
- ? Clean up AssemblyInfo.cs duplicate attributes  
- ? Update App.config (Examples project)

## Testing
- ? Solution builds with 0 errors
- ? Examples application runs successfully
- ? All demo scenarios tested and functional
- ? 100% API compatibility maintained

## Migration Details
- **Strategy:** All-At-Once (atomic upgrade)
- **Projects:** 2 (ComputationalAgentFramework, Examples)
- **Breaking Changes:** None
- **API Compatibility:** 340/340 APIs compatible (100%)
- **Package Updates:** None (zero external dependencies)

## Documentation
- See `.github/upgrades/assessment.md` for analysis
- See `.github/upgrades/plan.md` for detailed migration plan

## Rollback Plan
If issues arise: `git revert <commit-sha>` or merge main back if needed.
```

**Review Criteria:**
- Code changes appropriate for SDK-style conversion
- Project files clean and minimal
- No unintended functional changes
- Documentation reflects new requirements
- Build and test evidence provided

**Merge Process:**
```bash
# After PR approved, merge to main
git checkout main
git merge --no-ff upgrade-to-NET10 -m "Merge .NET 10.0 upgrade"
git push origin main

# Optionally delete upgrade branch
git branch -d upgrade-to-NET10
git push origin --delete upgrade-to-NET10
```

### Git Best Practices for This Migration

**Files to Include:**
- ? Modified `.csproj` files (both projects)
- ? Modified `AssemblyInfo.cs` files (cleaned up)
- ? Modified or removed `App.config` (Examples)
- ? Updated documentation (README.md, etc.)
- ? Migration artifacts (`.github/upgrades/assessment.md`, `plan.md`)

**Files to Exclude:**
- ? `bin/` directories (build output)
- ? `obj/` directories (intermediate build files)
- ? `.vs/` directories (Visual Studio cache)
- ? User-specific files (`.suo`, `.user`)

**Verify `.gitignore` is Correct:**
Ensure standard .NET `.gitignore` is in place to prevent committing build artifacts.

### Post-Merge Actions

After successful merge to main:
1. Update local repositories: Team members pull latest main
2. Verify builds on other machines: Confirm .NET 10.0 SDK installed
3. Update CI/CD pipelines: Ensure build servers have .NET 10.0 SDK
4. Update deployment targets: Ensure servers have .NET 10.0 Runtime
5. Communicate to team: Notify of framework requirement change

---

## Success Criteria

### Technical Criteria

**All-At-Once Strategy Principles Applied:**
- ? All projects migrated simultaneously in single operation
- ? No intermediate multi-targeting states
- ? Dependency relationships maintained throughout migration
- ? Single coordinated build validation

**All Projects Migrated:**
- ? ComputationalAgentFramework.csproj targeting net10.0
- ? Examples.csproj targeting net10.0
- ? Both projects converted to SDK-style format
- ? No projects remain on net472

**Build Success:**
- ? Solution builds with `dotnet build` with 0 errors
- ? Solution builds with 0 warnings (or only acceptable nullable warnings)
- ? Both Debug and Release configurations build successfully
- ? All project dependencies resolve correctly

**Package Updates Complete:**
- ? N/A - Zero NuGet packages in solution (criterion automatically met)

**No Security Vulnerabilities:**
- ? No package security vulnerabilities remain
- ? N/A - Zero packages means zero vulnerabilities (criterion automatically met)

**All Tests Pass:**
- ?? No test projects exist in solution
- ? Manual validation performed via Examples application
- ? All demo scenarios execute successfully
- ? No runtime errors or exceptions

**API Compatibility Maintained:**
- ? ComputationalAgentFramework public API unchanged
- ? Examples application continues to function
- ? No breaking changes to library consumers
- ? 340 APIs remain compatible (100% compatibility)

**Runtime Validation:**
- ? Examples application runs on .NET 10.0 Runtime
- ? All functionality tested and verified
- ? Output behavior consistent with net472 version
- ? No unexpected exceptions or errors

### Quality Criteria

**Code Quality Maintained:**
- ? No new code smells introduced
- ? Code structure preserved (minimal changes to logic)
- ? SDK-style projects simpler and cleaner than classic projects
- ? AssemblyInfo.cs cleaned up (no duplicate attributes)

**Test Coverage Maintained:**
- ?? No test projects exist (N/A)
- ? Manual testing coverage equivalent to pre-migration
- ? All demo scenarios validated

**Documentation Updated:**
- ? README.md reflects .NET 10.0 requirements
- ? Build instructions updated (if needed)
- ? Prerequisites documented (SDK version requirement)
- ? Migration documentation complete (`assessment.md`, `plan.md`)

**No Regressions:**
- ? All functionality working as before migration
- ? No features lost or broken
- ? Examples application behavior unchanged
- ? Performance acceptable (no significant degradation)

### Process Criteria

**Strategy Followed:**
- ? All-At-Once strategy executed as planned
- ? Atomic operation performed (all projects together)
- ? No deviations from planned approach
- ? Timeline followed (fast execution)

**Source Control Strategy Followed:**
- ? Dedicated `upgrade-to-NET10` branch used
- ? Commit strategy executed (single comprehensive commit recommended)
- ? Clean commit history
- ? Descriptive commit messages
- ? Merge to main completed successfully

**Validation Criteria Met:**
- ? All validation checklists completed
- ? Per-project validation passed
- ? Solution-wide validation passed
- ? Functional validation passed
- ? Quality validation passed

**Risk Management:**
- ? All identified risks addressed or mitigated
- ? No unexpected high-severity issues encountered
- ? Contingency plans available (rollback possible)
- ? SDK-style conversion successful

### Stakeholder Acceptance

**Developer Experience:**
- ? Developers can build solution with .NET 10.0 SDK
- ? Build times acceptable
- ? IDE experience functional (Visual Studio 2022+)
- ? Debugging works correctly

**Deployment Readiness:**
- ? Solution ready for deployment with .NET 10.0 Runtime
- ? Deployment requirements documented
- ? Runtime dependencies understood
- ? CI/CD pipeline updates identified

**Team Satisfaction:**
- ? Migration completed within expectations
- ? No significant blockers encountered
- ? Knowledge transfer complete (team understands new structure)
- ? Documentation sufficient for ongoing maintenance

### Final Acceptance Checklist

**Before marking migration complete:**

**Technical Sign-Off:**
- [ ] All projects targeting net10.0 ?
- [ ] Solution builds with 0 errors ?
- [ ] All tests pass (or manual validation complete) ?
- [ ] No security vulnerabilities ?
- [ ] Runtime validation successful ?

**Quality Sign-Off:**
- [ ] Code quality maintained ?
- [ ] Documentation updated ?
- [ ] No regressions detected ?
- [ ] AssemblyInfo.cs cleanup complete ?

**Process Sign-Off:**
- [ ] Strategy executed correctly ?
- [ ] Source control strategy followed ?
- [ ] All validation criteria met ?
- [ ] Risk mitigation successful ?

**Deployment Sign-Off:**
- [ ] Solution ready for deployment ?
- [ ] Requirements documented ?
- [ ] Team trained/informed ?
- [ ] CI/CD considerations addressed ?

### Success Declaration

**Migration is considered successful when:**

All technical, quality, process, and stakeholder criteria are met, and the solution:
1. Builds successfully on .NET 10.0
2. Runs correctly on .NET 10.0 Runtime
3. Maintains all functionality from net472 version
4. Has no breaking changes or regressions
5. Is documented and ready for deployment

**Final Validation Command:**
```bash
# One-command validation of success
dotnet build ComputationalAgentFramework.sln --configuration Release && \
dotnet run --project Examples\Examples.csproj --configuration Release

# Expected: Clean build + successful application execution
```

When this command succeeds and all checklists are complete, the migration is successful. ?
