# Ender Assist — Accessibility

**Target:** WCAG 2.2 Level AA minimum; AAA for safety-critical text.

---

## 1. Visual

| Requirement | Implementation |
|-------------|----------------|
| Contrast (body text) | ≥ 7:1 on card surfaces (`#F1F5F9` on `#141A22`) |
| Contrast (large text ≥18px) | ≥ 4.5:1 minimum |
| Color not sole indicator | Connection uses dot **+** label; heating uses text badge **+** color |
| Focus visible | 2px `accent` focus ring on all interactive controls |
| Touch targets | ≥ 44×44 DIP (workshop gloves) |

---

## 2. Keyboard

| Key | Action |
|-----|--------|
| Tab / Shift+Tab | Move focus through controls |
| Enter / Space | Activate button |
| F12 | Emergency stop (with confirmation) |
| Escape | Close dialog / dismiss banner |
| Ctrl+1–6 | Nav sections (planned) |

**Tab order:** Nav → Status strip → Main content (L→R, T→B) → Footer.

---

## 3. Screen Readers

All interactive elements require `AutomationProperties.Name`:

- `"Connect to printer"`, `"Emergency stop — sends M112"`, `"Nozzle temperature"`, etc.  
- Live regions: `StatusHero` uses `LiveSetting="Polite"` for state changes.  
- Progress: `AutomationProperties.Value` bound to print %.

---

## 4. Motion & Sensory

- Respect `SystemParameters.ClientAreaAnimation` — reduce motion if disabled.  
- No flashing > 3 Hz.  
- E-stop confirmation uses icon + text, not sound alone.

---

## 5. DPI & Vision

- Support 100–200% scaling without clipping.  
- Temperature metrics use `type.metric-xl` — readable at 3m with 24" monitor.  
- No critical info below 11px.

---

## 6. Cognitive

- One primary action per panel (STYLE_GUIDE).  
- Confirm destructive actions in plain language.  
- Consistent `—` for missing data (never misleading `0.0°C`).

---

## 7. Audit Checklist (pre-release)

- [ ] Keyboard-only connect → start print flow  
- [ ] Narrator reads connection state change  
- [ ] Focus visible on all buttons at 150% DPI  
- [ ] E-stop reachable via keyboard  
- [ ] Color-blind simulation: status distinguishable without color  

---

## Four-Agent Sign-Off

| Agent | Verdict |
|-------|---------|
| Product Designer | ✅ Workshop operators include low-vision users |
| Visual Designer | ✅ AAA palette documented in DESIGN_SYSTEM |
| UX Engineer | ✅ Keyboard + confirmation paths defined |
| Desktop UI Engineer | ✅ WPF `AutomationProperties` plan ready |
