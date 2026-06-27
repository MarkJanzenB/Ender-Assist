# Task ID
TASK-004

# Title
Serial port discovery

# Owner
SerialEngineer

# Status
DONE

# Priority
HIGH

# Complexity
LOW

# Dependencies
TASK-002, TASK-003

# Description
Implement `SerialPortDiscovery` service that enumerates available COM ports via `SerialPort.GetPortNames()`. Optionally enrich with friendly device names (USB serial adapter identification). Return structured `SerialPortInfo` objects for UI binding.

# Acceptance Criteria
- [x] Lists all available COM ports on Windows
- [x] Returns port name and optional description
- [x] Handles zero ports gracefully (empty list, no exception)
- [x] Refresh method re-enumerates without restart
- [x] Registered in DI as `ISerialPortDiscovery`

# Test Requirements
- Unit test: mock port name list returns expected count
- Unit test: empty port list returns empty collection
- Unit test: duplicate refresh returns consistent results

# Related Issues
None

# Progress Notes
2026-06-27: Task created during project initialization.
2026-06-27: Implemented `SerialPortDiscovery` with thread-safe cache, `GetPortsAsync`/`RefreshAsync`, and `SerialPortDiscoveryTests`. Registered in `HostBootstrap`.
