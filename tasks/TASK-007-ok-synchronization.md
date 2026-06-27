# Task ID
TASK-007

# Title
OK/busy synchronization

# Owner
SerialEngineer

# Status
DONE

# Priority
CRITICAL

# Complexity
HIGH

# Dependencies
TASK-006

# Description
Implement response parser and synchronization logic for Marlin `ok`, `busy`, and `Error:` responses. `SendCommandAsync` must block (async) until `ok` received or timeout. Handle `busy` by waiting and retrying. Support line number prefix if present (`N123 ...`).

# Acceptance Criteria
- [x] Waits for `ok` after each sent command
- [x] Handles `busy` with backoff retry
- [x] Parses `Error:line` and raises protocol exception
- [x] Configurable command timeout (default 30s for long moves)
- [x] Read loop runs continuously on background thread
- [x] Thread-safe command queue (one in-flight command)

# Test Requirements
- Unit test: ok response completes send
- Unit test: busy response retries then completes
- Unit test: error response throws MarlinProtocolException
- Unit test: timeout when ok never arrives
- Fixture: recorded Marlin transcript replay test

# Related Issues
None

# Progress Notes
2026-06-27: Task created during project initialization.
2026-06-27: T4-4 implemented `MarlinResponseParser`, `OkSynchronizer` (100ms busy backoff, max 50 retries, 30s default timeout), background read loop in `SerialEngine`, and unit tests (18 parser/synchronizer cases).
