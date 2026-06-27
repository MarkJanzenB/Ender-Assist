# Task ID
TASK-029

# Title
Monitoring dashboard UI

# Owner
UIEngineer

# Status
DONE

# Priority
HIGH

# Complexity
MEDIUM

# Dependencies
TASK-021, TASK-025

# Description
Build monitoring panel: hotend/bed temperature gauges or text, progress bar, elapsed/remaining time, current job name, optional position display.

# Acceptance Criteria
- [ ] Live temperature display updates from IPrinterStatusService
- [ ] Progress bar 0-100%
- [ ] Elapsed and ETA displayed
- [ ] Visual distinction for heating vs printing vs idle
- [ ] Updates without UI thread blocking

# Test Requirements
- ViewModel unit test: temperature binding updates
- ViewModel unit test: progress formatting
- ViewModel unit test: ETA display when available

# Related Issues
None

# Progress Notes
2026-06-27: Task created during project initialization.
2026-06-27: Monitoring panel temps/progress.
