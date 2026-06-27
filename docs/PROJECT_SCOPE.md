# Project Scope

## Product Name

**MarlinPrintMiddleware** — Windows desktop middleware between Cura (or any G-code source) and Marlin-based 3D printers.

## Problem Statement

Slicers such as Cura send G-code to printers but do not provide robust queue management, persistent job history, reconnect handling, or operational monitoring suitable for unattended or multi-job workflows. This application fills that gap as a dedicated desktop middleware layer.

## Data Flow

```text
Cura (or G-code file)
        ↓
Desktop Middleware (this application)
        ↓
USB Serial (System.IO.Ports)
        ↓
Marlin Firmware
        ↓
Ender 3 V2 / Creality 4.2.2 (primary target)
```

## Primary Target Hardware

| Item | Specification |
|------|---------------|
| Printer | Creality Ender 3 V2 |
| Mainboard | Creality 4.2.2 |
| Firmware | Marlin (stock or community builds) |
| Connection | USB serial (CH340/CP2102 typical) |
| Baud rate | 115200 (default; configurable) |

## Secondary Support

- Other Marlin 2.x printers over USB serial
- Configurable printer profiles for baud rate, line endings, and capability flags

## In Scope (v1)

- Discover and connect to USB serial ports
- Marlin handshake, firmware identification, and `ok` synchronization
- Stream G-code with flow control (buffer-aware sending)
- Import G-code files and manage a print queue
- Persist jobs, queue state, and printer profiles in SQLite
- Real-time monitoring: temperatures, position, progress, printer state
- Safety controls: pause, resume, cancel, emergency stop
- WPF desktop UI with MVVM
- Unit and integration tests for core subsystems

## Out of Scope (v1)

- Direct Cura plugin integration (user exports G-code manually)
- Wi-Fi / OctoPrint protocol support
- Firmware flashing or EEPROM editing
- Multi-printer simultaneous printing
- Cloud sync or remote access
- Slicing or model editing
- Camera streaming

## Success Criteria

1. Connect reliably to Ender 3 V2 on Windows 10/11 over USB.
2. Print a G-code file end-to-end with correct line pacing and `ok` handling.
3. Survive USB disconnect and reconnect without corrupting queue state.
4. Persist queue and history across application restarts.
5. Display live temperature and print progress in the UI.
6. Emergency stop halts the printer within one command round-trip.

## Assumptions

- Printer runs Marlin-compatible firmware with standard `ok` responses.
- User has appropriate USB drivers installed (CH340/CP2102).
- G-code files are ASCII text, one command per line (standard slicer output).
- Application runs on Windows 10/11 x64 with .NET 8 runtime.

## Constraints

- C# / .NET 8 / WPF only for the desktop client
- SQLite for local persistence
- `System.IO.Ports.SerialPort` for serial I/O (no third-party serial libraries unless ADR-approved)
- MVVM pattern for all UI
- No production code without a corresponding task file

## Milestones

| Milestone | Description | Key Tasks |
|-----------|-------------|-----------|
| M0 | Project management layer | Docs, tasks, ADRs, progress |
| M1 | Solution scaffold and core abstractions | TASK-001 – TASK-003 |
| M2 | Serial communication layer | TASK-004 – TASK-009 |
| M3 | Persistence layer | TASK-010 – TASK-014 |
| M4 | Queue and job management | TASK-015 – TASK-018 |
| M5 | Monitoring and safety | TASK-019 – TASK-024 |
| M6 | WPF UI | TASK-025 – TASK-030 |
| M7 | Testing and documentation | TASK-031 – TASK-035 |
| M8 | Ender 3 V2 validation | Hardware smoke test |

## Agent Roles

See [AGENT_ASSIGNMENTS.md](AGENT_ASSIGNMENTS.md) for the full role matrix.
