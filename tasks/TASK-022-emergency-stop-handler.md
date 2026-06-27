# Task ID
TASK-022

# Title
Emergency stop handler

# Owner
SafetyEngineer

# Status
TODO

# Priority
CRITICAL

# Complexity
MEDIUM

# Dependencies
TASK-008, TASK-017

# Description
Implement emergency stop: send `M112` immediately (priority command bypassing queue), halt G-code stream, set queue to Cancelled/Failed, log event. UI big-red-button binding.

# Acceptance Criteria
- [ ] M112 sent within 500ms of trigger
- [ ] Active G-code stream cancelled immediately
- [ ] Queue state set to safe halted state
- [ ] Subsequent commands blocked until reset acknowledged
- [ ] EmergencyStopTriggered event logged and surfaced

# Test Requirements
- Unit test: M112 sent with priority
- Unit test: stream cancellation invoked
- Unit test: commands blocked after estop until reset

# Related Issues
None

# Progress Notes
2026-06-27: Task created during project initialization.
