# Task ID
TASK-002

# Title
Core domain models and interfaces

# Owner
Architect

# Status
DONE

# Priority
CRITICAL

# Complexity
MEDIUM

# Dependencies
TASK-001

# Description
Define domain entities, enums, and service interfaces in `MarlinPrintMiddleware.Core`: `PrintJob`, `PrinterProfile`, `PrinterStatus`, `GCodeLine`, `ConnectionState`, `PrintState`, `ISerialEngine`, `IPrintQueueService`, `IPrintJobRepository`, `IPrinterProfileRepository`, and related event args.

# Acceptance Criteria
- [ ] All models documented in ARCHITECTURE.md exist in Core
- [ ] Interfaces have no implementation details (serial/SQLite agnostic)
- [ ] Enums cover all v1 states (connection, print, job priority)
- [ ] XML doc comments on all public interfaces
- [ ] Core project has zero NuGet dependencies beyond analyzers

# Test Requirements
- Unit tests for model validation logic (if any)
- Unit tests for enum extension helpers

# Related Issues
None

# Progress Notes
2026-06-27: Task created during project initialization.
2026-06-27: 25 Core files — enums, models, interfaces, events, exceptions (T4-1).
