# Architecture

## Overview

MarlinPrintMiddleware is a layered desktop application. The WPF UI observes state through ViewModels; all printer interaction flows through a serial engine guarded by safety policies; job orchestration is handled by a queue engine backed by SQLite persistence.

```text
┌─────────────────────────────────────────────────────────┐
│                    WPF UI Layer                          │
│  Views  ←→  ViewModels  ←→  Application Services         │
└──────────────────────────┬──────────────────────────────┘
                           │ commands / events
┌──────────────────────────▼──────────────────────────────┐
│                   Application Layer                        │
│  PrintOrchestrator │ ConnectionService │ SettingsService   │
└──────┬───────────────┬─────────────────┬──────────────────┘
       │               │                 │
┌──────▼──────┐ ┌──────▼──────┐ ┌───────▼────────┐
│ Queue Engine│ │  Monitoring │ │ Safety Engine  │
└──────┬──────┘ └──────┬──────┘ └───────┬────────┘
       │               │                 │
       └───────────────┼─────────────────┘
                       │
              ┌────────▼────────┐
              │  Serial Engine   │
              │ (background worker)│
              └────────┬────────┘
                       │ System.IO.Ports
              ┌────────▼────────┐
              │  Marlin Printer  │
              └─────────────────┘

       ┌─────────────────────────┐
       │   Persistence (SQLite)   │
       │  jobs │ queue │ profiles │
       └─────────────────────────┘
```

## Layer Responsibilities

### MarlinPrintMiddleware.App

- Application entry point (`App.xaml.cs`)
- DI container registration (`HostBuilder`)
- Background service registration
- Navigation and window management

### MarlinPrintMiddleware.Core

- Domain entities: `PrintJob`, `PrinterProfile`, `PrinterStatus`, `GCodeLine`
- Enums: `ConnectionState`, `PrintState`, `JobPriority`
- Interfaces consumed by all layers (dependency inversion)
- No references to UI, Serial, or SQLite

### MarlinPrintMiddleware.Serial

- Port discovery (`SerialPort.GetPortNames()` + optional WMI enrichment)
- Connection lifecycle (open, close, reconnect)
- Marlin protocol: handshake (`M115`), line sending, response parsing
- `ok` / `busy` / `Error:` synchronization
- Dedicated background worker thread or `BackgroundService` (see ADR-003)
- Raises events: `LineReceived`, `ConnectionStateChanged`, `ErrorReceived`

### MarlinPrintMiddleware.Queue

- G-code file import and validation
- In-memory queue with persisted backing
- State machine: `Idle → Preparing → Printing → Paused → Completed / Failed / Cancelled`
- Computes progress from bytes/lines sent vs total
- Requests lines from serial engine at safe rate

### MarlinPrintMiddleware.Monitoring

- Polls `M105` / parses autoreport temperatures when enabled
- Tracks `M114` position on demand
- Aggregates `PrinterStatus` snapshot for UI binding
- Configurable poll intervals (respects serial engine lock)

### MarlinPrintMiddleware.Safety

- Emergency stop: immediate `M112` + queue halt
- Pause: `M125` or `M600` depending on profile; holds queue
- Resume: validates temperature and state before continuing
- Prevents conflicting commands (e.g., send G-code during estop)

### MarlinPrintMiddleware.Persistence

- SQLite connection factory
- Schema migrations (version table)
- Repositories: `IPrintJobRepository`, `IPrinterProfileRepository`, `ISettingsRepository`
- Unit of work for transactional queue updates

### MarlinPrintMiddleware.UI

- XAML views, resource dictionaries, converters
- ViewModels using `CommunityToolkit.Mvvm`
- No direct `SerialPort` access from ViewModels

## Key Interfaces (Core)

```csharp
public interface ISerialEngine
{
    Task ConnectAsync(string portName, int baudRate, CancellationToken ct);
    Task DisconnectAsync(CancellationToken ct);
    Task<string> SendCommandAsync(string command, CancellationToken ct);
    IAsyncEnumerable<string> ReadLinesAsync(CancellationToken ct);
    ConnectionState State { get; }
    event EventHandler<SerialLineEventArgs> LineReceived;
}

public interface IPrintQueueService
{
    Task EnqueueAsync(PrintJob job, CancellationToken ct);
    Task StartNextAsync(CancellationToken ct);
    Task PauseAsync(CancellationToken ct);
    Task ResumeAsync(CancellationToken ct);
    Task CancelAsync(CancellationToken ct);
    PrintQueueSnapshot GetSnapshot();
}

public interface IPrintJobRepository
{
    Task<PrintJob?> GetByIdAsync(long id);
    Task<IReadOnlyList<PrintJob>> GetPendingAsync();
    Task UpsertAsync(PrintJob job);
}
```

## Threading Model

| Component | Thread |
|-----------|--------|
| UI | WPF dispatcher thread |
| SerialEngine | Dedicated background thread (ADR-003) |
| Monitoring | Timer-driven via `BackgroundService` |
| Queue | Invoked from serial callbacks + UI commands; uses locks |
| Persistence | Async I/O on thread pool |

**Rule:** Only the serial engine writes to `SerialPort`. All other layers submit commands through its async API.

## State Management

- **Connection state:** `Disconnected → Connecting → Connected → Error`
- **Print state:** Managed by queue state machine (see Queue layer)
- **UI binding:** ViewModels subscribe to `INotifyPropertyChanged` on services

## Error Handling

- Serial timeouts → retry with backoff (configurable, max 3)
- USB disconnect → transition to `Error`, persist queue position, notify UI
- Marlin `Error:` responses → log, optionally pause job (SafetyEngine decision)
- Unhandled exceptions in background services → log, surface toast in UI

## Security

- Local-only application; no network exposure in v1
- G-code paths validated to prevent directory traversal on import
- No elevation required; standard user COM port access

## Extensibility

- `IPrinterProfile` allows per-printer baud, buffer size, and command overrides
- `IFirmwareCapabilityDetector` parses `M115` response for feature flags
- Future: plugin interface for alternate transports (OctoPrint, Klipper)

## Related Documents

- [TECH_STACK.md](TECH_STACK.md)
- [FOLDER_STRUCTURE.md](FOLDER_STRUCTURE.md)
- [DEPENDENCY_GRAPH.md](DEPENDENCY_GRAPH.md)
- [../decisions/](../decisions/) — Architecture Decision Records
