# Coding Standards

## General Principles

1. **Task-gated development** — No production code without an open task in `/tasks`.
2. **Test alongside implementation** — Every task includes tests per its test requirements.
3. **Interface-first** — Core abstractions live in `MarlinPrintMiddleware.Core`; implementations in feature projects.
4. **Async by default** — Use `async`/`await` for I/O; avoid blocking the UI thread.
5. **Fail visibly** — Log errors; propagate meaningful messages to ViewModels for user display.

## Naming Conventions

| Element | Convention | Example |
|---------|------------|---------|
| Namespace | `MarlinPrintMiddleware.{Layer}` | `MarlinPrintMiddleware.Serial` |
| Interface | `I` + PascalCase | `ISerialEngine` |
| Class | PascalCase | `SerialEngine` |
| Method | PascalCase async suffix | `ConnectAsync` |
| Private field | `_camelCase` | `_serialPort` |
| Constant | PascalCase | `DefaultBaudRate` |
| Enum | PascalCase singular | `ConnectionState` |
| XAML file | PascalCase | `ConnectionPanelView.xaml` |
| ViewModel | `{Feature}ViewModel` | `ConnectionPanelViewModel` |

## Project Rules

- One public type per file (exceptions: small nested types, enums)
- Max method length ~40 lines; extract helpers when exceeded
- No `async void` except event handlers
- Use `CancellationToken` on all public async methods
- Prefer `IReadOnlyList<T>` over `List<T>` in return types

## MVVM Rules

- Views contain no business logic (code-behind limited to `InitializeComponent` and view-specific UI hooks)
- ViewModels never reference `System.IO.Ports` or `Microsoft.Data.Sqlite`
- Commands use `RelayCommand` / `AsyncRelayCommand` from CommunityToolkit.Mvvm
- Use `[ObservableProperty]` for bindable fields where toolkit is referenced

## Serial Layer Rules

- All `SerialPort` access confined to `MarlinPrintMiddleware.Serial`
- Send one command at a time; wait for `ok` before next (unless streaming with buffer accounting)
- Always set `NewLine = "\n"` and `Encoding = ASCII`
- Dispose port on disconnect; never leak handles
- Timeouts must be configurable via settings

## Persistence Rules

- Parameterized SQL only — no string concatenation for queries
- Migrations are sequential, numbered, and idempotent where possible
- Repository methods return domain types from Core, not `DataRow`
- Transactions for multi-table updates (e.g., queue reorder)

## Logging

- Use `Microsoft.Extensions.Logging.ILogger<T>`
- Levels: `Debug` for serial line trace (toggle), `Information` for state changes, `Warning` for recoverable errors, `Error` for failures
- Never log full G-code files at Info level

## Error Types

- Use custom exceptions sparingly: `SerialConnectionException`, `MarlinProtocolException`, `PrintQueueException`
- Catch at service boundaries; do not swallow exceptions

## Testing Standards

- Test project mirrors source project names: `{Project}.Tests`
- Arrange-Act-Assert pattern
- Unit tests: no real serial port (mock `ISerialEngine`)
- Integration tests: optional virtual serial port pair or recorded session replay
- Minimum coverage target: 70% on Core, Serial, Queue, Persistence

## Git Commit Messages

```text
type(scope): short description

Optional body explaining why.
```

Types: `feat`, `fix`, `test`, `docs`, `refactor`, `chore`

Reference task ID in body: `TASK-004`

## Code Review Checklist

- [ ] Task file exists and is IN_PROGRESS/DONE
- [ ] Tests added and passing
- [ ] No UI thread blocking
- [ ] Serial access isolated
- [ ] Documentation updated if public API changed
- [ ] Progress files updated

## Forbidden Patterns

- God classes (>500 lines)
- Static mutable state outside DI singletons
- Direct database access from ViewModels
- Polling serial from UI thread
- Hard-coded COM port names in production code

## File Headers

No mandatory file headers. Use XML doc comments on public API surfaces only.
