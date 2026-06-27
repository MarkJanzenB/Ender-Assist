# Task ID
TASK-021

# Title
Printer status aggregation

# Owner
MonitoringEngineer

# Status
TODO

# Priority
HIGH

# Complexity
MEDIUM

# Dependencies
TASK-019, TASK-020

# Description
Aggregate connection state, temperatures, progress, and print state into unified `PrinterStatus` snapshot. Refresh on timer and on events. Thread-safe for UI binding.

# Acceptance Criteria
- [ ] PrinterStatus combines all sub-states
- [ ] INotifyPropertyChanged or observable stream for UI
- [ ] Snapshot refresh at most 4 Hz (avoid UI flood)
- [ ] Handles partial data (connected but no temp yet)
- [ ] Registered in DI as IPrinterStatusService

# Test Requirements
- Unit test: aggregate from mocked sub-services
- Unit test: partial data handled gracefully
- Unit test: thread-safe concurrent updates

# Related Issues
None

# Progress Notes
2026-06-27: Task created during project initialization.
