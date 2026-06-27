# Task ID
TASK-008

# Title
G-code line sender with flow control

# Owner
SerialEngineer

# Status
DONE

# Priority
CRITICAL

# Complexity
HIGH

# Dependencies
TASK-007

# Description
Implement buffered G-code streaming. Send lines respecting Marlin planner buffer (configurable buffer size, default 4 for stock Marlin). Skip comments and blank lines. Track lines sent vs acknowledged. Support cancel mid-stream.

# Acceptance Criteria
- [x] Streams G-code file line-by-line with ok synchronization
- [x] Respects buffer size (don't send more than N unacknowledged lines)
- [x] Skips `;` comments and empty lines
- [x] Reports progress events (line number, percentage)
- [x] CancellationToken stops stream and clears buffer
- [x] Handles `M109`/`M190` long waits without false timeout

# Test Requirements
- Unit test: buffer never exceeds configured max
- Unit test: comments skipped, valid lines sent
- Unit test: cancellation mid-stream stops sending
- Integration test: stream sample 100-line G-code against mock serial

# Related Issues
None

# Progress Notes
2026-06-27: Task created during project initialization.
2026-06-27: T4-4 added `StreamGCodeAsync` with planner buffer (default 4), comment/blank skipping, `StreamProgress` events, M112 priority bypass, and stream/cancellation tests via `FakeSerialPort`.
