# Ender Assist — Developer Guide

## Prerequisites

- .NET 8 SDK
- Windows 10/11 (WPF)
- Visual Studio 2022 or `dotnet` CLI

## Clone and Build

```powershell
git clone https://github.com/MarkJanzenB/Ender-Assist.git
cd Ender-Assist
dotnet restore
dotnet build
dotnet test
```

## Run the Desktop App

```powershell
dotnet run --project src/MarlinPrintMiddleware.App
```

Output EXE (Debug): `src/MarlinPrintMiddleware.App/bin/Debug/net8.0-windows/EnderAssist.exe`

## Publish Release EXE

```powershell
dotnet publish src/MarlinPrintMiddleware.App -c Release -r win-x64
```

Single-file self-contained binary:

`src/MarlinPrintMiddleware.App/bin/Release/net8.0-windows/win-x64/publish/EnderAssist.exe`

## Solution Layout

| Project | Role |
|---------|------|
| `MarlinPrintMiddleware.App` | WPF host, DI bootstrap, **EXE entry** |
| `MarlinPrintMiddleware.UI` | Views, ViewModels (MVVM) |
| `MarlinPrintMiddleware.Core` | Domain models, interfaces |
| `MarlinPrintMiddleware.Serial` | USB serial, Marlin protocol |
| `MarlinPrintMiddleware.Queue` | G-code queue, state machine |
| `MarlinPrintMiddleware.Persistence` | SQLite repositories |
| `MarlinPrintMiddleware.Monitoring` | Temperature, status aggregation |
| `MarlinPrintMiddleware.Safety` | E-stop, pause/resume, thermal hooks |

## Agent Workflow

1. Read `/progress/DONE.md` and `/progress/TODO.md`
2. Pick a task with satisfied dependencies
3. Set task `IN_PROGRESS` → implement → tests → docs → `DONE`
4. Update progress boards

## Context Recovery

Authoritative state lives in:

- `/tasks` — work definitions
- `/progress` — kanban boards
- `/docs` — architecture and standards
- `/decisions` — ADRs

## Milestone Status

| Milestone | Status |
|-----------|--------|
| M1 Foundation | DONE |
| M2 Serial | DONE |
| M3 Persistence | DONE |
| M4 Queue | DONE |
| M5 Monitor/Safety | DONE |
| M6 UI | DONE |
| M7 QA/Docs | DONE |
| M8 Hardware validation | Manual (Ender 3 V2) |
