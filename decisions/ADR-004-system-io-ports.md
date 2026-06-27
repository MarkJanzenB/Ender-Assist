# Context

Communication with Marlin printers on Ender 3 V2 uses USB virtual COM ports. User explicitly specified the .NET SerialPort API.

# Decision

Use **`System.IO.Ports.SerialPort`** from the .NET 8 runtime for all serial communication. No third-party serial libraries in v1.

# Alternatives Considered

| Alternative | Rejected Because |
|-------------|------------------|
| RJCP.SerialPortStream | Extra dependency; user requirement |
| HidSharp | USB HID path; Ender 3 uses CDC ACM serial |
| Win32 P/Invoke directly | Unmaintainable; SerialPort sufficient |

# Consequences

- **Positive:** Zero additional dependencies; well-documented on Windows
- **Positive:** Matches user tech stack
- **Negative:** Limited async support — mitigated by ADR-003 background worker
- **Negative:** Port enumeration may need WMI supplement for friendly names (TASK-004)
