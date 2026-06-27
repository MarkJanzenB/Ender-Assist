# Context

The UI is WPF on .NET 8. Business logic must be testable without launching the visual tree. Multiple agents (UIEngineer, SerialEngineer) will work in parallel.

# Decision

Use **MVVM** with `CommunityToolkit.Mvvm` for all UI. Views are XAML-only; ViewModels consume application services via DI. No business logic in code-behind.

# Alternatives Considered

| Alternative | Rejected Because |
|-------------|------------------|
| Code-behind | Untestable; tight coupling to views |
| MVC (Razor) | Not applicable to WPF desktop |
| MVU (Elmish) | Unfamiliar to team; smaller WPF ecosystem |
| ReactiveUI | Heavier learning curve; CommunityToolkit sufficient |

# Consequences

- **Positive:** ViewModels unit-testable with mocked services
- **Positive:** Clear separation enables parallel UI/serial development
- **Negative:** More boilerplate (mitigated by source generators)
- **Negative:** Requires disciplined command binding patterns
