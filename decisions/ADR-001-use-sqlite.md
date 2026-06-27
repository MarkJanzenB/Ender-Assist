# Context

The application must persist print jobs, queue state, printer profiles, and user settings across restarts. v1 is a single-user Windows desktop app with no server component.

# Decision

Use **SQLite** via `Microsoft.Data.Sqlite` as the sole persistence store. Database file lives at `%AppData%/MarlinPrintMiddleware/data.db`.

# Alternatives Considered

| Alternative | Rejected Because |
|-------------|------------------|
| JSON files | No transactional integrity; poor query support for queue ordering |
| LiteDB | Additional dependency; team specified SQLite |
| SQL Server LocalDB | Heavier install footprint; overkill for local queue |
| Entity Framework Core | ORM overhead; raw SQL + migrations sufficient for v1 |

# Consequences

- **Positive:** Zero-config, single-file database, ACID transactions, easy backup
- **Positive:** Aligns with user tech stack requirement
- **Negative:** Manual migration scripts required (TASK-011)
- **Negative:** No built-in schema versioning — must implement version table
