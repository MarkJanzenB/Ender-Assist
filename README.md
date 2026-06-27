# Ender Assist

Windows desktop middleware between Cura (G-code) and Marlin-based 3D printers.

**GitHub:** [MarkJanzenB/Ender-Assist](https://github.com/MarkJanzenB/Ender-Assist)

**Primary target:** Creality Ender 3 V2 (4.2.2 board)

## Status

**All milestones M1–M7 complete.** Ready to run as `EnderAssist.exe`.

| Milestone | Status |
|-----------|--------|
| M1–M4 Foundation, Serial, Persistence, Queue | DONE |
| M5 Monitoring & Safety | DONE |
| M6 WPF UI | DONE |
| M7 QA & Docs | DONE |
| M8 Hardware validation | Manual |

```text
dotnet build && dotnet test   # 62 tests passing
```

## Run the App

```powershell
# Development
dotnet run --project src/MarlinPrintMiddleware.App

# Or run Debug EXE directly
.\src\MarlinPrintMiddleware.App\bin\Debug\net8.0-windows\EnderAssist.exe

# Release single-file EXE
.\publish.ps1
.\src\MarlinPrintMiddleware.App\bin\Release\net8.0-windows\win-x64\publish\EnderAssist.exe
```

## Architecture

```text
Cura → Ender Assist (EXE) → USB Serial → Marlin Printer
```

## Documentation

- [User Guide](docs/USER_GUIDE.md)
- [Developer Guide](docs/DEVELOPER_GUIDE.md)
- [Architecture](docs/ARCHITECTURE.md)
