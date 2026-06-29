using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarlinPrintMiddleware.Core.Enums;
using MarlinPrintMiddleware.Core.Interfaces;
using MarlinPrintMiddleware.Core.Models;
using MarlinPrintMiddleware.UI.Enums;
using MarlinPrintMiddleware.UI.Models;
using MarlinPrintMiddleware.UI.Themes;
using Microsoft.Win32;

namespace MarlinPrintMiddleware.UI.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private const string AutoStartSettingKey = "queue.auto_start";
    private const string DarkThemeSettingKey = "ui.dark_theme";
    private const double JogStepMm = 10;
    private const int ActivityLogMax = 40;
    private const double HeatingToleranceC = 2.0;

    private readonly ISerialPortDiscovery _portDiscovery;
    private readonly ISerialEngine _serialEngine;
    private readonly IPrintQueueService _queueService;
    private readonly IPrinterStatusService _statusService;
    private readonly IEmergencyStopService _emergencyStopService;
    private readonly IPauseResumeService _pauseResumeService;
    private readonly IPrinterProfileRepository _profileRepository;
    private readonly ISettingsRepository _settingsRepository;
    private readonly IThermalWarningService _thermalWarningService;
    private readonly ISerialConsoleService _consoleService;
    private readonly IPrinterControlService _printerControlService;
    private readonly IMacroRepository _macroRepository;

    private bool _savedAutoStart;
    private bool _applyThemeFromUi;
    private PrintState _printState = PrintState.Idle;

    public MainViewModel(
        ISerialPortDiscovery portDiscovery,
        ISerialEngine serialEngine,
        IPrintQueueService queueService,
        IPrinterStatusService statusService,
        IEmergencyStopService emergencyStopService,
        IPauseResumeService pauseResumeService,
        IPrinterProfileRepository profileRepository,
        ISettingsRepository settingsRepository,
        IThermalWarningService thermalWarningService,
        ISerialConsoleService consoleService,
        IPrinterControlService printerControlService,
        IMacroRepository macroRepository)
    {
        _portDiscovery = portDiscovery;
        _serialEngine = serialEngine;
        _queueService = queueService;
        _statusService = statusService;
        _emergencyStopService = emergencyStopService;
        _pauseResumeService = pauseResumeService;
        _profileRepository = profileRepository;
        _settingsRepository = settingsRepository;
        _thermalWarningService = thermalWarningService;
        _consoleService = consoleService;
        _printerControlService = printerControlService;
        _macroRepository = macroRepository;

        Jobs = new ObservableCollection<PrintJob>();
        HistoryJobs = new ObservableCollection<PrintJob>();
        Ports = new ObservableCollection<SerialPortInfo>();
        ActivityLog = new ObservableCollection<ActivityLogEntry>();
        ConsoleLines = new ObservableCollection<string>();
        Macros = new ObservableCollection<PrinterMacro>();
        BaudRates = new ObservableCollection<int> { 115200, 250000, 57600, 9600 };
        NavItems = CreateNavItems();

        _statusService.StatusChanged += (_, status) =>
        {
            if (Application.Current?.Dispatcher is { } dispatcher)
            {
                if (dispatcher.CheckAccess())
                {
                    ApplyStatus(status);
                }
                else
                {
                    dispatcher.BeginInvoke(() => ApplyStatus(status));
                }
            }
        };

        _thermalWarningService.ThermalWarning += (_, args) =>
        {
            if (Application.Current?.Dispatcher is { } dispatcher)
            {
                void Apply() => ShowThermalWarning(args.Message);
                if (dispatcher.CheckAccess())
                {
                    Apply();
                }
                else
                {
                    dispatcher.BeginInvoke(Apply);
                }
            }
        };

        _consoleService.LinesChanged += (_, _) =>
        {
            if (Application.Current?.Dispatcher is { } dispatcher)
            {
                void Apply() => RefreshConsoleLines();
                if (dispatcher.CheckAccess())
                {
                    Apply();
                }
                else
                {
                    dispatcher.BeginInvoke(Apply);
                }
            }
        };

        _ = InitializeSafeAsync();
    }

    public ObservableCollection<PrintJob> Jobs { get; }

    public ObservableCollection<PrintJob> HistoryJobs { get; }

    public ObservableCollection<SerialPortInfo> Ports { get; }

    public ObservableCollection<int> BaudRates { get; }

    public ObservableCollection<ActivityLogEntry> ActivityLog { get; }

    public ObservableCollection<string> ConsoleLines { get; }

    public ObservableCollection<PrinterMacro> Macros { get; }

    public ObservableCollection<NavItemViewModel> NavItems { get; }

    public const string AppVersion = "v1.0.0";

    [ObservableProperty] private NavSection _currentSection = NavSection.Dashboard;

    [ObservableProperty] private string _statusHeroText = "OFFLINE";

    [ObservableProperty] private string _statusHeroSubtext = "Connect to begin";

    [ObservableProperty] private string _fanDisplay = "—";

    [ObservableProperty] private string _fanTargetDisplay = "—";

    [ObservableProperty] private string _boardInfo = "—";

    [ObservableProperty] private string _printerModel = "Ender 3 v2";

    [ObservableProperty] private string _portDisplay = "—";

    [ObservableProperty] private string _footerSummary = "Disconnected";

    [ObservableProperty] private SerialPortInfo? _selectedPort;

    [ObservableProperty] private int _baudRate = 115200;

    [ObservableProperty] private string _firmwareInfo = "—";

    [ObservableProperty] private ConnectionState _connectionState = ConnectionState.Disconnected;

    [ObservableProperty] private string _hotendDisplay = "—";

    [ObservableProperty] private string _hotendTargetDisplay = "—";

    [ObservableProperty] private string _bedDisplay = "—";

    [ObservableProperty] private string _bedTargetDisplay = "—";

    [ObservableProperty] private string _hotendHeatingLabel = string.Empty;

    [ObservableProperty] private string _bedHeatingLabel = string.Empty;

    [ObservableProperty] private bool _isHotendHeating;

    [ObservableProperty] private bool _isBedHeating;

    [ObservableProperty] private bool _isHotendAtTemp;

    [ObservableProperty] private bool _isBedAtTemp;

    [ObservableProperty] private double _progress;

    [ObservableProperty] private string _currentJobName = "—";

    [ObservableProperty] private string _elapsedText = "—";

    [ObservableProperty] private string _etaText = "—";

    [ObservableProperty] private string _printStateText = "Idle";

    [ObservableProperty] private string _statusMessage = "Ready";

    [ObservableProperty] private bool _isConnected;

    [ObservableProperty] private bool _isBusy;

    [ObservableProperty] private bool _autoStartQueue;

    [ObservableProperty] private bool _hasUnsavedSettings;

    [ObservableProperty] private PrintJob? _selectedJob;

    [ObservableProperty] private string? _thermalWarningMessage;

    [ObservableProperty] private bool _hasThermalWarning;

    [ObservableProperty] private string _portsEmptyHint = string.Empty;

    [ObservableProperty] private string _positionDisplay = "X: —  Y: —  Z: —";

    [ObservableProperty] private string _layerDisplay = "—";

    [ObservableProperty] private string _consoleInput = string.Empty;

    [ObservableProperty] private int _consoleFilterIndex;

    [ObservableProperty] private bool _isDarkTheme = true;

    [ObservableProperty] private PrinterMacro? _selectedMacro;

    [ObservableProperty] private string _macroName = string.Empty;

    [ObservableProperty] private string _macroCommand = string.Empty;

    public bool HasPorts => Ports.Count > 0;

    public bool HasCompletedJobs => Jobs.Any(j => j.Status is JobStatus.Completed or JobStatus.Cancelled);

    [RelayCommand(CanExecute = nameof(CanRefreshPorts))]
    private async Task RefreshPortsAsync()
    {
        try
        {
            IsBusy = true;
            await _portDiscovery.RefreshAsync();
            await LoadPortsAsync();
            LogActivity("Ports refreshed.");
        }
        catch (Exception ex)
        {
            LogActivity(ex.Message, isError: true);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool CanRefreshPorts() => !IsBusy;

    [RelayCommand(CanExecute = nameof(CanConnect))]
    private async Task ConnectAsync()
    {
        if (SelectedPort is null)
        {
            LogActivity("Select a COM port before connecting.", isError: true);
            return;
        }

        try
        {
            IsBusy = true;
            LogActivity($"Connecting to {SelectedPort.PortName} at {BaudRate} baud…");
            await _serialEngine.ConnectAsync(SelectedPort.PortName, BaudRate);
            FirmwareInfo = _serialEngine.FirmwareInfo is { } info
                ? $"{info.FirmwareName} {info.Version}"
                : "Marlin";
            LogActivity("Connected to printer.");
        }
        catch (Exception ex)
        {
            LogActivity(ex.Message, isError: true);
        }
        finally
        {
            IsBusy = false;
            ApplyStatus(_statusService.CurrentStatus);
        }
    }

    private bool CanConnect() =>
        !IsBusy && ConnectionState is not (ConnectionState.Connected or ConnectionState.Connecting)
        && SelectedPort is not null;

    [RelayCommand(CanExecute = nameof(CanDisconnect))]
    private async Task DisconnectAsync()
    {
        try
        {
            IsBusy = true;
            await _serialEngine.DisconnectAsync();
            FirmwareInfo = "—";
            LogActivity("Disconnected.");
        }
        catch (Exception ex)
        {
            LogActivity(ex.Message, isError: true);
        }
        finally
        {
            IsBusy = false;
            ApplyStatus(_statusService.CurrentStatus);
        }
    }

    private bool CanDisconnect() => !IsBusy && IsConnected;

    [RelayCommand(CanExecute = nameof(CanAddGCode))]
    private async Task AddGCodeAsync()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "G-code files|*.gcode;*.gco;*.g|All files|*.*"
        };

        if (dialog.ShowDialog() != true)
        {
            return;
        }

        try
        {
            IsBusy = true;
            LogActivity($"Analyzing {System.IO.Path.GetFileName(dialog.FileName)}…");
            await EnqueueFileAsync(dialog.FileName);
        }
        catch (Exception ex)
        {
            LogActivity(ex.Message, isError: true);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool CanAddGCode() => !IsBusy;

    public async Task EnqueueFileAsync(string filePath)
    {
        await _queueService.EnqueueAsync(new PrintJob { FilePath = filePath });
        RefreshJobs();
        LogActivity($"Added {System.IO.Path.GetFileName(filePath)} to queue.");
    }

    public async Task ReorderJobAsync(long jobId, int newIndex)
    {
        await _queueService.ReorderJobAsync(jobId, newIndex);
        RefreshJobs();
    }

    [RelayCommand(CanExecute = nameof(CanStartPrint))]
    private async Task StartPrintAsync()
    {
        if (SelectedJob is null)
        {
            LogActivity("Select a pending job to start.", isError: true);
            return;
        }

        try
        {
            await _queueService.StartJobAsync(SelectedJob.Id);
            RefreshJobs();
            LogActivity($"Started {SelectedJob.Name}.");
        }
        catch (Exception ex)
        {
            LogActivity(ex.Message, isError: true);
        }
    }

    private bool CanStartPrint() =>
        IsConnected
        && SelectedJob?.Status == JobStatus.Pending
        && _printState is PrintState.Idle or PrintState.Completed or PrintState.Cancelled or PrintState.Failed;

    [RelayCommand(CanExecute = nameof(CanPausePrint))]
    private async Task PausePrintAsync()
    {
        try
        {
            await _pauseResumeService.PauseAsync();
            LogActivity("Print paused.");
        }
        catch (Exception ex)
        {
            LogActivity(ex.Message, isError: true);
        }
    }

    private bool CanPausePrint() => _printState == PrintState.Printing;

    [RelayCommand(CanExecute = nameof(CanResumePrint))]
    private async Task ResumePrintAsync()
    {
        try
        {
            await _pauseResumeService.ResumeAsync();
            LogActivity("Print resumed.");
        }
        catch (Exception ex)
        {
            LogActivity(ex.Message, isError: true);
        }
    }

    private bool CanResumePrint() => _printState == PrintState.Paused && IsConnected;

    [RelayCommand(CanExecute = nameof(CanCancelPrint))]
    private async Task CancelPrintAsync()
    {
        var result = MessageBox.Show(
            "Stop the active print job?\n\nThe printer will finish its current move before stopping.",
            "Confirm Cancel",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question,
            MessageBoxResult.No);

        if (result != MessageBoxResult.Yes)
        {
            return;
        }

        try
        {
            await _queueService.CancelAsync();
            RefreshJobs();
            LogActivity("Print cancelled.");
        }
        catch (Exception ex)
        {
            LogActivity(ex.Message, isError: true);
        }
    }

    private bool CanCancelPrint() =>
        _printState is PrintState.Printing or PrintState.Paused or PrintState.Preparing;

    [RelayCommand(CanExecute = nameof(CanEmergencyStop))]
    private async Task EmergencyStopAsync()
    {
        var result = MessageBox.Show(
            "EMERGENCY STOP sends M112 immediately.\n\nOn Marlin firmware this halts all motion and heaters and requires a full printer reset (power cycle or firmware restart) before you can print again.\n\nAre you sure?",
            "Confirm Emergency Stop",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning,
            MessageBoxResult.No);

        if (result != MessageBoxResult.Yes)
        {
            return;
        }

        try
        {
            await _emergencyStopService.TriggerAsync();
            RefreshJobs();
            LogActivity("EMERGENCY STOP (M112) sent.", isError: true);
        }
        catch (Exception ex)
        {
            LogActivity(ex.Message, isError: true);
        }
    }

    private bool CanEmergencyStop() => IsConnected;

    [RelayCommand(CanExecute = nameof(CanRemoveJob))]
    private async Task RemoveJobAsync()
    {
        if (SelectedJob is null)
        {
            return;
        }

        try
        {
            var name = SelectedJob.Name;
            await _queueService.RemoveJobAsync(SelectedJob.Id);
            RefreshJobs();
            LogActivity($"Removed {name} from queue.");
            SelectedJob = Jobs.FirstOrDefault();
        }
        catch (Exception ex)
        {
            LogActivity(ex.Message, isError: true);
        }
    }

    private bool CanRemoveJob() => SelectedJob?.Status == JobStatus.Pending && !IsBusy;

    [RelayCommand(CanExecute = nameof(CanRetryJob))]
    private async Task RetryJobAsync()
    {
        if (SelectedJob is null)
        {
            return;
        }

        try
        {
            await _queueService.RetryFailedJobAsync(SelectedJob.Id);
            RefreshJobs();
            LogActivity($"Queued {SelectedJob.Name} for retry.");
        }
        catch (Exception ex)
        {
            LogActivity(ex.Message, isError: true);
        }
    }

    private bool CanRetryJob() => SelectedJob?.Status == JobStatus.Failed && !IsBusy;

    [RelayCommand(CanExecute = nameof(CanClearCompleted))]
    private async Task ClearCompletedAsync()
    {
        try
        {
            await _queueService.ClearCompletedAsync();
            RefreshJobs();
            LogActivity("Cleared finished jobs from queue.");
        }
        catch (Exception ex)
        {
            LogActivity(ex.Message, isError: true);
        }
    }

    private bool CanClearCompleted() => HasCompletedJobs && !IsBusy;

    [RelayCommand(CanExecute = nameof(CanSaveSettings))]
    private async Task SaveSettingsAsync()
    {
        try
        {
            var profile = await _profileRepository.GetDefaultAsync();
            if (profile is null)
            {
                profile = new PrinterProfile
                {
                    Name = "Ender 3 v2",
                    IsDefault = true
                };
                profile = await _profileRepository.CreateAsync(profile);
            }

            profile.Port = SelectedPort?.PortName ?? string.Empty;
            profile.BaudRate = BaudRate;
            await _profileRepository.UpdateAsync(profile);
            await _settingsRepository.SetAsync(AutoStartSettingKey, AutoStartQueue);

            _savedAutoStart = AutoStartQueue;
            HasUnsavedSettings = false;
            LogActivity("Settings saved.");
        }
        catch (Exception ex)
        {
            LogActivity(ex.Message, isError: true);
        }
    }

    private bool CanSaveSettings() => HasUnsavedSettings;

    [RelayCommand]
    private void DismissThermalWarning()
    {
        HasThermalWarning = false;
        ThermalWarningMessage = null;
    }

    [RelayCommand]
    private void Navigate(NavSection section)
    {
        CurrentSection = section;
        foreach (var item in NavItems)
        {
            item.IsSelected = item.Section == section;
        }

        if (section == NavSection.Macros)
        {
            _ = LoadMacrosAsync();
        }
    }

    [RelayCommand(CanExecute = nameof(CanSendConsole))]
    private async Task SendConsoleAsync()
    {
        var command = ConsoleInput.Trim();
        if (string.IsNullOrEmpty(command))
        {
            return;
        }

        try
        {
            await _consoleService.SendAsync(command);
            ConsoleInput = string.Empty;
        }
        catch (Exception ex)
        {
            LogActivity(ex.Message, isError: true);
        }
    }

    private bool CanSendConsole() => !string.IsNullOrWhiteSpace(ConsoleInput);

    [RelayCommand]
    private void ClearConsole()
    {
        _consoleService.Clear();
        ConsoleLines.Clear();
    }

    [RelayCommand(CanExecute = nameof(CanControlPrinter))]
    private async Task HomeAllAsync() => await RunControlAsync("Homing…", () => _printerControlService.HomeAllAsync());

    [RelayCommand(CanExecute = nameof(CanControlPrinter))]
    private async Task DisableMotorsAsync() => await RunControlAsync("Motors disabled.", () => _printerControlService.DisableMotorsAsync());

    [RelayCommand(CanExecute = nameof(CanControlPrinter))]
    private async Task PreheatPlaAsync() => await RunControlAsync("PLA preheat started.", () => _printerControlService.PreheatPlaAsync());

    [RelayCommand(CanExecute = nameof(CanControlPrinter))]
    private async Task PreheatPetgAsync() => await RunControlAsync("PETG preheat started.", () => _printerControlService.PreheatPetgAsync());

    [RelayCommand(CanExecute = nameof(CanControlPrinter))]
    private async Task CooldownAsync() => await RunControlAsync("Cooldown started.", () => _printerControlService.CooldownAsync());

    [RelayCommand(CanExecute = nameof(CanControlPrinter))]
    private Task JogXMinusAsync() => JogAsync('X', -JogStepMm);

    [RelayCommand(CanExecute = nameof(CanControlPrinter))]
    private Task JogXPlusAsync() => JogAsync('X', JogStepMm);

    [RelayCommand(CanExecute = nameof(CanControlPrinter))]
    private Task JogYMinusAsync() => JogAsync('Y', -JogStepMm);

    [RelayCommand(CanExecute = nameof(CanControlPrinter))]
    private Task JogYPlusAsync() => JogAsync('Y', JogStepMm);

    [RelayCommand(CanExecute = nameof(CanControlPrinter))]
    private Task JogZMinusAsync() => JogAsync('Z', -JogStepMm);

    [RelayCommand(CanExecute = nameof(CanControlPrinter))]
    private Task JogZPlusAsync() => JogAsync('Z', JogStepMm);

    private bool CanControlPrinter() => IsConnected && !IsBusy;

    [RelayCommand]
    private void ToggleTheme() => IsDarkTheme = !IsDarkTheme;

    partial void OnIsDarkThemeChanged(bool value)
    {
        if (!_applyThemeFromUi)
        {
            return;
        }

        ThemeManager.Apply(value);
        _ = _settingsRepository.SetAsync(DarkThemeSettingKey, value);
        LogActivity(value ? "Dark theme enabled." : "Light theme enabled.");
    }

    [RelayCommand]
    private async Task SaveMacroAsync()
    {
        if (string.IsNullOrWhiteSpace(MacroName) || string.IsNullOrWhiteSpace(MacroCommand))
        {
            LogActivity("Macro name and command are required.", isError: true);
            return;
        }

        var macro = SelectedMacro ?? new PrinterMacro();
        macro.Name = MacroName.Trim();
        macro.Command = MacroCommand.Trim();

        if (SelectedMacro is null)
        {
            Macros.Add(macro);
        }

        await _macroRepository.SaveAsync(Macros.ToList());
        SelectedMacro = macro;
        LogActivity($"Saved macro {macro.Name}.");
    }

    [RelayCommand]
    private void NewMacro()
    {
        SelectedMacro = null;
        MacroName = string.Empty;
        MacroCommand = string.Empty;
    }

    [RelayCommand(CanExecute = nameof(CanDeleteMacro))]
    private async Task DeleteMacroAsync()
    {
        if (SelectedMacro is null)
        {
            return;
        }

        var name = SelectedMacro.Name;
        Macros.Remove(SelectedMacro);
        SelectedMacro = Macros.FirstOrDefault();
        await _macroRepository.SaveAsync(Macros.ToList());
        LogActivity($"Deleted macro {name}.");
    }

    private bool CanDeleteMacro() => SelectedMacro is not null;

    [RelayCommand(CanExecute = nameof(CanRunMacro))]
    private async Task RunMacroAsync()
    {
        if (SelectedMacro is null)
        {
            return;
        }

        try
        {
            await _consoleService.SendAsync(SelectedMacro.Command);
            LogActivity($"Ran macro {SelectedMacro.Name}.");
        }
        catch (Exception ex)
        {
            LogActivity(ex.Message, isError: true);
        }
    }

    private bool CanRunMacro() => IsConnected && SelectedMacro is not null;

    [RelayCommand]
    private void GoToSettings() => Navigate(NavSection.Settings);

    partial void OnSelectedPortChanged(SerialPortInfo? value)
    {
        PortDisplay = value?.PortName ?? "—";
        ConnectCommand.NotifyCanExecuteChanged();
        MarkSettingsDirty();
    }

    partial void OnBaudRateChanged(int value) => MarkSettingsDirty();

    partial void OnAutoStartQueueChanged(bool value)
    {
        HasUnsavedSettings = AutoStartQueue != _savedAutoStart;
        SaveSettingsCommand.NotifyCanExecuteChanged();
    }

    partial void OnConsoleInputChanged(string value) => SendConsoleCommand.NotifyCanExecuteChanged();

    partial void OnConsoleFilterIndexChanged(int value)
    {
        _consoleService.Filter = value switch
        {
            1 => ConsoleLogFilter.Rx,
            2 => ConsoleLogFilter.Tx,
            3 => ConsoleLogFilter.Errors,
            _ => ConsoleLogFilter.All
        };
        RefreshConsoleLines();
    }

    partial void OnSelectedMacroChanged(PrinterMacro? value)
    {
        if (value is null)
        {
            return;
        }

        MacroName = value.Name;
        MacroCommand = value.Command;
        RunMacroCommand.NotifyCanExecuteChanged();
        DeleteMacroCommand.NotifyCanExecuteChanged();
    }

    partial void OnSelectedJobChanged(PrintJob? value) => RefreshCommandStates();

    partial void OnIsBusyChanged(bool value) => RefreshCommandStates();

    partial void OnIsConnectedChanged(bool value) => RefreshCommandStates();

    partial void OnConnectionStateChanged(ConnectionState value) => RefreshCommandStates();

    partial void OnCurrentSectionChanged(NavSection value)
    {
        foreach (var item in NavItems)
        {
            item.IsSelected = item.Section == value;
        }
    }

    private static ObservableCollection<NavItemViewModel> CreateNavItems() => new()
    {
        new NavItemViewModel(NavSection.Dashboard, "Dashboard",
            "M3,13H11V3H3M3,21H11V15H3M13,21H21V11H13M13,3V9H21V3"),
        new NavItemViewModel(NavSection.Queue, "Queue",
            "M19,3H5C3.89,3 3,3.89 3,5V19A2,2 0 0,0 5,21H19A2,2 0 0,0 21,19V5C21,3.89 20.1,3 19,3"),
        new NavItemViewModel(NavSection.Terminal, "Terminal",
            "M4,6H20V8H4M4,10H20V12H4M4,14H16V16H4"),
        new NavItemViewModel(NavSection.Macros, "Macros",
            "M12,15.5A3.5,3.5 0 0,1 8.5,12A3.5,3.5 0 0,1 12,8.5A3.5,3.5 0 0,1 15.5,12A3.5,3.5 0 0,1 12,15.5Z"),
        new NavItemViewModel(NavSection.History, "History",
            "M13,3A9,9 0 0,0 4,12H1L4.89,15.89L9,12H6A7,7 0 0,1 13,5A7,7 0 0,1 20,12A7,7 0 0,1 13,19A9,9 0 0,0 22,12A9,9 0 0,0 13,3"),
        new NavItemViewModel(NavSection.Settings, "Settings",
            "M12,8A4,4 0 0,1 16,12A4,4 0 0,1 12,16A4,4 0 0,1 8,12A4,4 0 0,1 12,8M12,2L14.39,5.42C15.71,5.94 16.89,6.75 17.84,7.78L21.7,7.05L21,11.1C21.04,11.4 21.06,11.7 21.06,12C21.06,12.3 21.04,12.6 21,12.9L21.7,16.95L17.84,16.22C16.89,17.25 15.71,18.06 14.39,18.58L12,22L9.61,18.58C8.29,18.06 7.11,17.25 6.16,16.22L2.3,16.95L3,12.9C2.96,12.6 2.94,12.3 2.94,12C2.94,11.7 2.96,11.4 3,11.1L2.3,7.05L6.16,7.78C7.11,6.75 8.29,5.94 9.61,5.42L12,2Z")
    };

    private async Task InitializeSafeAsync()
    {
        try
        {
            await InitializeAsync();
        }
        catch (Exception ex)
        {
            LogActivity($"Startup error: {ex.Message}", isError: true);
        }
    }

    private async Task InitializeAsync()
    {
        var profile = await _profileRepository.GetDefaultAsync();
        if (profile is not null)
        {
            BaudRate = BaudRates.Contains(profile.BaudRate) ? profile.BaudRate : 115200;
        }

        var autoStart = await _settingsRepository.GetAsync<bool>(AutoStartSettingKey);
        AutoStartQueue = autoStart == true;
        _savedAutoStart = AutoStartQueue;
        HasUnsavedSettings = false;

        var darkTheme = await _settingsRepository.GetAsync<bool?>(DarkThemeSettingKey);
        IsDarkTheme = darkTheme ?? true;
        ThemeManager.Apply(IsDarkTheme);
        _applyThemeFromUi = true;

        await LoadPortsAsync();

        if (profile is not null && !string.IsNullOrWhiteSpace(profile.Port))
        {
            SelectedPort = Ports.FirstOrDefault(p =>
                string.Equals(p.PortName, profile.Port, StringComparison.OrdinalIgnoreCase));
        }

        RefreshJobs();
        ApplyStatus(_statusService.CurrentStatus);
        RefreshConsoleLines();
        NavItems.First(n => n.Section == NavSection.Dashboard).IsSelected = true;
        LogActivity("Ender Assist ready.");
    }

    private async Task LoadMacrosAsync()
    {
        var macros = await _macroRepository.GetAllAsync();
        Macros.Clear();
        foreach (var macro in macros)
        {
            Macros.Add(macro);
        }

        SelectedMacro = Macros.FirstOrDefault();
        if (SelectedMacro is not null)
        {
            MacroName = SelectedMacro.Name;
            MacroCommand = SelectedMacro.Command;
        }
    }

    private async Task RunControlAsync(string successMessage, Func<Task> action)
    {
        try
        {
            IsBusy = true;
            await action();
            LogActivity(successMessage);
        }
        catch (Exception ex)
        {
            LogActivity(ex.Message, isError: true);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task JogAsync(char axis, double delta)
    {
        try
        {
            await _printerControlService.JogAsync(axis, delta);
            LogActivity($"Jogged {char.ToUpper(axis)} {delta:+#.##;-#.##;0} mm.");
            await _statusService.RefreshPositionAsync();
        }
        catch (Exception ex)
        {
            LogActivity(ex.Message, isError: true);
        }
    }

    private void RefreshConsoleLines()
    {
        ConsoleLines.Clear();
        foreach (var line in _consoleService.Lines)
        {
            ConsoleLines.Add(line.Display);
        }
    }

    private async Task LoadPortsAsync()
    {
        var ports = await _portDiscovery.GetPortsAsync();
        Ports.Clear();
        foreach (var port in ports)
        {
            Ports.Add(port);
        }

        PortsEmptyHint = Ports.Count == 0
            ? "No COM ports found. Connect USB, install CH340/CP2102 driver, then click Refresh."
            : string.Empty;

        OnPropertyChanged(nameof(HasPorts));
        SelectedPort ??= Ports.FirstOrDefault();
        PortDisplay = SelectedPort?.PortName ?? "—";
        ConnectCommand.NotifyCanExecuteChanged();
    }

    private void RefreshJobs()
    {
        var snapshot = _queueService.GetSnapshot();
        Jobs.Clear();
        HistoryJobs.Clear();
        foreach (var job in snapshot.Jobs.OrderBy(j => j.QueueOrder))
        {
            if (job.Status is JobStatus.Completed or JobStatus.Failed or JobStatus.Cancelled)
            {
                HistoryJobs.Add(job);
            }

            Jobs.Add(job);
        }

        if (SelectedJob is not null)
        {
            SelectedJob = Jobs.FirstOrDefault(j => j.Id == SelectedJob.Id) ?? Jobs.FirstOrDefault();
        }
        else
        {
            SelectedJob = Jobs.FirstOrDefault(j => j.Status == JobStatus.Pending) ?? Jobs.FirstOrDefault();
        }

        OnPropertyChanged(nameof(HasCompletedJobs));
        RefreshCommandStates();
    }

    private void ApplyStatus(PrinterStatus status)
    {
        ConnectionState = status.ConnectionState;
        IsConnected = status.ConnectionState == ConnectionState.Connected;
        _printState = status.PrintState;
        PrintStateText = status.PrintState.ToString();
        Progress = status.Progress;
        CurrentJobName = string.IsNullOrWhiteSpace(status.CurrentJobName) ? "—" : status.CurrentJobName;
        ElapsedText = status.Elapsed > TimeSpan.Zero ? status.Elapsed.ToString(@"hh\:mm\:ss") : "—";
        EtaText = status.Eta?.ToString(@"hh\:mm\:ss") ?? "—";

        if (IsConnected && _serialEngine.FirmwareInfo is { } fw)
        {
            FirmwareInfo = $"{fw.FirmwareName} {fw.Version}";
        }
        else if (!IsConnected)
        {
            FirmwareInfo = "—";
        }

        ApplyTemperatureDisplay(status);
        ApplyFanDisplay(status);
        ApplyPositionDisplay(status);
        ApplyLayerDisplay(status);
        ApplyStatusHero(status);
        UpdateFooter(status);
        PortDisplay = SelectedPort?.PortName ?? "—";
        if (IsConnected && _serialEngine.FirmwareInfo is { } boardFw)
        {
            BoardInfo = string.IsNullOrWhiteSpace(boardFw.MachineType) ? "—" : boardFw.MachineType;
        }
        else if (!IsConnected)
        {
            BoardInfo = "—";
        }

        RefreshJobs();
        RefreshCommandStates();
    }

    private void ApplyStatusHero(PrinterStatus status)
    {
        if (status.ConnectionState == ConnectionState.Connecting)
        {
            StatusHeroText = "CONNECTING";
            StatusHeroSubtext = "Handshaking with printer…";
            return;
        }

        if (status.ConnectionState == ConnectionState.Error)
        {
            StatusHeroText = "ERROR";
            StatusHeroSubtext = "Check connection and retry";
            return;
        }

        if (status.ConnectionState == ConnectionState.Disconnected)
        {
            StatusHeroText = "OFFLINE";
            StatusHeroSubtext = "Connect to begin";
            return;
        }

        StatusHeroText = status.PrintState switch
        {
            PrintState.Printing => "PRINTING",
            PrintState.Paused => "PAUSED",
            PrintState.Preparing => "PREPARING",
            PrintState.Failed => "FAILED",
            PrintState.Cancelled => "CANCELLED",
            PrintState.Completed => "COMPLETED",
            _ => "IDLE"
        };

        StatusHeroSubtext = status.PrintState switch
        {
            PrintState.Printing => CurrentJobName,
            PrintState.Paused => "Tap Resume to continue",
            PrintState.Preparing => "Warming up…",
            PrintState.Failed => "See activity log",
            PrintState.Idle => "Ready to print",
            _ => PrintStateText
        };
    }

    private void UpdateFooter(PrinterStatus status)
    {
        var conn = status.ConnectionState switch
        {
            ConnectionState.Connected => "Connected",
            ConnectionState.Connecting => "Connecting",
            ConnectionState.Error => "Error",
            _ => "Disconnected"
        };

        FooterSummary = $"{conn} · {Progress:F0}% · {ElapsedText} · {HotendDisplay} / {BedDisplay}";
    }

    private void ApplyTemperatureDisplay(PrinterStatus status)
    {
        if (!status.HasLiveTemperature)
        {
            HotendDisplay = "—";
            HotendTargetDisplay = "—";
            BedDisplay = "—";
            BedTargetDisplay = "—";
            HotendHeatingLabel = string.Empty;
            BedHeatingLabel = string.Empty;
            IsHotendHeating = false;
            IsBedHeating = false;
            IsHotendAtTemp = false;
            IsBedAtTemp = false;
            return;
        }

        HotendDisplay = $"{status.HotendTemp:F1}°C";
        HotendTargetDisplay = status.TargetHotend > 0 ? $"{status.TargetHotend:F1}°C" : "—";
        BedDisplay = $"{status.BedTemp:F1}°C";
        BedTargetDisplay = status.TargetBed > 0 ? $"{status.TargetBed:F1}°C" : "—";

        IsHotendHeating = status.TargetHotend > 0
            && status.HotendTemp < status.TargetHotend - HeatingToleranceC;
        IsHotendAtTemp = status.TargetHotend > 0
            && status.HotendTemp >= status.TargetHotend - HeatingToleranceC;
        HotendHeatingLabel = IsHotendHeating ? "Heating ↑" : IsHotendAtTemp ? "At temp ✓" : string.Empty;

        IsBedHeating = status.TargetBed > 0
            && status.BedTemp < status.TargetBed - HeatingToleranceC;
        IsBedAtTemp = status.TargetBed > 0
            && status.BedTemp >= status.TargetBed - HeatingToleranceC;
        BedHeatingLabel = IsBedHeating ? "Heating ↑" : IsBedAtTemp ? "At temp ✓" : string.Empty;
    }

    private void ApplyFanDisplay(PrinterStatus status)
    {
        if (!status.HasLiveFan || status.FanSpeedPercent is not { } fan)
        {
            FanDisplay = "—";
            FanTargetDisplay = "—";
            return;
        }

        FanDisplay = $"{fan:F0}%";
        FanTargetDisplay = "—";
    }

    private void ApplyPositionDisplay(PrinterStatus status)
    {
        if (!status.HasLivePosition)
        {
            PositionDisplay = "X: —  Y: —  Z: —";
            return;
        }

        PositionDisplay =
            $"X:{status.PositionX:F1}  Y:{status.PositionY:F1}  Z:{status.PositionZ:F1}";
    }

    private void ApplyLayerDisplay(PrinterStatus status)
    {
        if (status.TotalLayers is null or <= 0)
        {
            LayerDisplay = "—";
            return;
        }

        LayerDisplay = status.CurrentLayer is { } current
            ? $"{current} / {status.TotalLayers}"
            : $"— / {status.TotalLayers}";
    }

    private void ShowThermalWarning(string message)
    {
        ThermalWarningMessage = message;
        HasThermalWarning = true;
        LogActivity(message, isError: true);
    }

    private void MarkSettingsDirty()
    {
        HasUnsavedSettings = true;
        SaveSettingsCommand.NotifyCanExecuteChanged();
    }

    private void LogActivity(string message, bool isError = false)
    {
        StatusMessage = message;
        ActivityLog.Insert(0, new ActivityLogEntry(message, isError));
        while (ActivityLog.Count > ActivityLogMax)
        {
            ActivityLog.RemoveAt(ActivityLog.Count - 1);
        }
    }

    private void RefreshCommandStates()
    {
        ConnectCommand.NotifyCanExecuteChanged();
        DisconnectCommand.NotifyCanExecuteChanged();
        RefreshPortsCommand.NotifyCanExecuteChanged();
        AddGCodeCommand.NotifyCanExecuteChanged();
        StartPrintCommand.NotifyCanExecuteChanged();
        PausePrintCommand.NotifyCanExecuteChanged();
        ResumePrintCommand.NotifyCanExecuteChanged();
        CancelPrintCommand.NotifyCanExecuteChanged();
        EmergencyStopCommand.NotifyCanExecuteChanged();
        RemoveJobCommand.NotifyCanExecuteChanged();
        RetryJobCommand.NotifyCanExecuteChanged();
        ClearCompletedCommand.NotifyCanExecuteChanged();
        SaveSettingsCommand.NotifyCanExecuteChanged();
        SendConsoleCommand.NotifyCanExecuteChanged();
        HomeAllCommand.NotifyCanExecuteChanged();
        DisableMotorsCommand.NotifyCanExecuteChanged();
        PreheatPlaCommand.NotifyCanExecuteChanged();
        PreheatPetgCommand.NotifyCanExecuteChanged();
        CooldownCommand.NotifyCanExecuteChanged();
        JogXMinusCommand.NotifyCanExecuteChanged();
        JogXPlusCommand.NotifyCanExecuteChanged();
        JogYMinusCommand.NotifyCanExecuteChanged();
        JogYPlusCommand.NotifyCanExecuteChanged();
        JogZMinusCommand.NotifyCanExecuteChanged();
        JogZPlusCommand.NotifyCanExecuteChanged();
        RunMacroCommand.NotifyCanExecuteChanged();
    }
}
