# Ender Assist — UI Audit

**Audited:** `MainWindow.xaml`, `MainViewModel.cs`, `EnderAssistTheme.xaml`  
**Date:** 2026-06-27  
**Baseline:** Post-QA refactor, pre-dashboard redesign

---

## Summary

The current UI is **functionally solid** after the QA pass but **structurally a single-page form** — not a manufacturing dashboard. It lacks persistent status visibility, navigation depth, and the information architecture shown in the target mockup.

| Severity | Count |
|----------|-------|
| Critical | 2 |
| High | 8 |
| Medium | 11 |
| Low | 6 |

---

## Critical

### AUD-001 — No persistent global status strip
| | |
|--|--|
| **Severity** | Critical |
| **Evidence** | Temps live only in Print tab → Live Monitor column |
| **Impact** | Operator must stay on Print tab to monitor; fails "readable from across the room" |
| **Improvement** | `SHELL-02` top strip always visible with nozzle/bed/status hero |

### AUD-002 — No navigation architecture
| | |
|--|--|
| **Severity** | Critical |
| **Evidence** | `TabControl` with Print + Settings only |
| **Impact** | Terminal, macros, history undiscoverable; scales poorly |
| **Improvement** | Left nav rail with section switching per `SCREEN_INVENTORY.md` |

---

## High

### AUD-003 — Connection duplicated / buried
| | |
|--|--|
| **Severity** | High |
| **Evidence** | Full connection card in row 2; status chip in header |
| **Impact** | Wastes vertical space; COM/baud not visible during print |
| **Improvement** | Connect actions in status strip; printer details in nav card |

### AUD-004 — Dashboard not a grid
| | |
|--|--|
| **Severity** | High |
| **Evidence** | 2-column split: queue vs monitor only |
| **Impact** | Missing Active Job, Control, Console panels from target UX |
| **Improvement** | 2×2 grid + console per `LAYOUT_RULES.md` |

### AUD-005 — False drag-drop affordance
| | |
|--|--|
| **Severity** | High |
| **Evidence** | Empty state: "Drop a G-code file" — no handlers |
| **Impact** | Broken user expectation |
| **Improvement** | Implement `AllowDrop` or change copy until Phase C |

### AUD-006 — No serial console
| | |
|--|--|
| **Severity** | High |
| **Evidence** | Activity log is app events only, not TX/RX |
| **Impact** | Operators need external terminal for debugging |
| **Improvement** | Console panel wired to `LineReceived` (Phase B) |

### AUD-007 — No machine control surface
| | |
|--|--|
| **Severity** | High |
| **Evidence** | No home/jog/preheat UI |
| **Impact** | Ender 3 v2 manual workflows unsupported in-app |
| **Improvement** | Control panel tiles (Phase B) |

### AUD-008 — Fan speed absent
| | |
|--|--|
| **Severity** | High |
| **Evidence** | Mockup shows Fan %; backend has no fan data |
| **Impact** | Incomplete status strip vs design |
| **Improvement** | Parse fan; show `—` until Phase C |

### AUD-009 — Position/layer not surfaced
| | |
|--|--|
| **Severity** | High |
| **Evidence** | `M114` parsed internally, not on `PrinterStatus` |
| **Impact** | Live Monitor missing X/Y/Z/E, layer |
| **Improvement** | Extend `PrinterStatus` + UI (Phase B/C) |

### AUD-010 — Single ViewModel monolith
| | |
|--|--|
| **Severity** | High |
| **Evidence** | All logic in `MainViewModel.cs` (~400 lines) |
| **Impact** | Dashboard refactor will become unmaintainable |
| **Improvement** | Split: `ShellViewModel`, `QueueViewModel`, `MonitorViewModel`, `ConsoleViewModel` |

---

## Medium

### AUD-011 — Weak visual hierarchy in header
| | |
|--|--|
| **Severity** | Medium |
| **Evidence** | App title same weight as connection chip |
| **Improvement** | Status hero dominates; title moves to nav rail |

### AUD-012 — Settings orphaned from connection
| | |
|--|--|
| **Severity** | Medium |
| **Evidence** | Port/baud in connection card; save in Settings tab |
| **Improvement** | Unified printer profile in nav card + Settings |

### AUD-013 — Activity log undersized
| | |
|--|--|
| **Severity** | Medium |
| **Evidence** | MaxHeight 80px, mono not styled as console |
| **Improvement** | Dedicated console panel 160–240px |

### AUD-014 — DataGrid feels legacy
| | |
|--|--|
| **Severity** | Medium |
| **Evidence** | Standard WPF DataGrid with grid lines aesthetic |
| **Improvement** | `JobRow` custom list with rounded selection |

### AUD-015 — No footer status line
| | |
|--|--|
| **Severity** | Medium |
| **Evidence** | Mockup bottom bar missing |
| **Improvement** | `SHELL-03` footer with line progress |

### AUD-016 — Tab navigation hides queue during Settings
| | |
|--|--|
| **Severity** | Medium |
| **Evidence** | Switching tabs loses print visibility |
| **Improvement** | Status strip persistent; Settings is nav section only |

### AUD-017 — Estimated duration / filament not shown
| | |
|--|--|
| **Severity** | Medium |
| **Evidence** | `GCodeParser` has duration estimate; not in grid |
| **Improvement** | Add columns when data available |

### AUD-018 — No queue reorder
| | |
|--|--|
| **Severity** | Medium |
| **Evidence** | `QueueOrder` in model; no UI |
| **Improvement** | Drag reorder (Phase C) |

### AUD-019 — ComboBox dropdown still system-styled
| | |
|--|--|
| **Severity** | Medium |
| **Evidence** | Dark theme on field; popup may flash light |
| **Improvement** | Full ComboBox `ControlTemplate` in theme |

### AUD-020 — E-stop zone improved but not mockup-grade
| | |
|--|--|
| **Severity** | Medium |
| **Evidence** | Danger zone exists; mockup uses Active Job panel separation |
| **Improvement** | E-stop in Active Job + strip shortcut; keep confirmation |

### AUD-021 — Inconsistent panel titles
| | |
|--|--|
| **Severity** | Medium |
| **Evidence** | "Print Queue" vs "Live Monitor" vs "Active Print" |
| **Improvement** | Title system: uppercase labels + sentence titles |

---

## Low

### AUD-022 — No app version in UI
| | |
|--|--|
| **Severity** | Low |
| **Improvement** | Nav rail footer `v1.0.0` |

### AUD-023 — No light mode toggle
| | |
|--|--|
| **Severity** | Low |
| **Improvement** | Phase D; mockup shows toggle |

### AUD-024 — Emoji removed but icons minimal
| | |
|--|--|
| **Severity** | Low |
| **Improvement** | Consistent Path icon set per nav item |

### AUD-025 — Window title only "Ender Assist"
| | |
|--|--|
| **Severity** | Low |
| **Improvement** | Dynamic: "Ender Assist — PRINTING (42%)" |

### AUD-026 — Segment tab host underused
| | |
|--|--|
| **Severity** | Low |
| **Evidence** | `SegmentTabHost` style wraps TabControl awkwardly |
| **Improvement** | Replace with nav rail |

### AUD-027 — 50/50 mental model leftover
| | |
|--|--|
| **Severity** | Low |
| **Evidence** | 3:2 split good; full grid not implemented |
| **Improvement** | Dashboard grid per layout rules |

---

## Positive (preserve)

| Item | Notes |
|------|-------|
| QA connection states | `CanConnect`, single `ConnectionState` source |
| E-stop confirmation | Keep in redesign |
| Temp `—` until live | Keep in status strip |
| Thermal warning banner | Move to shell level |
| Activity log | Evolve into console |
| Touch target sizes | Maintain ≥44px |
| F12 E-stop | Keep |
| Selected-job Start | Keep |

---

## Four-Agent Sign-Off

| Agent | Verdict |
|-------|---------|
| Product Designer | ✅ Audit supports IA restructure |
| Visual Designer | ✅ Issues map to design system upgrade |
| UX Engineer | ✅ Interaction gaps documented |
| Desktop UI Engineer | ✅ AUD-010 flags MVVM split before Phase A code |

**Recommendation:** Proceed to **Phase A implementation** per `SCREEN_INVENTORY.md` roadmap.
