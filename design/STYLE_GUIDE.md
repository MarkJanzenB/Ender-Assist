# Ender Assist — Style Guide

**Version:** 1.0  
**Date:** 2026-06-27  
**Status:** Approved for Phase 1 (documentation only)

---

## Brand Positioning

Ender Assist is a **workshop manufacturing control center** for the Creality Ender 3 v2 — not a hobby slicer utility. Visual language should feel closer to Bambu Studio, Linear, and industrial HMI panels than to legacy Windows printer dialogs.

**Tone:** Calm, precise, authoritative. The UI should remain readable under fluorescent workshop lighting and at 6–10 feet distance during long prints.

---

## Design North Star

| Attribute | Target |
|-----------|--------|
| Density | High information, low noise |
| Motion | Subtle; never distracting during prints |
| Color | Dark-first; semantic accents only |
| Shape | Soft corners (10–14px cards, 8–10px controls) |
| Depth | Layered surfaces, not heavy borders |
| Typography | Large metrics, small metadata |

---

## Color Philosophy

- **Shell** — near-black blue-gray (`#0A0D12`). Avoid pure `#000000` (harsh on LCD).
- **Surfaces** — stepped elevation: shell → card → inset → overlay.
- **Accent** — single cyan-teal (`#2DD4BF` / `#14B8A6`) for primary actions and focus rings.
- **Semantic** — green = ready/safe, amber = heating/warning, red = danger/E-stop. Never decorative.

Semantic colors must pass **WCAG AAA** against their background when used for text (≥7:1). Status dots and large numerals may use AA for non-text UI (≥3:1).

---

## Typography Philosophy

- **Display / metrics:** Segoe UI Variable Display — light or regular weight at 28–40px for temperatures and progress.
- **UI chrome:** Segoe UI Variable Text — 13–15px semi-bold for labels; 11px uppercase tracking for field labels (`NOZZLE`, `BAUD RATE`).
- **Monospace:** Cascadia Mono for console/serial log only.

Avoid mixing more than two families. No condensed or serif fonts.

---

## Iconography

- **Style:** 1.5px stroke, rounded caps, 20–24px grid. Lucide/Feather-inspired geometry (implemented as WPF `Path` data).
- **Usage:** Icons support labels; never icon-only for safety-critical actions.
- **Color:** Inherit foreground or semantic brush; no multicolor icons except printer brand badge.

---

## Spacing Rhythm

Base unit **4px**. Common steps: 4, 8, 12, 16, 20, 24, 32.

| Context | Padding |
|---------|---------|
| App shell margin | 0 (full bleed rail) |
| Card interior | 20px |
| Control groups | 12px gap |
| Touch targets | min 44×44px |

---

## Elevation & Glass

- Cards: `1px` border `#FFFFFF08` + optional `DropShadow` blur 24, opacity 0.25, depth 4.
- **No** heavy glass blur on WPF (performance); simulate with layered semi-transparent fills.
- Top status bar: `ShellElevated` surface, subtle bottom border.

---

## What We Avoid

- Windows Forms gray (`#F0F0F0` controls on gray panels)
- Purple-gradient “AI dashboard” clichés
- Thick `#533483`-style borders (legacy Ender Assist)
- Skeuomorphic knobs, fake metal textures
- Excessive gradients on every button
- Emoji as primary iconography

---

## Reference Mockup Alignment

The approved layout direction (see `SCREEN_INVENTORY.md`) uses:

1. **Left nav rail** — persistent navigation + printer identity card  
2. **Top status strip** — always-visible IDLE/printing state + nozzle/bed/fan + connect actions  
3. **Dashboard grid** — Queue, Active Job, Live Monitor, Control, Console  
4. **Bottom status footer** — connection summary, line progress, elapsed  

---

## Four-Agent Sign-Off (Style Guide)

| Agent | Verdict | Note |
|-------|---------|------|
| Product Designer | ✅ | Hierarchy supports glanceable status + deep workflows |
| Visual Designer | ✅ | Token set supports premium dark industrial aesthetic |
| UX Engineer | ✅ | Semantic color system maps to all interaction states |
| Desktop UI Engineer | ✅ | WPF-friendly brushes; no GPU-heavy effects required |
