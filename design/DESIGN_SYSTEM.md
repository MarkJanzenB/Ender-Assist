# Ender Assist — Design System

**Version:** 1.0  
**Companion:** `STYLE_GUIDE.md`, `COMPONENT_LIBRARY.md`

---

## 1. Color Tokens

### Surfaces

| Token | Hex | Usage |
|-------|-----|-------|
| `color.shell` | `#0A0D12` | Window background |
| `color.shell-elevated` | `#0F1319` | Nav rail, status bar |
| `color.card` | `#141A22` | Primary panels |
| `color.card-hover` | `#1A222C` | Row hover, card highlight |
| `color.inset` | `#0D1117` | Console, empty states |
| `color.overlay` | `#1E2632` | Inputs, combo boxes |

### Borders

| Token | Hex | Usage |
|-------|-----|-------|
| `color.border.subtle` | `#252D3A` | Card borders |
| `color.border.focus` | `#2DD4BF` | Focus ring |
| `color.border.divider` | `#1E2632` | Section separators |

### Text

| Token | Hex | Contrast on `card` |
|-------|-----|-------------------|
| `color.text.primary` | `#F1F5F9` | ~15:1 |
| `color.text.secondary` | `#94A3B8` | ~7.5:1 |
| `color.text.tertiary` | `#64748B` | ~4.8:1 (labels only) |

### Accent & Semantic

| Token | Hex | Usage |
|-------|-----|-------|
| `color.accent` | `#2DD4BF` | Primary CTA, progress fill |
| `color.accent-muted` | `#134E4A` | Selected row, chip bg |
| `color.ready` | `#34D399` | Connected, at temp |
| `color.ready-muted` | `#064E3B` | Ready pill background |
| `color.heating` | `#FBBF24` | Heating, warnings |
| `color.heating-muted` | `#78350F` | Warning banner bg |
| `color.danger` | `#F87171` | E-stop, errors |
| `color.danger-muted` | `#450A0A` | E-stop zone bg |

### Gradients (restrained)

| Token | Definition | Usage |
|-------|------------|-------|
| `gradient.shell` | `#0A0D12` → `#0F141C` | Root grid only |
| `gradient.accent` | `#2DD4BF` → `#14B8A6` | Primary button fill |
| `gradient.danger` | `#F87171` → `#DC2626` | E-stop button |

---

## 2. Typography Scale

| Token | Size | Weight | Line height | Use |
|-------|------|--------|-------------|-----|
| `type.metric-xl` | 40px | Light | 1.1 | Nozzle/bed live temp |
| `type.metric-lg` | 28px | Light | 1.2 | Progress %, ETA |
| `type.title` | 20px | SemiBold | 1.3 | Panel titles |
| `type.heading` | 17px | SemiBold | 1.35 | Section headers |
| `type.body` | 14px | Regular | 1.5 | Body copy |
| `type.body-strong` | 14px | SemiBold | 1.5 | Status labels |
| `type.caption` | 12px | Regular | 1.4 | Hints, footer |
| `type.label` | 11px | SemiBold | 1.2 | Uppercase field labels |
| `type.mono` | 12px | Regular | 1.5 | Console (Cascadia Mono) |

---

## 3. Radius

| Token | Value |
|-------|-------|
| `radius.sm` | 6px |
| `radius.md` | 10px |
| `radius.lg` | 14px |
| `radius.pill` | 999px |

---

## 4. Shadows

| Token | WPF `DropShadowEffect` |
|-------|------------------------|
| `shadow.card` | Blur 20, Depth 2, Opacity 0.2, Color `#000000` |
| `shadow.elevated` | Blur 32, Depth 4, Opacity 0.3 |
| `shadow.danger-glow` | Blur 16, Depth 0, Opacity 0.35, Color `#EF4444` (E-stop only) |

---

## 5. Motion Tokens

| Token | Duration | Easing |
|-------|----------|--------|
| `motion.fast` | 120ms | `CubicEase` EaseOut |
| `motion.normal` | 200ms | `CubicEase` EaseOut |
| `motion.slow` | 320ms | `CubicEase` EaseInOut |

Apply to: nav selection, button press, progress bar value changes (not serial polling).

---

## 6. Z-Index / Layer Order

1. Shell background  
2. Nav rail + main content  
3. Cards  
4. Dropdowns / popups  
5. Thermal warning banner  
6. Modal dialogs (confirm E-stop, etc.)  

---

## 7. WPF Resource Mapping

Planned `EnderAssistTheme.xaml` keys (implementation Phase 10):

```
ShellBrush, CardBrush, SurfaceBrush, AccentBrush, ...
Font: DisplayFont, BodyFont, MonoFont
Styles: NavItem, StatusChip, PrimaryButton, GhostButton, DangerButton,
        MetricCard, ConsoleLog, DataGridModern, ...
```

---

## 8. Dark Mode

**v1:** Dark only (workshop default).  
**v2:** Light theme via swapped resource dictionary (`EnderAssistTheme.Light.xaml`) — not in initial implementation.

---

## Four-Agent Sign-Off

| Agent | Verdict |
|-------|---------|
| Product Designer | ✅ Tokens support status-at-a-glance |
| Visual Designer | ✅ Cohesive premium dark system |
| UX Engineer | ✅ Semantic tokens cover all states |
| Desktop UI Engineer | ✅ Maps 1:1 to WPF `SolidColorBrush` / styles |
