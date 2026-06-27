# Task ID
TASK-025

# Title
MVVM infrastructure

# Owner
UIEngineer

# Status
TODO

# Priority
HIGH

# Complexity
MEDIUM

# Dependencies
TASK-003

# Description
Set up UI project with CommunityToolkit.Mvvm. Create base ViewModel, navigation service stub, value converters (bool/visibility, progress), and resource dictionaries for shared styles.

# Acceptance Criteria
- [ ] CommunityToolkit.Mvvm referenced in UI project
- [ ] BaseViewModel with error handling helper
- [ ] Common converters registered in App resources
- [ ] Light theme resource dictionary
- [ ] ViewModelLocator or DI-based ViewModel resolution

# Test Requirements
- Unit test: converter bool-to-visibility
- Unit test: BaseViewModel error property notification

# Related Issues
None

# Progress Notes
2026-06-27: Task created during project initialization.
