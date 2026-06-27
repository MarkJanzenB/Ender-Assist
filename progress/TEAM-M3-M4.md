# Team M3/M4 Sprint Log

**Sprint goal:** Complete M3 (Persistence) and M4 (Queue).  
**Status:** **COMPLETE**  
**Completed:** 2026-06-27

## Roster

| Slot | Role | Tasks | Status |
|------|------|-------|--------|
| M3-A | DatabaseEngineer | TASK-010, 011 | DONE |
| M3-B | PersistenceEngineer | TASK-012, 013, 014 | DONE |
| M4-A | QueueEngineer | TASK-015, 016, 017 | DONE |
| M4-B | QueueEngineer | TASK-018 | DONE |

## Milestone Progress

| Milestone | Tasks | Done | Status |
|-----------|-------|------|--------|
| M3 Persistence | 010–014 | 5/5 | **DONE** |
| M4 Queue | 015–018 | 4/4 | **DONE** |

## Deliverables

### M3 — Persistence (`MarlinPrintMiddleware.Persistence`)
- `Migrations/001_initial.sql` — print_jobs, printer_profiles, settings, schema_version
- `DatabaseMigrator` + `DatabaseInitializer` (runs on app start)
- `PrintJobRepository`, `PrinterProfileRepository`, `SettingsRepository`
- DB path: `%AppData%/EnderAssist/data.db`
- **10 persistence tests**

### M4 — Queue (`MarlinPrintMiddleware.Queue`)
- `GCodeParser` — streaming read, Cura metadata, comment skipping
- `PrintStateMachine` — validated lifecycle transitions
- `PrintQueueService` — enqueue, start, pause, resume, cancel, auto-start
- Wired in `HostBootstrap` via `AddPersistence()` + `AddPrintQueue()`
- **8 queue tests** (+ sample G-code fixture)

## Verification

```text
dotnet build  → 0 errors
dotnet test   → 60 passed (10 Persistence + 9 Queue + 37 Serial + 4 other)
```

## Session Timeline

| Time | Event |
|------|-------|
| 2026-06-27 | M3/M4 sprint started |
| 2026-06-27 | M3 schema, migrations, repositories implemented |
| 2026-06-27 | M4 parser, state machine, queue service implemented |
| 2026-06-27 | DI wired, stubs removed, 60 tests passing |

## Next Sprint Recommendation

**M5 Monitoring & Safety** — TASK-019, 022 can start in parallel.
