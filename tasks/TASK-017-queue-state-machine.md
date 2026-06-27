# Task ID
TASK-017

# Title
Queue state machine

# Owner
QueueEngineer

# Status
TODO

# Priority
CRITICAL

# Complexity
HIGH

# Dependencies
TASK-016, TASK-008

# Description
Implement print state machine: Idle → Preparing → Printing → Paused → Completed/Failed/Cancelled. Coordinate with SerialEngine for G-code streaming. Update job progress in repository. Emit state change events.

# Acceptance Criteria
- [ ] Valid state transitions enforced (invalid transitions throw)
- [ ] Preparing validates connection and file before Printing
- [ ] Printing streams via ISerialEngine g-code sender
- [ ] Progress updated every N lines (configurable)
- [ ] Completed sets timestamp and 100% progress
- [ ] Failed captures error message

# Test Requirements
- Unit test: all valid transitions succeed
- Unit test: invalid transition rejected
- Unit test: pause from Printing enters Paused
- Integration test: full mock print cycle Idle → Completed

# Related Issues
None

# Progress Notes
2026-06-27: Task created during project initialization.
