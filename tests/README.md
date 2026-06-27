# Tests

Run all unit and integration tests from the repository root:

```bash
dotnet test
```

To run a single test project:

```bash
dotnet test tests/MarlinPrintMiddleware.Core.Tests
dotnet test tests/MarlinPrintMiddleware.Serial.Tests
dotnet test tests/MarlinPrintMiddleware.Queue.Tests
dotnet test tests/MarlinPrintMiddleware.Persistence.Tests
dotnet test tests/MarlinPrintMiddleware.Integration.Tests
```
