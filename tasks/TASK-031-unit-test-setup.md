# Task ID
TASK-031

# Title
Unit test project setup

# Owner
TestEngineer

# Status
DONE

# Priority
HIGH

# Complexity
LOW

# Dependencies
TASK-001

# Description
Create all test projects with xUnit, Moq, FluentAssertions. Add to solution. Configure test run in CI-friendly way. Shared test utilities project optional.

# Acceptance Criteria
- [ ] 5 test projects created and referenced to source projects
- [ ] xUnit, Moq, FluentAssertions packages added
- [ ] `dotnet test` runs (zero tests initially, zero failures)
- [ ] Test projects target net8.0

# Test Requirements
- Meta: `dotnet test` passes with at least one placeholder test per project

# Related Issues
None

# Progress Notes
2026-06-27: Task created during project initialization.
2026-06-27: xUnit + Moq + FluentAssertions, PlaceholderTests per project, tests/README.md (T4-2).
