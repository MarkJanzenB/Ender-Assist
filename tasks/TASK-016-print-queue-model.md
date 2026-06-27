# Task ID
TASK-016

# Title
Print queue model

# Owner
QueueEngineer

# Status
DONE

# Priority
HIGH

# Complexity
MEDIUM

# Dependencies
TASK-015, TASK-012

# Description
Implement in-memory print queue backed by `IPrintJobRepository`. Support enqueue, dequeue, reorder, and snapshot for UI. Queue reflects persisted state on startup reload.

# Acceptance Criteria
- [ ] Enqueue adds job with Pending status
- [ ] Dequeue returns next by priority then FIFO
- [ ] Reorder updates priority/order in DB
- [ ] GetSnapshot returns immutable read-only view
- [ ] ReloadFromPersistence restores queue on start

# Test Requirements
- Unit test: priority ordering
- Unit test: FIFO within same priority
- Unit test: reload restores pending jobs
- Unit test: enqueue persists to repository (mock)

# Related Issues
None

# Progress Notes
2026-06-27: Task created during project initialization.
2026-06-27: PrintQueueService enqueue/dequeue with SQLite backing.
