# DONE

| ID | Title | Owner | Team | Completed |
|----|-------|-------|------|-----------|
| TASK-001 | Solution scaffold | Architect | T4-1 | 2026-06-27 |
| TASK-002 | Core domain models | Architect | T4-1 | 2026-06-27 |
| TASK-003 | DI bootstrap | Architect | T4-1 | 2026-06-27 |
| TASK-004 | Serial port discovery | SerialEngineer | T4-3 | 2026-06-27 |
| TASK-005 | Serial connection lifecycle | SerialEngineer | T4-3 | 2026-06-27 |
| TASK-006 | Marlin handshake | SerialEngineer | T4-3 | 2026-06-27 |
| TASK-007 | OK/busy synchronization | SerialEngineer | T4-4 | 2026-06-27 |
| TASK-008 | G-code line sender | SerialEngineer | T4-4 | 2026-06-27 |
| TASK-009 | Serial reconnection recovery | SerialEngineer | T4-4 | 2026-06-27 |
| TASK-031 | Unit test project setup | TestEngineer | T4-2 | 2026-06-27 |

## Milestones Complete

| Milestone | Tasks | Status |
|-----------|-------|--------|
| **M1 Foundation** | 001, 002, 003, 031 | **DONE** (4/4) |
| **M2 Serial** | 004–009 | **DONE** (6/6) |

## Verification

- `dotnet build` — 0 errors
- `dotnet test` — 43 passed (37 Serial + 3 Integration + 3 placeholders)

## Last Updated

2026-06-27 — Team-4 completed M1 and M2.
