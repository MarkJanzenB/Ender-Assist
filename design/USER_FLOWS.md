# Ender Assist — User Flows

---

## Flow 1: First Launch → First Print

```
Launch app
  → Dashboard (empty queue, DISCONNECTED)
  → User connects USB
  → Refresh ports (if empty: driver hint)
  → Select COM + baud (115200 default)
  → Connect
  → Status: CONNECTED, temps show — then live values
  → Add G-code (dialog or drop)
  → Job appears in queue (selected)
  → Start
  → Active Job panel: progress, elapsed, ETA
  → Live Monitor: temps, layer* (*when available)
  → Complete → job status Completed
```

**Primary actions:** Connect → Add → Start (3 clicks minimum after file chosen).

---

## Flow 2: Daily Workshop Check-In

```
Launch
  → Auto-restore last COM/baud from profile (Settings)
  → Connect
  → Glance top strip: IDLE + temps
  → Proceed to queue or start next job
```

**Design requirement:** Status strip answers "is printer ready?" without navigation.

---

## Flow 3: Mid-Print Monitoring (across room)

```
User at distance
  → Read StatusHero: PRINTING
  → Read large progress % (Active Job or strip)
  → Read nozzle/bed chips for thermal issues
  → Optional: thermal warning banner if anomaly
```

**No interaction required** — passive monitoring.

---

## Flow 4: Pause → Swap Filament → Resume

```
Printing
  → Pause (toolbar)
  → Status: PAUSED
  → User performs manual filament change (off-app)
  → Resume
  → Status: PRINTING
```

**E-stop not used** for normal pause.

---

## Flow 5: Emergency Stop

```
Any state (connected)
  → E-STOP or F12
  → Confirm dialog (default No)
  → M112 sent
  → Queue cancelled, latched
  → User power-cycles printer (documented in UI)
  → Reconnect
```

---

## Flow 6: Failed Job Recovery

```
Print fails
  → Job row: Failed status
  → User selects job → Retry
  → Job returns to Pending
  → Start when ready
```

---

## Flow 7: Manual Control (jog / preheat)

```
Connected
  → Dashboard or Control section
  → Preheat PLA (one click)
  → Monitor temps in strip
  → Move Axis → jog dialog
  → Home All when done
  → Cooldown
```

**Phase B** backend: direct G-code via `SendCommandAsync`.

---

## Flow 8: Terminal Power User

```
Connected
  → Nav: Terminal
  → Type M114 → Send
  → See RX in log
  → Filter RX only
```

**Phase B** — replaces need for external serial terminal.

---

## Flow 9: Settings Persistence

```
Nav: Settings
  → Toggle auto-start
  → "Unsaved changes" appears
  → Save settings
  → Port + baud + auto-start persisted to profile
```

---

## Navigation Strategy

| User goal | Entry point |
|-----------|-------------|
| Start/monitor print | Dashboard (default) |
| Manage queue only | Queue nav |
| Send raw G-code | Terminal |
| Quick preheat/jog | Control panel on Dashboard |
| Change preferences | Settings |
| Review past prints | History (Phase D) |

**Rule:** 80% of sessions never leave Dashboard.

---

## Four-Agent Sign-Off

| Agent | Verdict |
|-------|---------|
| Product Designer | ✅ Flows optimized for workshop reality |
| Visual Designer | ✅ — |
| UX Engineer | ✅ Edge cases (E-stop, empty ports) covered |
| Desktop UI Engineer | ✅ Flows map to existing + phased services |
