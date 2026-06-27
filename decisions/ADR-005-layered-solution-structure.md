# Context

Multiple specialized agents will implement subsystems in parallel. Clear boundaries needed to prevent circular dependencies and enable independent testing.

# Decision

Organize code into **layered class library projects** under `/src` as documented in FOLDER_STRUCTURE.md. Dependency direction: App → UI → feature layers → Core. Core has zero outward dependencies.

# Alternatives Considered

| Alternative | Rejected Because |
|-------------|------------------|
| Single monolithic project | Parallel agent work causes merge conflicts |
| Vertical slices per feature | Shared serial/queue coupling harder to isolate |
| Microservices | Absurd for local desktop middleware |

# Consequences

- **Positive:** Enforces dependency inversion via Core interfaces
- **Positive:** Each agent owns a project boundary
- **Negative:** More projects to manage (mitigated by TASK-001 scaffold)
- **Negative:** Cross-project refactoring requires coordination
