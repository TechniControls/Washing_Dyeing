using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using Avalonia.Media;
using Barca_Dyeing_Screen.Models;
using Barca_Dyeing_Screen.Services;
using Barca_Dyeing_Screen.Stores;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tmds.DBus.Protocol;

namespace Barca_Dyeing_Screen.ViewModels;

public partial class ConnectionViewModel : ViewModelBase
{
    private readonly ConnectionStore _connectionStore;
    private readonly ConnectionService _connectionService;
    private readonly SerialPortModel? _serialPortModel;

    [ObservableProperty] private string _selectedComPort;
    [ObservableProperty] private string _selectedBaudRate;
    [ObservableProperty] private string _selectedParity;
    [ObservableProperty] private string _selectedDataBits;
    [ObservableProperty] private string _selectedStopBits;

    public ConnectionViewModel(
        ConnectionStore connectionStore,
        ConnectionService connectionService)
    {
        _connectionStore = connectionStore;
        _connectionService = connectionService;
        _serialPortModel = new SerialPortModel();

        SelectedComPort = string.Empty;
        SelectedBaudRate = string.Empty;
        SelectedParity = string.Empty;
        SelectedDataBits = string.Empty;
        SelectedStopBits = string.Empty;
        
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


    public List<string>? PortNames => _serialPortModel?.PortNames;
    public List<string>? BaudRate => _serialPortModel?.BaudRate;
    public List<string>? Parity => _serialPortModel?.Parity;
    public List<string>? DataBits => _serialPortModel?.DataLength;
    public List<string>? StopBits => _serialPortModel?.StopBits;
    private bool ConnectionStatus => _connectionStore.IsConnected;
    public string ConnectionState => ConnectionStatus ? "Connected" : "Disconnected";
    public IBrush ConnectionStatusColor => ConnectionStatus ? Brushes.Green : Brushes.Orange;
    

    private void OnConnectionStoreChanged(object? sender, PropertyChangedEventArgs eventArgs)
    {
        if (eventArgs.PropertyName == nameof(_connectionStore.IsConnected))
        {
            OnPropertyChanged(nameof(ConnectionState));
            OnPropertyChanged(nameof(ConnectionStatusColor));
            ConnectSerialPortCommand.NotifyCanExecuteChanged();
            DisconnectSerialPortCommand.NotifyCanExecuteChanged();
        }
    }

    [RelayCommand]
    private void UpdatePorts()
    {
        if (PortNames != null) _connectionService.GetSerialPorts(PortNames);
    }

    private bool _canConnect() => !ConnectionStatus;
    private bool _canDisconnect() => ConnectionStatus;

    [RelayCommand(CanExecute = nameof(_canConnect))]
    private void ConnectSerialPort()
    {
        _connectionService.ConnectModbusRtu(
            SelectedComPort,
            SelectedBaudRate,
            SelectedParity,
            SelectedDataBits,
            SelectedStopBits);
    }

    [RelayCommand(CanExecute = nameof(_canDisconnect))]
    private void DisconnectSerialPort()
    {
        _connectionService.DisconnectModbusRtu();
    }
}