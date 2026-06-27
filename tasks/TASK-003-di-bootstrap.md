# Task ID
TASK-003

# Title
Dependency injection bootstrap

# Owner
Architect

# Status
DONE

# Priority
CRITICAL

# Complexity
MEDIUM

# Dependencies
TASK-001

# Description
Configure `Microsoft.Extensions.Hosting` in the App project. Register placeholder services for all Core interfaces. Set up logging (`ILogger<T>`). Enable background service hosting. Wire host startup in `App.xaml.cs`.

# Acceptance Criteria
- [ ] `HostBuilder` configured with DI container
- [ ] All Core interfaces registered (stub or null implementations initially)
- [ ] Logging configured (console + debug in dev)
- [ ] Application starts without crash
- [ ] Background services can be registered and started

# Test Requirements
- Integration test: host builds and resolves `ISerialEngine` from DI
- Integration test: application host starts and stops cleanly

# Related Issues
None

# Progress Notes
2026-06-27: Task created during project initialization.
2026-06-27: HostBootstrap, stub services, App lifecycle, integration tests (T4-1).
