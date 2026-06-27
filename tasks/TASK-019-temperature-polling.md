# Task ID
TASK-019

# Title
Temperature polling service

# Owner
MonitoringEngineer

# Status
DONE

# Priority
HIGH

# Complexity
MEDIUM

# Dependencies
TASK-007

# Description
Implement background temperature monitor sending `M105` at configurable interval (default 2s). Parse hotend and bed temperatures from response. Expose `TemperatureSnapshot` for UI binding.

# Acceptance Criteria
- [ ] Polls M105 when connected and not paused (configurable)
- [ ] Parses T: and B: values from Marlin response
- [ ] TemperatureChanged event fires on update
- [ ] Stops polling on disconnect
- [ ] Does not interfere with active G-code stream (uses command queue)

# Test Requirements
- Unit test: parse standard M105 response
- Unit test: polling stops on disconnect
- Unit test: interval respected (mock timer)

# Related Issues
None

# Progress Notes
2026-06-27: Task created during project initialization.
2026-06-27: TemperatureMonitorService M105 polling.
