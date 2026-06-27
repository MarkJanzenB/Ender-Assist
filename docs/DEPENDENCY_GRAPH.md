# Dependency Graph

Task dependency directed acyclic graph (DAG). A task may start only when all dependencies are **DONE**.

## Visual Overview

```mermaid
flowchart TD
    subgraph M1["M1 — Foundation"]
        T001[TASK-001 Solution Scaffold]
        T002[TASK-002 Core Domain Models]
        T003[TASK-003 DI Bootstrap]
        T031[TASK-031 Test Project Setup]
    end

    subgraph M2["M2 — Serial"]
        T004[TASK-004 Port Discovery]
        T005[TASK-005 Connection Lifecycle]
        T006[TASK-006 Marlin Handshake]
        T007[TASK-007 OK Synchronization]
        T008[TASK-008 G-code Streaming]
        T009[TASK-009 Reconnect Recovery]
    end

    subgraph M3["M3 — Persistence"]
        T010[TASK-010 SQLite Schema]
        T011[TASK-011 Migrations]
        T012[TASK-012 Job Repository]
        T013[TASK-013 Profile Repository]
        T014[TASK-014 Settings Persistence]
    end

    subgraph M4["M4 — Queue"]
        T015[TASK-015 G-code Parser]
        T016[TASK-016 Queue Model]
        T017[TASK-017 State Machine]
        T018[TASK-018 Job Scheduling]
    end

    subgraph M5["M5 — Monitor & Safety"]
        T019[TASK-019 Temperature Monitor]
        T020[TASK-020 Position Tracker]
        T021[TASK-021 Status Aggregator]
        T022[TASK-022 Emergency Stop]
        T023[TASK-023 Thermal Safety Hooks]
        T024[TASK-024 Pause Resume]
    end

    subgraph M6["M6 — UI"]
        T025[TASK-025 MVVM Infrastructure]
        T026[TASK-026 Main Window Shell]
        T027[TASK-027 Connection Panel]
        T028[TASK-028 Queue UI]
        T029[TASK-029 Monitoring Dashboard]
        T030[TASK-030 Settings UI]
    end

    subgraph M7["M7 — QA & Docs"]
        T032[TASK-032 Serial Integration Tests]
        T033[TASK-033 E2E Simulation Tests]
        T034[TASK-034 User Guide]
        T035[TASK-035 Developer Guide]
    end

    T001 --> T002
    T001 --> T003
    T001 --> T031
    T002 --> T004
    T003 --> T004
    T004 --> T005
    T005 --> T006
    T006 --> T007
    T007 --> T008
    T005 --> T009
    T008 --> T009

    T001 --> T010
    T010 --> T011
    T011 --> T012
    T011 --> T013
    T011 --> T014

    T002 --> T015
    T015 --> T016
    T012 --> T016
    T016 --> T017
    T008 --> T017
    T017 --> T018
    T014 --> T018

    T007 --> T019
    T019 --> T020
    T019 --> T021
    T020 --> T021

    T008 --> T022
    T017 --> T022
    T019 --> T023
    T017 --> T024
    T022 --> T024

    T003 --> T025
    T025 --> T026
    T005 --> T027
    T025 --> T027
    T018 --> T028
    T025 --> T028
    T021 --> T029
    T025 --> T029
    T014 --> T030
    T013 --> T030
    T025 --> T030

    T008 --> T032
    T031 --> T032
    T018 --> T033
    T031 --> T033
    T030 --> T034
    T033 --> T034
    T001 --> T035
```

## Dependency Table

| Task | Depends On |
|------|------------|
| TASK-001 | — |
| TASK-002 | TASK-001 |
| TASK-003 | TASK-001 |
| TASK-004 | TASK-002, TASK-003 |
| TASK-005 | TASK-004 |
| TASK-006 | TASK-005 |
| TASK-007 | TASK-006 |
| TASK-008 | TASK-007 |
| TASK-009 | TASK-005, TASK-008 |
| TASK-010 | TASK-001 |
| TASK-011 | TASK-010 |
| TASK-012 | TASK-011 |
| TASK-013 | TASK-011 |
| TASK-014 | TASK-011 |
| TASK-015 | TASK-002 |
| TASK-016 | TASK-015, TASK-012 |
| TASK-017 | TASK-016, TASK-008 |
| TASK-018 | TASK-017, TASK-014 |
| TASK-019 | TASK-007 |
| TASK-020 | TASK-019 |
| TASK-021 | TASK-019, TASK-020 |
| TASK-022 | TASK-008, TASK-017 |
| TASK-023 | TASK-019 |
| TASK-024 | TASK-017, TASK-022 |
| TASK-025 | TASK-003 |
| TASK-026 | TASK-025 |
| TASK-027 | TASK-005, TASK-025 |
| TASK-028 | TASK-018, TASK-025 |
| TASK-029 | TASK-021, TASK-025 |
| TASK-030 | TASK-014, TASK-013, TASK-025 |
| TASK-031 | TASK-001 |
| TASK-032 | TASK-008, TASK-031 |
| TASK-033 | TASK-018, TASK-031 |
| TASK-034 | TASK-030, TASK-033 |
| TASK-035 | TASK-001 |

## Critical Path

```text
TASK-001 → TASK-002 → TASK-004 → TASK-005 → TASK-006 → TASK-007 → TASK-008 → TASK-017 → TASK-018 → TASK-033 → TASK-034
```

**Estimated critical path duration:** ~120–160 hours (sequential agent work).

## Parallel Work Streams

After TASK-001 completes, these streams can proceed in parallel:

| Stream | Tasks | Owner Focus |
|--------|-------|-------------|
| A — Serial | TASK-002 → 004–009 | SerialEngineer |
| B — Persistence | TASK-010 → 011–014 | DatabaseEngineer, PersistenceEngineer |
| C — Queue prep | TASK-015 (after TASK-002) | QueueEngineer |
| D — Test infra | TASK-031 | TestEngineer |
| E — UI prep | TASK-025 (after TASK-003) | UIEngineer |

## Ready to Start (Initial)

These tasks have **no dependencies** and are available immediately after project initialization:

- TASK-001 — Solution scaffold
- TASK-010 — SQLite schema design (after TASK-001; blocked until implementation gate opens)

**Note:** All tasks remain TODO until implementation phase begins. TASK-001 is the first implementation task.

## State Recovery

To find available work:

1. List task IDs in `/progress/TODO.md`
2. For each, verify all dependencies appear in `/progress/DONE.md`
3. Pick highest-priority eligible task
4. Move to IN_PROGRESS per workflow rules
