# Ender Assist — Interaction Rules

---

## 1. Global Principles

1. **One primary action per region** — e.g., Connect in status strip; Start in active job panel.  
2. **Destructive actions require confirmation** — E-stop (M112), Cancel print.  
3. **Feedback within 100ms** — button press visual; within 500ms — status text update.  
4. **Errors persist** — activity log retains errors; status line may update.  
5. **No silent binding failures** — invalid baud/port shows validation message.

---

## 2. Connection Flow

| Step | User action | System response | UI state |
|------|-------------|-----------------|----------|
| 1 | Select port + baud | — | Connect enabled |
| 2 | Click Connect | `IsBusy=true`, strip shows CONNECTING | Connect disabled |
| 3 | Handshake success | Status → Connected, firmware populated | Metrics poll begins |
| 4 | Handshake fail | Dialog or banner with error | Connection Error chip |
| 5 | Disconnect | Clean shutdown | Temps → `—`, IDLE |

**Rule:** Never allow double-connect (`CanConnect` checks `Connecting` state).

---

## 3. Queue Interactions

| Action | Trigger | Confirmation | Result |
|--------|---------|--------------|--------|
| Add file | Button or drop | No | Analyze → enqueue → log |
| Start | Button | No | Starts **selected** pending job |
| Pause / Resume | Button | No | Only when `CanExecute` |
| Cancel | Button | Yes | Stops active print |
| Remove | Button | No (pending only) | Deletes from queue |
| Retry | Button | No | Failed → pending |
| Clear done | Button | Optional soft confirm | Removes completed/cancelled |
| Reorder | Drag row | No | Phase C — updates `QueueOrder` |

**Add G-code loading:** Show indeterminate bar in queue panel + disable Add button.

---

## 4. Print Monitoring

- Temperatures update on poll interval (no user action).  
- `—` displayed until first valid M105 parse.  
- Heating pills appear when `actual < target - 2°C`.  
- Thermal warning banner: persists until Dismiss; also logged.  

---

## 5. Control Panel

| Tile | On click | Loading | Error |
|------|----------|---------|-------|
| Home All | Send `G28` | Tile spinner until `ok` | Toast + log |
| Move Axis | Open jog flyout | — | — |
| Disable Motors | `M84` | Brief spinner | Toast |
| Preheat PLA/PETG | Script | Until temps rising | Thermal warning if runaway |
| Cooldown | `M104 S0` / `M140 S0` | Brief | — |

All disabled when disconnected.

---

## 6. Console

| Key | Action |
|-----|--------|
| Enter | Send command |
| Up/Down | Command history (Phase B) |
| Ctrl+L | Clear log |
| Ctrl+F | Focus filter |

Auto-scroll when pinned to bottom; pause auto-scroll if user scrolls up.

---

## 7. Navigation

- Click nav item → switch section without losing connection.  
- Dashboard nav returns to grid layout.  
- Settings changes require explicit Save (unsaved indicator).  
- Keyboard: `Ctrl+1`…`Ctrl+6` for nav items (Phase B).  

---

## 8. Emergency Stop

1. User clicks E-STOP or **F12**  
2. Modal: "M112 requires full printer reset…" — default **No**  
3. On Yes: send M112, cancel queue, latch E-stop service  
4. UI: FAILED/ERROR state, red banner, log entry  

E-STOP button **never** adjacent to Cancel without visual separator (min 24px + danger zone).

---

## 9. Motion & Animation

| Element | Animation |
|---------|-----------|
| Nav selection | Background fade 120ms |
| Button hover | Background 120ms |
| Progress bar | Value change 200ms ease |
| Connecting dot | Opacity pulse 1.2s loop |
| Thermal banner | Slide down 200ms |

**No animation** on temperature numeric changes (distracting during print).

---

## 10. Empty & Error States

| Context | Empty | Error |
|---------|-------|-------|
| Queue | Illustration + "Add G-code" CTA | — |
| Console | "No messages yet" | Red filter tab |
| Ports | CH340 driver hint | — |
| Active job | Gauge icon + "No active job" | Failed job message |

---

## 11. Loading States Summary

| Surface | Loading indicator |
|---------|-------------------|
| Connect | Status strip CONNECTING + thin indeterminate bar |
| Add G-code | Queue panel overlay spinner |
| Save settings | Button "Saving…" disabled |
| Control tile | Per-tile spinner |

---

## Four-Agent Sign-Off

| Agent | Verdict |
|-------|---------|
| Product Designer | ✅ Workflows minimize clicks for frequent actions |
| Visual Designer | ✅ Motion restrained for long-print calm |
| UX Engineer | ✅ All states specified with timings |
| Desktop UI Engineer | ✅ Achievable with MVVM + `CanExecute` |
