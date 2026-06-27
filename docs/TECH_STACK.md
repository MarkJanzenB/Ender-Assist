# Technology Stack

## Runtime and Framework

| Component | Choice | Version | Rationale |
|-----------|--------|---------|-----------|
| Language | C# | 12 (.NET 8) | User requirement; strong Windows desktop ecosystem |
| Runtime | .NET | 8.0 LTS | User requirement; long-term support |
| UI Framework | WPF | .NET 8 | User requirement; mature MVVM support on Windows |
| Architecture Pattern | MVVM | — | Separation of UI and logic; testable ViewModels |
| Database | SQLite | via Microsoft.Data.Sqlite | User requirement; zero-config local persistence |
| Serial I/O | System.IO.Ports | .NET 8 built-in | User requirement; no extra dependency for USB serial |

## Supporting Libraries (Approved)

| Library | Purpose | Notes |
|---------|---------|-------|
| CommunityToolkit.Mvvm | MVVM helpers (`ObservableObject`, commands) | Reduces boilerplate; ADR-002 |
| Microsoft.Extensions.DependencyInjection | DI container | Standard .NET DI |
| Microsoft.Extensions.Hosting | App lifecycle, background services | Host serial worker and monitors |
| Microsoft.Data.Sqlite | SQLite access | Official provider |
| xUnit | Unit/integration tests | .NET standard |
| Moq | Mocking in tests | Interface-based design |
| FluentAssertions | Readable test assertions | Test readability |

## Development Tools

| Tool | Purpose |
|------|---------|
| Visual Studio 2022 / Rider / VS Code + C# Dev Kit | IDE |
| dotnet CLI | Build, test, publish |
| Git | Version control |

## Target Platform

- **OS:** Windows 10 (1903+) / Windows 11 x64
- **Deployment:** Self-contained or framework-dependent publish
- **Minimum RAM:** 512 MB (application only)

## Solution Structure (Planned)

```text
MarlinPrintMiddleware.sln
├── src/
│   ├── MarlinPrintMiddleware.App/          # WPF host, DI bootstrap
│   ├── MarlinPrintMiddleware.Core/         # Domain models, interfaces
│   ├── MarlinPrintMiddleware.Serial/       # Serial port, Marlin protocol
│   ├── MarlinPrintMiddleware.Queue/        # G-code parsing, queue engine
│   ├── MarlinPrintMiddleware.Monitoring/   # Temperature, progress
│   ├── MarlinPrintMiddleware.Safety/       # E-stop, pause/resume
│   ├── MarlinPrintMiddleware.Persistence/  # SQLite repositories
│   └── MarlinPrintMiddleware.UI/           # Views, ViewModels, resources
└── tests/
    ├── MarlinPrintMiddleware.Core.Tests/
    ├── MarlinPrintMiddleware.Serial.Tests/
    ├── MarlinPrintMiddleware.Queue.Tests/
    ├── MarlinPrintMiddleware.Persistence.Tests/
    └── MarlinPrintMiddleware.Integration.Tests/
```

## Serial Protocol

- **Transport:** USB virtual COM port
- **Default baud:** 115200, 8N1, no handshake
- **Line ending:** `\n` (Marlin standard)
- **Flow control:** Wait for `ok` or `busy` before sending next line (buffer-aware)
- **Init sequence:** Open port → `M115` (firmware info) → optional `M105` (temps)

## Database

- **Engine:** SQLite 3
- **File location:** `%AppData%/MarlinPrintMiddleware/data.db`
- **Migrations:** Versioned SQL scripts applied on startup

## Configuration

- **User settings:** SQLite `settings` table + optional JSON override for dev
- **Printer profiles:** SQLite `printer_profiles` table

## Build Commands

```powershell
dotnet restore
dotnet build
dotnet test
dotnet publish src/MarlinPrintMiddleware.App -c Release -r win-x64 --self-contained
```

## Non-Goals for Stack Selection

- No Electron / web wrapper
- No Entity Framework (lightweight ADO.NET/Dapper-style or raw SQL preferred for v1)
- No third-party serial libraries unless a documented blocker arises (see ADR-004)
