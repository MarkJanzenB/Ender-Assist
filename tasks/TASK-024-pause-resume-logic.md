# Task ID
TASK-024

# Title
Safe pause/resume logic

# Owner
SafetyEngineer

# Status
DONE

# Priority
HIGH

# Complexity
MEDIUM

# Dependencies
TASK-017, TASK-022

# Description
Implement pause (M125 or M0 per profile) and resume. On pause: stop sending G-code, retract/hold as per Marlin. On resume: verify temperatures stable, restore queue to Printing, continue from last acknowledged line.

# Acceptance Criteria
- [ ] PauseAsync halts stream and sends pause command
- [ ] ResumeAsync validates state and continues from last_line_sent
- [ ] Cannot resume after emergency stop without user reset
- [ ] Paused state persisted in DB
- [ ] Coordinate with queue state machine

# Test Requirements
- Unit test: pause transitions to Paused
- Unit test: resume continues from correct line
- Unit test: resume blocked after estop
- Integration test: pause/resume mock cycle

# Related Issues
None

# Progress Notes
2026-06-27: Task created during project initialization.
2026-06-27: PauseResumeService M125 + queue.
