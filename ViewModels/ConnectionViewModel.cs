using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using Barca_Dyeing_Screen.Models;
using Barca_Dyeing_Screen.Services;
using Barca_Dyeing_Screen.Stores;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using Tmds.DBus.Protocol;

namespace Barca_Dyeing_Screen.ViewModels;

public partial class ConnectionViewModel : ViewModelBase
{
    private readonly ConnectionStore _connectionStore;
    private readonly ConnectionService _connectionService;
    private readonly PlcService _plcService;
    private readonly AlarmService _alarmService;
    private readonly SerialPortModel? _serialPortModel;

    private CancellationTokenSource? _cancellationTokenSource;
    [ObservableProperty] private bool _connectionSuccessful;

    [ObservableProperty] private string _selectedComPort;
    [ObservableProperty] private string _selectedBaudRate;
    [ObservableProperty] private string _selectedParity;
    [ObservableProperty] private string _selectedDataBits;
    [ObservableProperty] private string _selectedStopBits;

    public ConnectionViewModel(
        ConnectionStore connectionStore,
        ConnectionService connectionService,
        PlcService plcService,
        AlarmService alarmService)
    {
        _connectionStore = connectionStore;
        _connectionService = connectionService;
        _plcService = plcService;
        _alarmService = alarmService;
        _serialPortModel = new SerialPortModel();

        SelectedComPort = string.Empty;
        SelectedBaudRate = string.Empty;
        SelectedParity = string.Empty;
        SelectedDataBits = string.Empty;
        SelectedStopBits = string.Empty;

        ConnectionSuccessful = false;
        PlcBitStatus = false;


        _connectionStore.PropertyChanged += OnConnectionStoreChanged;
    }

    #region Store Data

    // Store Selected Com Port
    partial void OnSelectedComPortChanged(string value)
    {
        _connectionStore.SelectedComPort = value;
        Debug.WriteLine(value);
        Debug.WriteLine(_connectionStore.SelectedComPort);
    }

    // Store Selected BaudRate
    partial void OnSelectedBaudRateChanged(string value)
    {
        _connectionStore.SelectedBaudRate = value;
        Debug.WriteLine(value);
        Debug.WriteLine(_connectionStore.SelectedBaudRate);
    }

    // Store Selected Parity
    partial void OnSelectedParityChanged(string value)
    {
        _connectionStore.SelectedParity = value;
        Debug.WriteLine(value);
        Debug.WriteLine(_connectionStore.SelectedParity);
    }

    // Store Selected DataBits
    partial void OnSelectedDataBitsChanged(string value)
    {
        _connectionStore.SelectedDataBits = value;
        Debug.WriteLine(value);
        Debug.WriteLine(_connectionStore.SelectedDataBits);
    }

    // Store Selected StopBits
    partial void OnSelectedStopBitsChanged(string value)
    {
        _connectionStore.SelectedStopBits = value;
        Debug.WriteLine(value);
        Debug.WriteLine(_connectionStore.SelectedStopBits);
    }

    #endregion

    #region Expose Collections

    public ObservableCollection<string>? PortNames => _serialPortModel?.PortNames;
    public ObservableCollection<string>? BaudRate => _serialPortModel?.BaudRate;
    public ObservableCollection<string>? Parity => _serialPortModel?.Parity;
    public ObservableCollection<string>? DataBits => _serialPortModel?.DataLength;
    public ObservableCollection<string>? StopBits => _serialPortModel?.StopBits;

    #endregion


    // Status Connection RTU
    private bool ConnectionStatus => _connectionStore.IsConnected;
    public string ConnectionRtuState => ConnectionStatus ? "Connected" : "Disconnected";
    public IBrush ConnectionRtuStatusColor => ConnectionStatus ? Brushes.Green : Brushes.Orange;

    // Status Connection PLC
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PlcStatus))]
    [NotifyPropertyChangedFor(nameof(PlcStatusColor))]
    private bool _plcBitStatus;

    public string PlcStatus => PlcBitStatus ? "Running" : "Stopped";
    public IBrush PlcStatusColor => PlcBitStatus ? Brushes.Green : Brushes.Orange;

    #region Read Bits for Trigger Alarms

    [ObservableProperty] private bool _almOne;

    #endregion


    #region Update On Connection Changed Event

    private void OnConnectionStoreChanged(object? sender, PropertyChangedEventArgs eventArgs)
    {
        if (eventArgs.PropertyName == nameof(_connectionStore.IsConnected))
        {
            OnPropertyChanged(nameof(ConnectionRtuState));
            OnPropertyChanged(nameof(ConnectionRtuStatusColor));
            ConnectSerialPortCommand.NotifyCanExecuteChanged();
            DisconnectSerialPortCommand.NotifyCanExecuteChanged();

            // Method for start reading parameters immediately device is connected also maintain connection alive
            HandleReadingOnConnection();
        }
    }

    #endregion


    [RelayCommand]
    private void UpdatePorts()
    {
        if (PortNames != null) _connectionService.GetSerialPorts(PortNames);
    }

    private bool _canConnect() => !ConnectionStatus;
    private bool _canDisconnect() => ConnectionStatus;

    [RelayCommand(CanExecute = nameof(_canConnect))]
    private async Task ConnectSerialPort()
    {
        if (string.IsNullOrEmpty(SelectedComPort)
            || string.IsNullOrEmpty(SelectedBaudRate)
            || string.IsNullOrEmpty(SelectedParity)
            || string.IsNullOrEmpty(SelectedDataBits)
            || string.IsNullOrEmpty(SelectedStopBits))
        {
            var messageBox = MessageBoxManager.GetMessageBoxStandard(
                new MessageBoxStandardParams
                {
                    ContentTitle = "Parameters Empty",
                    ContentMessage = "Parameters can not be empty",
                    Width = 300,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                });
            await messageBox.ShowWindowAsync();
            return;
        }

        var messageBoxConfirm = MessageBoxManager.GetMessageBoxStandard(
            new MessageBoxStandardParams
            {
                ContentTitle = "Confirmation",
                ContentMessage = "PLC already connected and the status is RUN?",
                Width = 300,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ButtonDefinitions = ButtonEnum.YesNo
            });

        var result = await messageBoxConfirm.ShowAsync();

        if (result == ButtonResult.Yes)
        {
            await _connectionService.ConnectModbusRtu(
                SelectedComPort,
                SelectedBaudRate,
                SelectedParity,
                SelectedDataBits,
                SelectedStopBits);
        }

        if (result == ButtonResult.No)
        {
            var messageBoxInfo = MessageBoxManager.GetMessageBoxStandard(
                new MessageBoxStandardParams
                {
                    ContentTitle = "Retry",
                    ContentMessage = "Connect the plc and confirm the current status is RUN",
                    Width = 300,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                });

            await messageBoxInfo.ShowAsync();
        }
    }

    // Task for automatic start reading
    private async Task StartReading()
    {
        if (_cancellationTokenSource != null) return;
        _cancellationTokenSource = new CancellationTokenSource();

        try
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                if (!ConnectionStatus)
                {
                    StopReading();
                    break;
                }

                var taskOne = _plcService.ReadParameters(_cancellationTokenSource.Token);
                var taskTwo = _alarmService.UpdateAlarms(_cancellationTokenSource.Token);

                await Task.WhenAll(taskOne, taskTwo);

                var resultOne = taskOne.Result;

                PlcBitStatus = resultOne[0];
                Debug.WriteLine($"Result two {resultOne[0]}");

                await Task.Delay(1000, _cancellationTokenSource.Token);
            }
        }
        catch (TaskCanceledException e)
        {
            Debug.WriteLine(e.Message);
        }
    }

    private void StopReading()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = null;
        PlcBitStatus = false;
    }

    private void HandleReadingOnConnection()
    {
        if (_connectionStore.IsConnected && _cancellationTokenSource == null)
        {
            _ = StartReading();
        }
        else
        {
            StopReading();
        }
    }

    [RelayCommand(CanExecute = nameof(_canDisconnect))]
    private void DisconnectSerialPort()
    {
        _connectionService.DisconnectModbusRtu();
        StopReading();
    }
}