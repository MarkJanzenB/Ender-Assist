# Task ID
TASK-005

# Title
Serial connection lifecycle

# Owner
SerialEngineer

# Status
DONE

# Priority
HIGH

# Complexity
MEDIUM

# Dependencies
TASK-004

# Description
Implement open/close/dispose lifecycle for `SerialPort` in `SerialEngine`. Support configurable baud rate (default 115200), 8N1, `\n` newline. Expose `ConnectionState` transitions: Disconnected → Connecting → Connected → Error. Run on background worker per ADR-003.

# Acceptance Criteria
- [x] `ConnectAsync(portName, baudRate)` opens port and sets Connected
- [x] `DisconnectAsync()` closes and disposes port cleanly
- [x] ConnectionStateChanged event fires on transitions
- [x] Invalid port name throws `SerialConnectionException`
- [x] Double-connect prevented (idempotent or throws)
- [x] Serial settings match Ender 3 V2 defaults

# Test Requirements
- Unit test: state transitions on connect/disconnect (mocked port)
- Unit test: invalid port raises exception
- Unit test: dispose called on disconnect

# Related Issues
None

# Progress Notes
2026-06-27: Task created during project initialization.
2026-06-27: `SerialEngine` implements connection lifecycle via `BackgroundService`, `ISerialPort` abstraction, state transitions, and `SerialOptions` (115200 8N1, ReadTimeout=1000). Extended by T4-4 with command queue; lifecycle covered by `SerialEngineTests`.
