# Task ID
TASK-032

# Title
Serial layer integration tests

# Owner
TestEngineer

# Status
DONE

# Priority
HIGH

# Complexity
HIGH

# Dependencies
TASK-008, TASK-031

# Description
Integration tests for serial layer using recorded Marlin session transcripts (fake serial port or mock stream). Cover handshake, ok sync, g-code streaming, and error responses.

# Acceptance Criteria
- [ ] Fixture transcripts for Ender 3 V2 M115, M105, ok responses
- [ ] Tests replay transcript through SerialEngine
- [ ] G-code stream test with 50+ lines
- [ ] Error response test triggers exception
- [ ] All tests run without physical printer

# Test Requirements
- Minimum 10 integration tests for serial layer
- Tests documented in test project README

# Related Issues
None

# Progress Notes
2026-06-27: Task created during project initialization.
2026-06-27: Serial tests (37) in Serial.Tests.
