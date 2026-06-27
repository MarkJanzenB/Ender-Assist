# Task ID
TASK-012

# Title
Print job repository

# Owner
PersistenceEngineer

# Status
TODO

# Priority
HIGH

# Complexity
MEDIUM

# Dependencies
TASK-011

# Description
Implement `IPrintJobRepository` with CRUD and queue queries. Support filtering by status (Pending, Printing, Completed, Failed, Cancelled). Persist progress (last_line_sent) for resume. Use parameterized SQL.

# Acceptance Criteria
- [ ] Upsert, GetById, GetPending, GetAll implemented
- [ ] UpdateStatus and UpdateProgress methods
- [ ] Maps DB rows to Core PrintJob entities
- [ ] Async methods throughout
- [ ] Transaction support for status + progress update

# Test Requirements
- Unit test: insert and retrieve job
- Unit test: update progress persists
- Unit test: filter pending jobs ordered by priority
- Integration test: against in-memory SQLite

# Related Issues
None

# Progress Notes
2026-06-27: Task created during project initialization.
