# Task ID
TASK-028

# Title
Print queue UI

# Owner
UIEngineer

# Status
TODO

# Priority
HIGH

# Complexity
MEDIUM

# Dependencies
TASK-018, TASK-025

# Description
Build queue panel: job list (DataGrid or ListView), Add G-code file, Remove, Reorder priority, Start/Pause/Cancel buttons, progress per job.

# Acceptance Criteria
- [ ] File picker imports .gcode into queue
- [ ] Job list shows name, status, priority, progress
- [ ] Start/Pause/Cancel/E-stop buttons functional
- [ ] Drag-drop or buttons for priority reorder
- [ ] Empty state message when queue empty
- [ ] Double-click opens file location

# Test Requirements
- ViewModel unit test: add file enqueues job
- ViewModel unit test: remove pending job
- ViewModel unit test: start command delegates to queue service

# Related Issues
None

# Progress Notes
2026-06-27: Task created during project initialization.
