# Agent Assignments

Logical agent roles and their task ownership. Agents communicate only through repository files (`/tasks`, `/issues`, `/progress`, `/docs`).

## Role Definitions

| Role | Responsibility |
|------|----------------|
| **ProjectManager** | Scope, milestones, progress boards, task lifecycle |
| **Architect** | Solution structure, core abstractions, DI, cross-cutting design |
| **SerialEngineer** | USB serial, Marlin protocol, flow control, reconnect |
| **QueueEngineer** | G-code parsing, print queue, state machine, scheduling |
| **MonitoringEngineer** | Temperature, position, status aggregation |
| **SafetyEngineer** | Emergency stop, pause/resume, thermal safety hooks |
| **UIEngineer** | WPF views, ViewModels, UX |
| **DatabaseEngineer** | SQLite schema, migrations |
| **PersistenceEngineer** | Repositories, settings, data access |
| **TestEngineer** | Test infrastructure, unit/integration/E2E tests |
| **DocumentationEngineer** | User and developer guides |

## Task Ownership Matrix

| Task ID | Title | Owner | Priority |
|---------|-------|-------|----------|
| TASK-001 | Solution scaffold and project structure | Architect | CRITICAL |
| TASK-002 | Core domain models and interfaces | Architect | CRITICAL |
| TASK-003 | Dependency injection bootstrap | Architect | CRITICAL |
| TASK-004 | Serial port discovery | SerialEngineer | HIGH |
| TASK-005 | Serial connection lifecycle | SerialEngineer | HIGH |
| TASK-006 | Marlin handshake and firmware info | SerialEngineer | HIGH |
| TASK-007 | OK/busy synchronization | SerialEngineer | CRITICAL |
| TASK-008 | G-code line sender with flow control | SerialEngineer | CRITICAL |
| TASK-009 | Serial reconnection and error recovery | SerialEngineer | HIGH |
| TASK-010 | SQLite schema design | DatabaseEngineer | HIGH |
| TASK-011 | Database migration infrastructure | DatabaseEngineer | HIGH |
| TASK-012 | Print job repository | PersistenceEngineer | HIGH |
| TASK-013 | Printer profile repository | PersistenceEngineer | MEDIUM |
| TASK-014 | Application settings persistence | PersistenceEngineer | MEDIUM |
| TASK-015 | G-code file parser | QueueEngineer | HIGH |
| TASK-016 | Print queue model | QueueEngineer | HIGH |
| TASK-017 | Queue state machine | QueueEngineer | CRITICAL |
| TASK-018 | Job scheduling and prioritization | QueueEngineer | HIGH |
| TASK-019 | Temperature polling service | MonitoringEngineer | HIGH |
| TASK-020 | Position and progress tracking | MonitoringEngineer | MEDIUM |
| TASK-021 | Printer status aggregation | MonitoringEngineer | HIGH |
| TASK-022 | Emergency stop handler | SafetyEngineer | CRITICAL |
| TASK-023 | Thermal runaway detection hooks | SafetyEngineer | MEDIUM |
| TASK-024 | Safe pause/resume logic | SafetyEngineer | HIGH |
| TASK-025 | MVVM infrastructure | UIEngineer | HIGH |
| TASK-026 | Main window shell | UIEngineer | HIGH |
| TASK-027 | Connection panel UI | UIEngineer | HIGH |
| TASK-028 | Print queue UI | UIEngineer | HIGH |
| TASK-029 | Monitoring dashboard UI | UIEngineer | HIGH |
| TASK-030 | Settings and printer profile UI | UIEngineer | MEDIUM |
| TASK-031 | Unit test project setup | TestEngineer | HIGH |
| TASK-032 | Serial layer integration tests | TestEngineer | HIGH |
| TASK-033 | End-to-end print simulation tests | TestEngineer | HIGH |
| TASK-034 | User guide | DocumentationEngineer | MEDIUM |
| TASK-035 | Developer setup guide | DocumentationEngineer | MEDIUM |

## Workload by Role

| Role | Task Count | Est. Hours |
|------|------------|------------|
| Architect | 3 | 12–18 |
| SerialEngineer | 6 | 28–40 |
| DatabaseEngineer | 2 | 8–12 |
| PersistenceEngineer | 3 | 12–18 |
| QueueEngineer | 4 | 20–28 |
| MonitoringEngineer | 3 | 12–18 |
| SafetyEngineer | 3 | 12–16 |
| UIEngineer | 6 | 24–36 |
| TestEngineer | 3 | 16–24 |
| DocumentationEngineer | 2 | 6–10 |
| ProjectManager | ongoing | — |

## Assignment Rules

1. Each task has exactly one owner (see task file `# Owner` field).
2. Agents must not modify another task's ownership or status without ProjectManager coordination.
3. Cross-layer integration issues: owner of the consuming layer files the issue; owner of the providing layer resolves.
4. When blocked, owner sets task to BLOCKED and creates an issue in `/issues`.

## Current Phase

**Phase:** Project initialization complete → Implementation gate open  
**Next action:** SerialEngineer/Architect begins TASK-001 (Solution scaffold)  
**ProjectManager:** Maintains `/progress` boards

## Escalation Path

```text
Engineer discovers defect
    → Create ISSUE-NNN in /issues
    → Set task BLOCKED if needed
    → ProjectManager triages severity
    → Assign resolution to appropriate owner
```
