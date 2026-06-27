# Task ID
TASK-009

# Title
Serial reconnection and error recovery

# Owner
SerialEngineer

# Status
DONE

# Priority
HIGH

# Complexity
HIGH

# Dependencies
TASK-005, TASK-008

# Description
Handle USB disconnect during print. Detect port loss (IOException, port removed). Transition to Error state. Support manual reconnect to same or different port. Preserve queue position for resume after reconnect (coordinate with QueueEngineer).

# Acceptance Criteria
- [x] Detects unexpected port closure within 2s
- [x] Raises ConnectionLost event with last acknowledged line
- [x] ReconnectAsync restores connection and re-handshakes
- [x] Does not auto-resume print without user confirmation
- [x] Retry policy: max 3 attempts with exponential backoff
- [x] Logs all recovery actions

# Test Requirements
- Unit test: IOException triggers ConnectionLost event
- Unit test: reconnect succeeds on mock port
- Unit test: retry limit enforced
- Integration test: simulated disconnect during stream

# Related Issues
None

# Progress Notes
2026-06-27: Task created during project initialization.
2026-06-27: T4-4 added `ConnectionLostEventArgs`, `ReconnectAsync` on `ISerialEngine`, IOException handling in read/write paths, 3-attempt exponential backoff reconnect, and disconnect/reconnect unit tests.
