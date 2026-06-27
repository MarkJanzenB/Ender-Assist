# Task ID
TASK-014

# Title
Application settings persistence

# Owner
PersistenceEngineer

# Status
DONE

# Priority
MEDIUM

# Complexity
LOW

# Dependencies
TASK-011

# Description
Implement `ISettingsRepository` key-value store for app settings: last used port, poll intervals, log level, buffer size override, theme preference.

# Acceptance Criteria
- [ ] Get/Set string and typed values (int, bool)
- [ ] Settings survive restart
- [ ] Default values returned when key missing
- [ ] Thread-safe access

# Test Requirements
- Unit test: set and get round-trip
- Unit test: missing key returns default
- Integration test: persistence across connection reopen

# Related Issues
None

# Progress Notes
2026-06-27: Task created during project initialization.
2026-06-27: SettingsRepository JSON key-value, 2 tests.
