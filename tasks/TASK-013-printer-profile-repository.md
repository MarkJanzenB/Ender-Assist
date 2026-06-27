# Task ID
TASK-013

# Title
Printer profile repository

# Owner
PersistenceEngineer

# Status
TODO

# Priority
MEDIUM

# Complexity
LOW

# Dependencies
TASK-011

# Description
Implement `IPrinterProfileRepository`. Seed default Ender 3 V2 profile (115200 baud, buffer 4). Support CRUD and default profile selection.

# Acceptance Criteria
- [ ] Default Ender 3 V2 profile seeded on first run
- [ ] CRUD operations for profiles
- [ ] GetDefault returns default profile
- [ ] SetDefault ensures only one default
- [ ] Maps to Core PrinterProfile entity

# Test Requirements
- Unit test: default profile seeded once
- Unit test: set default demotes previous default
- Integration test: CRUD round-trip

# Related Issues
None

# Progress Notes
2026-06-27: Task created during project initialization.
