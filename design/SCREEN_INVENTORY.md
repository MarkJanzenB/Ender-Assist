# Ender Assist — Screen Inventory

Aligned with reference mockup (premium dashboard layout).

---

## Shell (persistent)

| ID | Name | Contents |
|----|------|----------|
| `SHELL-01` | Nav rail | Logo, nav items, printer card, version |
| `SHELL-02` | Top status strip | Status hero, nozzle/bed/fan chips, connect actions |
| `SHELL-03` | Status footer | Connection, progress %, line, elapsed, temps |
| `SHELL-04` | Thermal banner | Conditional warning |

---

## Sections (nav)

| ID | Name | Layout | MVP |
|----|------|--------|-----|
| `SEC-01` | Dashboard | 2×2 grid + console | **Phase A** |
| `SEC-02` | Queue | Full-width queue focus | Phase A (subset) |
| `SEC-03` | Terminal | Full console | Phase B |
| `SEC-04` | Macros | Macro grid | Phase D |
| `SEC-05` | History | Completed job table | Phase D |
| `SEC-06` | Settings | Form card | **Phase A** (exists) |

---

## Dashboard Panels

| ID | Panel | Data source | MVP |
|----|-------|-------------|-----|
| `PNL-01` | Print Queue | `IPrintQueueService` | Phase A |
| `PNL-02` | Active Job | `PrinterStatus` + queue | Phase A |
| `PNL-03` | Live Monitor | `PrinterStatus`, temps | Phase A |
| `PNL-04` | Control | `ISerialEngine.SendCommandAsync` | Phase B |
| `PNL-05` | Console | `LineReceived` + send | Phase B |

---

## Dialogs / Overlays

| ID | Name | Trigger |
|----|------|---------|
| `DLG-01` | Confirm E-Stop | E-STOP / F12 |
| `DLG-02` | Confirm Cancel | Cancel print |
| `DLG-03` | Move Axis jog | Control tile |
| `OVR-01` | Drop zone overlay | Drag over queue |
| `OVR-02` | Busy overlay | Connect / add file |

---

## Nav Rail Items (order)

1. Dashboard  
2. Queue  
3. Terminal  
4. Macros  
5. History  
6. Settings  

*Reference mockup also shows "Files" — merged into Queue (file pick + drop) for v1 to reduce scope.*

---

## Printer Info Card (nav footer)

| Field | Source |
|-------|--------|
| Model | Profile name / "Ender 3 v2" |
| Connection | `ConnectionState` |
| Port | `SelectedPort.PortName` |
| Baud | `BaudRate` |
| Firmware | `FirmwareInfo` |
| Board | Parsed from handshake if available, else `—` |

---

## Top Status Strip Metrics

| Metric | Source | Phase |
|--------|--------|-------|
| Nozzle actual/target | `PrinterStatus` | A |
| Bed actual/target | `PrinterStatus` | A |
| Fan % | Not in backend | C (show `—`) |
| Status hero | `PrintState` + `ConnectionState` | A |

---

## Implementation Roadmap

### Phase A — Shell + Dashboard (existing features)
- Nav rail + section switching  
- Top status strip (always visible)  
- Dashboard grid with Queue, Active Job, Live Monitor  
- Footer strip  
- Settings section  
- Preserve all QA UX behaviors  

### Phase B — Operator tools
- Console panel + Terminal section  
- Control panel G-code tiles  
- Position display (extend `PrinterStatus`)  

### Phase C — Data enrichment
- Fan speed parsing  
- Layer / filament from G-code  
- Drag-drop + queue reorder  

### Phase D — Power features
- Macros persistence  
- History view  
- Light theme  

---

## Four-Agent Review (Screen Inventory)

| Agent | Question | Answer |
|-------|----------|--------|
| Product Designer | Workflow optimal? | ✅ Dashboard default; deep sections on demand |
| Visual Designer | Hierarchy clear? | ✅ Strip > panels > footer |
| UX Engineer | Interactions predictable? | ✅ One section per nav item |
| Desktop UI Engineer | Realistic? | ✅ Phased; Phase A uses existing ViewModel |

**Unanimous approval** to proceed to Phase A implementation after doc sign-off.
