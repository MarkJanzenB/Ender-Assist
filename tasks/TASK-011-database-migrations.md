# Task ID
TASK-011

# Title
Database migration infrastructure

# Owner
DatabaseEngineer

# Status
DONE

# Priority
HIGH

# Complexity
MEDIUM

# Dependencies
TASK-010

# Description
Implement migration runner that applies numbered SQL scripts on startup. Track applied version in `schema_version`. Support idempotent first-run creation. Integrate with DI as `IDatabaseMigrator`.

# Acceptance Criteria
- [ ] Applies pending migrations in order on app start
- [ ] Skips already-applied migrations
- [ ] Creates database file if missing
- [ ] Logs migration results
- [ ] Fails fast with clear error on migration failure

# Test Requirements
- Unit test: fresh DB reaches latest version
- Unit test: re-run is idempotent (no error)
- Unit test: partial migration failure rolls back or reports clearly

# Related Issues
None

# Progress Notes
2026-06-27: Task created during project initialization.
2026-06-27: DatabaseMigrator + DatabaseInitializer hosted service.
