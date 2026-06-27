# Task ID
TASK-015

# Title
G-code file parser

# Owner
QueueEngineer

# Status
DONE

# Priority
HIGH

# Complexity
MEDIUM

# Dependencies
TASK-002

# Description
Implement G-code file reader that validates file existence, counts lines, extracts metadata (estimated time if present in comments), and yields `GCodeLine` objects. Handle large files via streaming (IAsyncEnumerable).

# Acceptance Criteria
- [ ] Validates file path and extension (.gcode, .gco, .g)
- [ ] Streams lines without loading entire file into memory
- [ ] Skips comments and blank lines for count
- [ ] Extracts slicer metadata from header comments if present
- [ ] Rejects binary/non-text files

# Test Requirements
- Unit test: parse sample Cura G-code header
- Unit test: line count matches expected
- Unit test: invalid path throws
- Sample G-code fixture in test project

# Related Issues
None

# Progress Notes
2026-06-27: Task created during project initialization.
2026-06-27: GCodeParser streaming + Cura metadata, 3 tests.
