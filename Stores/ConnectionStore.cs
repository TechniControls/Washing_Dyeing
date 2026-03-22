using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Modbus.Serial;

namespace Barca_Dyeing_Screen.Stores;

public partial class ConnectionStore : ObservableObject
{
    [ObservableProperty] private string _selectedComPort = string.Empty;
    [ObservableProperty] private string _selectedParity = string.Empty;
    [ObservableProperty] private string _selectedDataBits = string.Empty;
    [ObservableProperty] private string _selectedStopBits = string.Empty;
    [ObservableProperty] private string _selectedBaudRate = string.Empty;
    [ObservableProperty] private bool _isConnected = false;
}