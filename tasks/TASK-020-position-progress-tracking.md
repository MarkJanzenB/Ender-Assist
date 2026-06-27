# Task ID
TASK-020

# Title
Position and progress tracking

# Owner
MonitoringEngineer

# Status
DONE

# Priority
MEDIUM

# Complexity
MEDIUM

# Dependencies
TASK-019

# Description
Track print progress from queue line counts and optionally poll `M114` for current position on user request. Compute ETA from elapsed time and progress percentage.

# Acceptance Criteria
- [ ] Progress percentage from lines sent / total lines
- [ ] ETA calculated when progress > 5%
- [ ] M114 position available on demand (not continuous poll)
- [ ] PositionSnapshot model with X/Y/Z/E
- [ ] ProgressUpdated event for UI

# Test Requirements
- Unit test: progress calculation
- Unit test: ETA calculation with known elapsed time
- Unit test: M114 parse sample response

# Related Issues
None

# Progress Notes
2026-06-27: Task created during project initialization.
2026-06-27: Progress/ETA in PrinterStatusService.
