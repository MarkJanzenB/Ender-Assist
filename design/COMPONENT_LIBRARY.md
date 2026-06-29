# Ender Assist — Component Library

Every component defines **idle, hover, active, disabled, loading, error** states.

---

## Navigation

### `NavRail`

| State | Visual | Behavior |
|-------|--------|----------|
| Idle | `shell-elevated` bg, icons `#94A3B8` | — |
| Hover | Item bg `#1A222C` | Pointer cursor |
| Active | Left 3px `accent` bar + item bg `#1A222C` + text primary | Current section |
| Disabled | Opacity 0.4 | Future: when disconnected for print-only sections |
| Loading | — | N/A |
| Error | — | N/A |

**Items:** Dashboard, Queue, Terminal, Macros, History, Settings  
**Footer:** Printer info card (model, COM, baud, firmware, board, connection chip)

### `PrinterInfoCard` (nav footer)

| State | Visual |
|-------|--------|
| Connected | Green dot + "Connected" |
| Disconnected | Gray dot |
| Connecting | Amber pulse (animated opacity) |
| Error | Red dot + "Error" |

---

## Status Bar (`TopStatusStrip`)

Always visible. Contains:

- `StatusHero` — large IDLE / PRINTING / PAUSED / ERROR label + subtext
- `MetricChip` ×3 — Nozzle, Bed, Fan (actual / target)
- `ConnectionActions` — Connect (primary), Disconnect (ghost)

### `MetricChip`

| State | Visual |
|-------|--------|
| Idle | Card inset, label uppercase, value `metric-xl` |
| Heating | Amber left border + "↑" badge |
| At temp | Green subtle glow on value |
| No data | Em dash `—` (never `0.0`) |
| Error | Red border + tooltip |

### `StatusHero`

| PrintState | Label | Subtext | Color |
|------------|-------|---------|-------|
| Idle | IDLE | Ready to print | Green |
| Printing | PRINTING | {job name} | Accent |
| Paused | PAUSED | Tap Resume | Amber |
| Preparing | PREPARING | Warming up… | Amber |
| Failed | FAILED | See console | Red |

---

## Buttons

### `PrimaryButton`

| State | Style |
|-------|-------|
| Idle | `gradient.accent`, text `#042F2E` |
| Hover | Opacity 0.92 |
| Active | Opacity 0.82, scale 0.98 (RenderTransform) |
| Disabled | Opacity 0.4 |
| Loading | Content → spinner + "Connecting…" |
| Error | — |

### `GhostButton`

Transparent bg; hover `card-hover`.

### `DangerButton` (E-STOP)

| State | Style |
|-------|-------|
| Idle | `gradient.danger` + glow shadow |
| Hover | Brighter red |
| Active | Darker press |
| Disabled | Opacity 0.4 when disconnected |
| Loading | — |
| Error | — |

**Rule:** E-STOP always isolated in `DangerZone` panel; requires confirmation dialog.

---

## Queue

### `QueuePanel`

- Toolbar: Add, Remove, Retry, Clear done  
- `JobRow` list (replaces raw DataGrid styling)  
- Empty state: illustration + CTA + drop zone  

### `JobRow`

| State | Visual |
|-------|--------|
| Idle | Transparent |
| Hover | `card-hover` |
| Selected | `accent-muted` border + left accent bar |
| Printing | Accent progress bar animated |
| Failed | Red status pill |
| Disabled | N/A (read-only list) |

**Columns:** Name, Status, Progress bar, Est. duration*, Filament*, Position*  
\* *Requires backend enrichment — show `—` until available*

### `DropZone`

| State | Visual |
|-------|--------|
| Idle | Dashed border `border.subtle` |
| Hover | Border `accent`, bg `accent-muted` @ 20% |
| Active drop | Solid `accent` border |
| Disabled | Hidden when printing |
| Error | Red border + message (invalid file) |

---

## Active Job Panel

- Job name, circular or linear progress, elapsed, ETA, layer*  
- Transport: Start, Pause, Resume, Cancel (contextual `CanExecute`)  
- E-STOP in dedicated danger zone (not adjacent to Cancel)

---

## Live Monitor

- `TempCard` ×2 (nozzle, bed) — large metric + target + heating pill  
- `ProgressRing` or bar with overlaid %  
- `PositionGrid` — X/Y/Z/E color-coded (requires `PrinterStatus` extension)  
- Stats list: Layer, Speed, Flow* (*future*)

---

## Control Panel

Grid of `MacroTile` buttons:

| Tile | Command | Disabled when |
|------|---------|---------------|
| Home All | `G28` | Disconnected |
| Move Axis | Opens jog dialog | Disconnected |
| Disable Motors | `M84` | Disconnected |
| Preheat PLA | `M140 S60` + `M104 S200` | Disconnected |
| Preheat PETG | `M140 S80` + `M104 S230` | Disconnected |
| Cooldown | `M104 S0` + `M140 S0` | Disconnected |

| State | Visual |
|-------|--------|
| Idle | Icon tile on `card` |
| Hover | Elevate + border brighten |
| Active | Inset press |
| Disabled | Opacity 0.35 |
| Loading | Spinner overlay (awaiting `ok`) |
| Error | Red flash + toast |

---

## Console Panel

- `LogStream` — monospace, timestamped, auto-scroll  
- Filter: All / TX / RX / Errors  
- `CommandInput` — single line + Send (Enter)  
- Clear button  

| State | Behavior |
|-------|----------|
| Disconnected | Input disabled, placeholder "Connect to send commands" |
| Connected | Live `LineReceived` append |
| Error | Red line prefix |

---

## Footer (`StatusFooter`)

Single line: connection · progress · line N/M · elapsed · temps  

---

## Dialogs

### `ConfirmEStopDialog`

Warning icon, M112 explanation, Yes/No (default No).

### `ConfirmCancelDialog`

Lighter warning; default No.

---

## Implementation Phases

| Component | Phase | Backend needed |
|-----------|-------|----------------|
| Nav rail, status strip, queue (existing) | A | Minimal |
| Console | B | Wire `LineReceived`, `SendCommandAsync` |
| Control tiles | B | Send G-code commands |
| Fan metric | C | Parse fan from M105 or M123 |
| Layer / filament | C | G-code parser enrichment |
| Drag-drop, reorder | C | UI handlers + queue order API |
| Macros, History | D | New persistence + VMs |

---

## Four-Agent Sign-Off

| Agent | Verdict |
|-------|---------|
| Product Designer | ✅ Components map to user journeys |
| Visual Designer | ✅ Consistent state visual language |
| UX Engineer | ✅ All six states defined per component |
| Desktop UI Engineer | ✅ MVVM-friendly; phased backend alignment |
