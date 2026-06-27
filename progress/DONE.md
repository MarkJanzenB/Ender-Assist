# DONE

| ID | Title | Milestone | Completed |
|----|-------|-----------|-----------|
| TASK-001 | Solution scaffold | M1 | 2026-06-27 |
| TASK-002 | Core domain models | M1 | 2026-06-27 |
| TASK-003 | DI bootstrap | M1 | 2026-06-27 |
| TASK-004 | Serial port discovery | M2 | 2026-06-27 |
| TASK-005 | Serial connection lifecycle | M2 | 2026-06-27 |
| TASK-006 | Marlin handshake | M2 | 2026-06-27 |
| TASK-007 | OK/busy synchronization | M2 | 2026-06-27 |
| TASK-008 | G-code line sender | M2 | 2026-06-27 |
| TASK-009 | Serial reconnection recovery | M2 | 2026-06-27 |
| TASK-010 | SQLite schema design | M3 | 2026-06-27 |
| TASK-011 | Database migrations | M3 | 2026-06-27 |
| TASK-012 | Print job repository | M3 | 2026-06-27 |
| TASK-013 | Printer profile repository | M3 | 2026-06-27 |
| TASK-014 | Settings persistence | M3 | 2026-06-27 |
| TASK-015 | G-code file parser | M4 | 2026-06-27 |
| TASK-016 | Print queue model | M4 | 2026-06-27 |
| TASK-017 | Queue state machine | M4 | 2026-06-27 |
| TASK-018 | Job scheduling | M4 | 2026-06-27 |
| TASK-031 | Unit test project setup | M1 | 2026-06-27 |

## Milestones Complete

| Milestone | Tasks | Status |
|-----------|-------|--------|
| M1 Foundation | 001, 002, 003, 031 | **DONE** |
| M2 Serial | 004–009 | **DONE** |
| M3 Persistence | 010–014 | **DONE** |
| M4 Queue | 015–018 | **DONE** |

## Verification

```text
dotnet build  → 0 errors
dotnet test   → 60 passed
```

## Last Updated

2026-06-27 — M3 + M4 complete.
