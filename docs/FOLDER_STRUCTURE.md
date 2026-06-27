# Folder Structure

## Repository Root

```text
MarlinPrintMiddleware/
├── docs/                    # Project documentation (this layer)
├── tasks/                   # Task definitions (source of work)
├── issues/                  # Defect and blocker tracking
├── progress/                # Task state boards (authoritative status)
├── decisions/               # Architecture Decision Records (ADRs)
├── src/                     # Application source (created during implementation)
├── tests/                   # Test projects (created during implementation)
├── .gitignore
└── README.md                # Created in TASK-035
```

## Documentation (`/docs`)

| File | Purpose |
|------|---------|
| PROJECT_SCOPE.md | Goals, scope, milestones |
| ARCHITECTURE.md | System design and layer diagram |
| TECH_STACK.md | Languages, libraries, build |
| CODING_STANDARDS.md | Conventions and rules |
| FOLDER_STRUCTURE.md | This file |
| DEPENDENCY_GRAPH.md | Task dependency DAG |
| AGENT_ASSIGNMENTS.md | Role-to-task matrix |

## Project Management (`/tasks`, `/issues`, `/progress`, `/decisions`)

- **tasks/** — One markdown file per task (`TASK-NNN-slug.md`)
- **issues/** — One markdown file per defect (`ISSUE-NNN-slug.md`); never deleted
- **progress/** — Four kanban files listing task IDs by state
- **decisions/** — ADR files for major architectural choices

## Source (`/src`) — Planned

```text
src/
├── MarlinPrintMiddleware.App/
│   ├── App.xaml
│   ├── App.xaml.cs
│   ├── HostBootstrap.cs
│   └── appsettings.json
│
├── MarlinPrintMiddleware.Core/
│   ├── Models/
│   ├── Enums/
│   ├── Interfaces/
│   └── Events/
│
├── MarlinPrintMiddleware.Serial/
│   ├── SerialEngine.cs
│   ├── SerialPortDiscovery.cs
│   ├── MarlinHandshake.cs
│   ├── OkSynchronizer.cs
│   └── Parsers/
│
├── MarlinPrintMiddleware.Queue/
│   ├── GCodeParser.cs
│   ├── PrintQueueService.cs
│   ├── PrintStateMachine.cs
│   └── GCodeStreamReader.cs
│
├── MarlinPrintMiddleware.Monitoring/
│   ├── TemperatureMonitor.cs
│   ├── PositionTracker.cs
│   └── PrinterStatusAggregator.cs
│
├── MarlinPrintMiddleware.Safety/
│   ├── EmergencyStopService.cs
│   ├── PauseResumeService.cs
│   └── SafetyPolicy.cs
│
├── MarlinPrintMiddleware.Persistence/
│   ├── SqliteConnectionFactory.cs
│   ├── Migrations/
│   └── Repositories/
│
└── MarlinPrintMiddleware.UI/
    ├── Views/
    ├── ViewModels/
    ├── Converters/
    └── Resources/
```

## Tests (`/tests`) — Planned

```text
tests/
├── MarlinPrintMiddleware.Core.Tests/
├── MarlinPrintMiddleware.Serial.Tests/
│   └── Fixtures/            # Recorded Marlin session transcripts
├── MarlinPrintMiddleware.Queue.Tests/
│   └── SampleGCode/         # Sample .gcode files
├── MarlinPrintMiddleware.Persistence.Tests/
└── MarlinPrintMiddleware.Integration.Tests/
```

## Naming Conventions for PM Files

| Type | Pattern | Example |
|------|---------|---------|
| Task | `TASK-NNN-kebab-slug.md` | `TASK-004-serial-port-discovery.md` |
| Issue | `ISSUE-NNN-kebab-slug.md` | `ISSUE-001-usb-reconnect-failure.md` |
| ADR | `ADR-NNN-kebab-slug.md` | `ADR-001-use-sqlite.md` |

## Context Recovery Protocol

On any new session, read in order:

1. `/progress/*.md` — current task states
2. `/docs/PROJECT_SCOPE.md` — goals
3. `/docs/DEPENDENCY_GRAPH.md` — what can be started next
4. `/tasks/TASK-NNN-*.md` — details for available TODO tasks
5. `/issues/` — open blockers
6. `/decisions/` — architectural constraints
