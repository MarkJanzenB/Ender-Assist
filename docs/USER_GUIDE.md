# Ender Assist — User Guide

## Requirements

- Windows 10/11 x64
- USB cable to Ender 3 V2 (or other Marlin printer)
- CH340/CP2102 USB serial driver installed
- G-code file exported from Cura (or other slicer)

## Running the Application

### Development build

```powershell
dotnet run --project src/MarlinPrintMiddleware.App
```

### Published EXE (single-file, self-contained)

```powershell
dotnet publish src/MarlinPrintMiddleware.App -c Release -r win-x64
```

Run: `src/MarlinPrintMiddleware.App/bin/Release/net8.0-windows/win-x64/publish/EnderAssist.exe`

## Connecting Your Printer

1. Power on the printer and connect USB.
2. Open **Ender Assist**.
3. Click **Refresh** to list COM ports.
4. Select your port (often `COM3` or similar).
5. Baud rate defaults to **115200** (Ender 3 V2).
6. Click **Connect**. Firmware info appears after handshake.

## Printing

1. Click **Add G-code** and select a `.gcode` file from Cura.
2. Click **Start** to begin the first queued job.
3. Monitor hotend/bed temperatures and progress on the right panel.
4. Use **Pause** / **Resume** as needed.
5. **Cancel** stops the current job; **E-STOP** sends `M112` immediately.

## Settings

- **Auto-start next job** — when enabled, the next queued file starts automatically after the current job completes.

## Troubleshooting

| Problem | Solution |
|---------|----------|
| No COM ports | Install CH340 driver; try another USB port |
| Connect fails | Close Cura/OctoPrint; verify baud 115200 |
| Print stalls | Check USB cable; watch for thermal warnings |
| App won't start | Install [.NET 8 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0) if using framework-dependent build |

## Data Location

Print queue and settings: `%AppData%\EnderAssist\data.db`
