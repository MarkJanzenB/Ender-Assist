# Task ID
TASK-033

# Title
End-to-end print simulation tests

# Owner
TestEngineer

# Status
TODO

# Priority
HIGH

# Complexity
HIGH

# Dependencies
TASK-018, TASK-031

# Description
E2E tests simulating full print workflow: enqueue G-code, mock serial, run state machine to completion. Test pause/resume and cancel paths.

# Acceptance Criteria
- [ ] E2E: enqueue → print → complete
- [ ] E2E: pause → resume → complete
- [ ] E2E: cancel mid-print
- [ ] E2E: auto-start second job
- [ ] Uses in-memory DB and mock serial

# Test Requirements
- Minimum 5 E2E scenario tests
- All pass in `dotnet test`

# Related Issues
None

# Progress Notes
2026-06-27: Task created during project initialization.
