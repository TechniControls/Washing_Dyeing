using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using Barca_Dyeing_Screen.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Modbus.Device;
using Modbus.Serial;

namespace Barca_Dyeing_Screen.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private INavigationService _navigationService;

    public MainWindowViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        _navigationService.NavigateTo<ConnectionViewModel>();
    }

    [ObservableProperty] private bool _openPane;

    [RelayCommand]
    private void ToggleSplitViewPane()
    {
        OpenPane = !OpenPane;
    }

    [RelayCommand]
    private void NavigateToControlView()
    {
        NavigationService.NavigateTo<ControlViewModel>();
    }

    [RelayCommand]
    private void NavigateToConnectionView()
    {
        NavigationService.NavigateTo<ConnectionViewModel>();
    }


    // [ObservableProperty]
    // private string _values = string.Empty;
    // private const byte slaveId = 1;
    // private const ushort ReadPlcStatus = 3120;
    // private const ushort StartDrive = 4096;
    // private const ushort SetPointFrequency = 4106;
    // private const ushort OutputFrequency = 5146;
    // [ObservableProperty]
    // private ushort _frequencyValue;
    // [ObservableProperty] private bool _monitoring = false;

    // partial void OnMonitoringChanged(bool value)
    // {
    //     Task.Run(() => MonitorLoop(_cts.Token));
    // }
    //
    // private CancellationTokenSource? _cts;
    //
    // [RelayCommand]
    // private async Task Connect()
    // {
    //
    //     _cts = new CancellationTokenSource();
    //
    //     Monitoring = true;
    // }
    //
    // private async Task MonitorLoop(CancellationToken token)
    // {
    //     while (!token.IsCancellationRequested)
    //     {
    //         try
    //         {
    //             var data = await Master.ReadCoilsAsync(slaveId, ReadPlcStatus, 1);
    //             var frequency = await Master.ReadHoldingRegistersAsync(slaveId, OutputFrequency, 1);
    //
    //             bool m0 = data[0];
    //             ushort freq = frequency[0];
    //
    //             Values = ($"M0: {m0}, Hz: {freq}");
    //         }
    //         catch (Exception ex)
    //         {
    //             Debug.WriteLine(ex.Message);
    //         }
    //
    //         await Task.Delay(200); // 🔥 CLAVE: no saturar bus
    //     }
    // }
    //
    // [RelayCommand]
    // private async Task Run()
    // {
    //     await Master.WriteSingleRegisterAsync(slaveId, StartDrive, 1);
    // }
    //
    // [RelayCommand]
    // private async Task Stop()
    // {
    //     await Master.WriteSingleRegisterAsync(slaveId, StartDrive, 0);
    // }
    //
    // [RelayCommand]
    // private async Task WriteFrequency()
    // {
    //     await Master.WriteSingleRegisterAsync(slaveId, SetPointFrequency, FrequencyValue);
    // }
}