# Task ID
TASK-010

# Title
SQLite schema design

# Owner
DatabaseEngineer

# Status
TODO

# Priority
HIGH

# Complexity
MEDIUM

# Dependencies
TASK-001

# Description
Design SQLite schema for print jobs, queue ordering, printer profiles, application settings, and schema version. Document ERD in task progress notes. Tables: `schema_version`, `print_jobs`, `printer_profiles`, `settings`.

# Acceptance Criteria
- [ ] Schema SQL script in `Persistence/Migrations/001_initial.sql`
- [ ] print_jobs: id, file_path, name, status, priority, progress, created_at, started_at, completed_at, last_line_sent
- [ ] printer_profiles: id, name, port, baud_rate, buffer_size, is_default
- [ ] settings: key-value store
- [ ] schema_version table for migration tracking
- [ ] Indexes on status and queue order columns

# Test Requirements
- Schema applies cleanly to fresh in-memory SQLite database
- All foreign keys and constraints validated

# Related Issues
None

# Progress Notes
2026-06-27: Task created during project initialization.
