using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarlinPrintMiddleware.Core.Enums;
using MarlinPrintMiddleware.Core.Interfaces;
using MarlinPrintMiddleware.Core.Models;
using Microsoft.Win32;

namespace MarlinPrintMiddleware.UI.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly ISerialPortDiscovery _portDiscovery;
    private readonly ISerialEngine _serialEngine;
    private readonly IPrintQueueService _queueService;
    private readonly IPrinterStatusService _statusService;
    private readonly IEmergencyStopService _emergencyStopService;
    private readonly IPauseResumeService _pauseResumeService;
    private readonly IPrinterProfileRepository _profileRepository;
    private readonly ISettingsRepository _settingsRepository;

    private const string AutoStartSettingKey = "queue.auto_start";

    public MainViewModel(
        ISerialPortDiscovery portDiscovery,
        ISerialEngine serialEngine,
        IPrintQueueService queueService,
        IPrinterStatusService statusService,
        IEmergencyStopService emergencyStopService,
        IPauseResumeService pauseResumeService,
        IPrinterProfileRepository profileRepository,
        ISettingsRepository settingsRepository)
    {
        _portDiscovery = portDiscovery;
        _serialEngine = serialEngine;
        _queueService = queueService;
        _statusService = statusService;
        _emergencyStopService = emergencyStopService;
        _pauseResumeService = pauseResumeService;
        _profileRepository = profileRepository;
        _settingsRepository = settingsRepository;

        Jobs = new ObservableCollection<PrintJob>();
        Ports = new ObservableCollection<SerialPortInfo>();

        _statusService.StatusChanged += (_, status) =>
        {
            var dispatcher = Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;
            dispatcher.Invoke(() => ApplyStatus(status));
        };

        _ = InitializeAsync();
    }

    public ObservableCollection<PrintJob> Jobs { get; }

    public ObservableCollection<SerialPortInfo> Ports { get; }

    [ObservableProperty] private SerialPortInfo? _selectedPort;

    [ObservableProperty] private int _baudRate = 115200;

    [ObservableProperty] private string _firmwareInfo = "—";

    [ObservableProperty] private string _connectionStatus = "Disconnected";

    [ObservableProperty] private double _hotendTemp;

    [ObservableProperty] private double _bedTemp;

    [ObservableProperty] private double _targetHotend;

    [ObservableProperty] private double _targetBed;

    [ObservableProperty] private double _progress;

    [ObservableProperty] private string _currentJobName = "—";

    [ObservableProperty] private string _elapsedText = "00:00:00";

    [ObservableProperty] private string _etaText = "—";

    [ObservableProperty] private string _printStateText = "Idle";

    [ObservableProperty] private string _statusMessage = "Ready";

    [ObservableProperty] private bool _isConnected;

    [ObservableProperty] private bool _isBusy;

    [ObservableProperty] private bool _autoStartQueue;

    [RelayCommand]
    private async Task RefreshPortsAsync()
    {
        await _portDiscovery.RefreshAsync();
        await LoadPortsAsync();
    }

    [RelayCommand]
    private async Task ConnectAsync()
    {
        if (SelectedPort is null)
        {
            StatusMessage = "Select a COM port.";
            return;
        }

        try
        {
            IsBusy = true;
            StatusMessage = "Connecting…";
            await _serialEngine.ConnectAsync(SelectedPort.PortName, BaudRate);
            FirmwareInfo = _serialEngine.FirmwareInfo is { } info
                ? $"{info.FirmwareName} {info.Version}"
                : "Marlin";
            IsConnected = true;
            ConnectionStatus = "Connected";
            StatusMessage = "Connected.";
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            IsConnected = false;
            ConnectionStatus = "Error";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task DisconnectAsync()
    {
        try
        {
            IsBusy = true;
            await _serialEngine.DisconnectAsync();
            IsConnected = false;
            ConnectionStatus = "Disconnected";
            FirmwareInfo = "—";
            StatusMessage = "Disconnected.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
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

        await _queueService.EnqueueAsync(new PrintJob { FilePath = dialog.FileName });
        RefreshJobs();
        StatusMessage = $"Added {System.IO.Path.GetFileName(dialog.FileName)}";
    }

    [RelayCommand]
    private async Task StartPrintAsync()
    {
        try
        {
            await _queueService.StartNextAsync();
            RefreshJobs();
            StatusMessage = "Print started.";
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }

    [RelayCommand]
    private async Task PausePrintAsync()
    {
        try
        {
            await _pauseResumeService.PauseAsync();
            StatusMessage = "Print paused.";
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }

    [RelayCommand]
    private async Task ResumePrintAsync()
    {
        try
        {
            await _pauseResumeService.ResumeAsync();
            StatusMessage = "Print resumed.";
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }

    [RelayCommand]
    private async Task CancelPrintAsync()
    {
        await _queueService.CancelAsync();
        RefreshJobs();
        StatusMessage = "Print cancelled.";
    }

    [RelayCommand]
    private async Task EmergencyStopAsync()
    {
        await _emergencyStopService.TriggerAsync();
        RefreshJobs();
        StatusMessage = "EMERGENCY STOP sent.";
    }

    [RelayCommand]
    private async Task SaveSettingsAsync()
    {
        await _settingsRepository.SetAsync(AutoStartSettingKey, AutoStartQueue);
        StatusMessage = "Settings saved.";
    }

    private async Task InitializeAsync()
    {
        var profile = await _profileRepository.GetDefaultAsync();
        if (profile is not null)
        {
            BaudRate = profile.BaudRate;
        }

        var autoStart = await _settingsRepository.GetAsync<bool>(AutoStartSettingKey);
        AutoStartQueue = autoStart == true;

        await LoadPortsAsync();
        RefreshJobs();
        ApplyStatus(_statusService.CurrentStatus);
    }

    private async Task LoadPortsAsync()
    {
        var ports = await _portDiscovery.GetPortsAsync();
        Ports.Clear();
        foreach (var port in ports)
        {
            Ports.Add(port);
        }

        SelectedPort ??= Ports.FirstOrDefault();
    }

    private void RefreshJobs()
    {
        var snapshot = _queueService.GetSnapshot();
        Jobs.Clear();
        foreach (var job in snapshot.Jobs)
        {
            Jobs.Add(job);
        }
    }

    private void ApplyStatus(PrinterStatus status)
    {
        HotendTemp = status.HotendTemp;
        BedTemp = status.BedTemp;
        TargetHotend = status.TargetHotend;
        TargetBed = status.TargetBed;
        Progress = status.Progress;
        CurrentJobName = status.CurrentJobName ?? "—";
        ElapsedText = status.Elapsed.ToString(@"hh\:mm\:ss");
        EtaText = status.Eta?.ToString(@"hh\:mm\:ss") ?? "—";
        PrintStateText = status.PrintState.ToString();
        IsConnected = status.ConnectionState == ConnectionState.Connected;
        ConnectionStatus = status.ConnectionState.ToString();
        RefreshJobs();
    }
}
