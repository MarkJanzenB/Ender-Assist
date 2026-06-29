# Ender Assist — Layout Rules

---

## 1. Application Shell

```
┌──────────┬──────────────────────────────────────────────────────┐
│          │  TOP STATUS STRIP (fixed height, auto)                │
│   NAV    ├──────────────────────────────────────────────────────┤
│   RAIL   │                                                       │
│  240px   │              MAIN CONTENT AREA                        │
│  fixed   │         (dashboard grid OR section view)              │
│          │                                                       │
│          ├──────────────────────────────────────────────────────┤
│ [printer]│  STATUS FOOTER (32px)                                 │
└──────────┴──────────────────────────────────────────────────────┘
```

| Region | Width | Min height |
|--------|-------|------------|
| Nav rail | 240px fixed (collapsible to 64px icons-only in v2) | 100% |
| Content | `*` star | 400px |
| Status strip | 100% | 72px |
| Footer | 100% | 32px |

**Window minimum:** 1100 × 700 (supports 100% DPI).  
**Optimal:** 1280 × 860+.

---

## 2. Nav Rail

- Logo + product name top (16px padding)  
- Nav items: 44px row height, 12px vertical gap  
- Active indicator: 3px left accent bar  
- Printer card: pinned bottom, 16px margin, contains model, connection, COM, baud, firmware, board  
- Version label: `v1.0.0` caption at rail bottom  

**No horizontal scroll.** Rail does not resize with window.

---

## 3. Top Status Strip

Three zones (left → right):

1. **Status hero** (flex 1) — IDLE/PRINTING + subtext  
2. **Metric chips** (fixed) — Nozzle | Bed | Fan — equal width ~140px each  
3. **Actions** (auto) — Connect, Disconnect, utility icons (settings shortcut)  

Must remain visible when navigating to Queue, Terminal, etc. (shell-level, not page-level).

---

## 4. Dashboard Grid (default view)

**Breakpoint ≥ 1100px** — 2×2 asymmetric grid:

```
┌─────────────────────┬──────────────────┐
│   PRINT QUEUE       │   ACTIVE JOB     │
│   (col 3*, row *)   │   (col 2*)       │
├─────────────────────┼──────────────────┤
│   LIVE MONITOR      │   CONTROL        │
│   (col 3*)          │   (col 2*)       │
├─────────────────────┴──────────────────┤
│   CONSOLE (full width, row auto,       │
│   min 160px, max 240px height)         │
└────────────────────────────────────────┘
```

Column ratio: **3:2** (queue/monitor wider).

**Breakpoint 900–1099px:** Stack to single column; order preserved.  
**Breakpoint < 900:** Not supported (enforce `MinWidth`).

---

## 5. Section Views (nav destinations)

| Section | Layout |
|---------|--------|
| Dashboard | Grid above |
| Queue | Full-width queue panel + footer transport |
| Terminal | Full-width console (log + input) |
| Macros | Grid of user-defined macro cards (Phase D) |
| History | Filterable job table (Phase D) |
| Settings | Centered form card max-width 560px |

Only **one** main section visible; dashboard is default landing.

---

## 6. Spacing & Alignment

- Outer content padding: **20px**  
- Inter-card gap: **12px**  
- Card internal padding: **20px**  
- Align all card titles to same baseline grid  

---

## 7. Responsive / DPI

- Use `Viewbox` only for icons, not text  
- Prefer `Grid` star sizing over fixed pixel widths in content area  
- `UseLayoutRounding="True"`, `SnapsToDevicePixels="True"` on shell  
- Test at 100%, 125%, 150%, 200% Windows scaling  
- Touch targets remain ≥ 44 DIP at all scales  

---

## 8. Z-Order & Overlays

- Thermal warning: banner below status strip, above content  
- Drop zone overlay: z-index above queue empty state only  
- Modals: centered, dimmed shell overlay `#000000` @ 60%  

---

## 9. Content Priority (visual weight)

1. Printer status + temperatures (largest)  
2. Active print progress  
3. Queue + transport controls  
4. Console + control tiles  
5. Metadata (firmware, version)  

---

## Four-Agent Sign-Off

| Agent | Verdict | Note |
|-------|---------|------|
| Product Designer | ✅ | Status always visible; dashboard matches mockup |
| Visual Designer | ✅ | Grid proportions match reference image |
| UX Engineer | ✅ | Resize breakpoints defined |
| Desktop UI Engineer | ✅ | WPF Grid/DockPanel feasible; nav 240px fixed |
