# Task ID
TASK-027

# Title
Connection panel UI

# Owner
UIEngineer

# Status
DONE

# Priority
HIGH

# Complexity
MEDIUM

# Dependencies
TASK-005, TASK-025

# Description
Build ConnectionPanel view: COM port dropdown (from discovery), baud rate selector, Connect/Disconnect buttons, firmware info display, connection status indicator.

# Acceptance Criteria
- [ ] Port list populates from ISerialPortDiscovery
- [ ] Refresh ports button works
- [ ] Connect/Disconnect commands bound to serial service
- [ ] Firmware info shown after successful handshake
- [ ] Disabled controls when connecting/disconnecting
- [ ] Error messages displayed to user

# Test Requirements
- ViewModel unit test: port list binding
- ViewModel unit test: connect command disabled when no port selected
- ViewModel unit test: firmware info populated after connect

# Related Issues
None

# Progress Notes
2026-06-27: Task created during project initialization.
2026-06-27: Connection panel in MainWindow.
