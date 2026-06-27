# Task ID
TASK-023

# Title
Thermal runaway detection hooks

# Owner
SafetyEngineer

# Status
TODO

# Priority
MEDIUM

# Complexity
MEDIUM

# Dependencies
TASK-019

# Description
Monitor temperature trends for anomalies (optional v1 hooks). Detect unexpected drop during print or failure to reach target within timeout. Surface warnings to UI; rely on Marlin firmware for actual runaway protection.

# Acceptance Criteria
- [ ] Warn if hotend drops >20°C during active print
- [ ] Warn if M109 target not reached within 10 min
- [ ] Warnings do not auto-estop (firmware handles hard safety)
- [ ] Configurable thresholds in settings
- [ ] ThermalWarning event for UI

# Test Requirements
- Unit test: drop detection triggers warning
- Unit test: timeout detection triggers warning
- Unit test: thresholds configurable

# Related Issues
None

# Progress Notes
2026-06-27: Task created during project initialization.
