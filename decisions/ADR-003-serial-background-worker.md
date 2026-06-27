# Context

Serial I/O is blocking at the OS level. Marlin communication requires continuous read loop for `ok`/`busy` responses. UI must never block on serial operations.

# Decision

Run all `SerialPort` access on a **dedicated background worker** implemented as a `BackgroundService` with an internal command queue. Public API is async (`SendCommandAsync`, `ConnectAsync`). UI and queue layers submit work through this service only.

# Alternatives Considered

| Alternative | Rejected Because |
|-------------|------------------|
| UI thread polling | Freezes UI; unacceptable |
| Timer-based polling on thread pool | Race conditions on SerialPort; not thread-safe |
| async read loop on thread pool | SerialPort not fully async-safe across platforms |
| Third-party async serial library | User specified System.IO.Ports; avoid dependency unless blocked |

# Consequences

- **Positive:** Single writer to SerialPort eliminates race conditions
- **Positive:** Predictable command serialization for Marlin flow control
- **Negative:** Command queue adds latency (~ms); acceptable for 3D printing
- **Negative:** Must implement graceful shutdown and cancellation
