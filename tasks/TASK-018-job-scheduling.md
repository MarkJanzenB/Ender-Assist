# Task ID
TASK-018

# Title
Job scheduling and prioritization

# Owner
QueueEngineer

# Status
DONE

# Priority
HIGH

# Complexity
MEDIUM

# Dependencies
TASK-017, TASK-014

# Description
Implement automatic start of next queued job when current completes (if auto-start enabled in settings). Support manual start. Priority levels: Low, Normal, High, Critical.

# Acceptance Criteria
- [ ] Auto-start next job when enabled and printer connected
- [ ] Manual StartNextAsync bypasses auto-start setting
- [ ] Priority reordering takes effect before next dequeue
- [ ] No start if printer disconnected (graceful message)
- [ ] Setting persisted via ISettingsRepository

# Test Requirements
- Unit test: auto-start chains two jobs
- Unit test: auto-start disabled waits for manual
- Unit test: disconnected blocks start

# Related Issues
None

# Progress Notes
2026-06-27: Task created during project initialization.
2026-06-27: Auto-start via queue.auto_start setting.
