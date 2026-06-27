# Ender Assist

Windows desktop middleware between Cura (G-code) and Marlin-based 3D printers.

**GitHub:** [MarkJanzenB/Ender-Assist](https://github.com/MarkJanzenB/Ender-Assist)

**Primary target:** Creality Ender 3 V2 (4.2.2 board)

## Status

**Phase:** M1–M4 complete. Next: M5 Monitoring & Safety.

| Milestone | Status |
|-----------|--------|
| M1 Foundation | DONE (4/4) |
| M2 Serial | DONE (6/6) |
| M3 Persistence | DONE (5/5) |
| M4 Queue | DONE (4/4) |
| M5 Monitor & Safety | TODO |

```text
dotnet build && dotnet test   # 60 tests passing
```

## Architecture

```text
Cura → Desktop Middleware → USB Serial → Marlin Printer
```

## Tech Stack

- C# / .NET 8 / WPF / MVVM
- SQLite / System.IO.Ports

## Project Management

| Directory | Purpose |
|-----------|---------|
| `/docs` | Architecture and standards |
| `/tasks` | Work items (TASK-001 – TASK-035) |
| `/progress` | Kanban state (TODO, IN_PROGRESS, BLOCKED, DONE) |
| `/issues` | Defect tracking |
| `/decisions` | Architecture Decision Records |

## Context Recovery

1. Read `/progress/*.md` for current state
2. Read `/docs/DEPENDENCY_GRAPH.md` for available work
3. Pick eligible TODO task and set IN_PROGRESS

## Next Task

**TASK-001** — Solution scaffold and project structure (Architect)

## Documentation

- [Project Scope](docs/PROJECT_SCOPE.md)
- [Architecture](docs/ARCHITECTURE.md)
- [Tech Stack](docs/TECH_STACK.md)
- [Dependency Graph](docs/DEPENDENCY_GRAPH.md)
- [Agent Assignments](docs/AGENT_ASSIGNMENTS.md)

## License

TBD
