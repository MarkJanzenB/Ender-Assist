# Task ID
TASK-006

# Title
Marlin handshake and firmware info

# Owner
SerialEngineer

# Status
DONE

# Priority
HIGH

# Complexity
MEDIUM

# Dependencies
TASK-005

# Description
After connection, send `M115` to retrieve firmware info. Parse response for Marlin version, machine type, and capability flags. Store result in `FirmwareInfo` model. Validate printer responds before marking connection ready.

# Acceptance Criteria
- [x] Sends `M115` automatically after connect
- [x] Parses firmware name and version from response
- [x] Detects non-Marlin responses and sets Error state
- [x] Timeout (configurable, default 5s) if no response
- [x] FirmwareInfo exposed via ISerialEngine or dedicated service

# Test Requirements
- Unit test: parse sample M115 response from Ender 3 V2
- Unit test: timeout when no response
- Unit test: non-Marlin response detected

# Related Issues
None

# Progress Notes
2026-06-27: Task created during project initialization.
2026-06-27: Added `MarlinHandshakeParser` (M115 → `FirmwareInfo`), wired into `SerialEngine.PerformHandshakeAsync` with `SerialOptions.HandshakeTimeoutMs=5000`. Tests in `MarlinHandshakeParserTests` using `Fixtures/sample_m115_response.txt`.
