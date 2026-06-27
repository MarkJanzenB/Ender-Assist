# Task ID
TASK-030

# Title
Settings and printer profile UI

# Owner
UIEngineer

# Status
TODO

# Priority
MEDIUM

# Complexity
MEDIUM

# Dependencies
TASK-014, TASK-013, TASK-025

# Description
Build settings dialog: printer profiles CRUD, default baud/buffer, auto-start queue, temperature poll interval, logging level.

# Acceptance Criteria
- [ ] List/edit/create/delete printer profiles
- [ ] Set default profile
- [ ] App settings save on OK
- [ ] Validation on numeric fields
- [ ] Ender 3 V2 defaults visible and editable

# Test Requirements
- ViewModel unit test: save settings calls repository
- ViewModel unit test: invalid baud rejected
- ViewModel unit test: profile CRUD operations

# Related Issues
None

# Progress Notes
2026-06-27: Task created during project initialization.
