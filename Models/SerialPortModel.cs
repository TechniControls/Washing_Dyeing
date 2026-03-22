using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Barca_Dyeing_Screen.Models;

public partial class SerialPortModel : ObservableObject
{
    [ObservableProperty] private List<string> _portNames = new();

    [ObservableProperty] private List<string> _baudRate = new()
    {
        "9600",
        "19200",
        "115200"
    };

    [ObservableProperty] private List<string> _parity = new()
    {
        "Odd",
        "Even",
        "None"
    };

    [ObservableProperty] private List<string> _dataLength = new()
    {
        "7",
        "8"
    };

    [ObservableProperty] private List<string> _stopBits = new()
    {
        "1",
        "2"
    };
}