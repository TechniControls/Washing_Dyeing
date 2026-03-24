using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Barca_Dyeing_Screen.Models;

public partial class SerialPortModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<string> _portNames = [];

    [ObservableProperty] private ObservableCollection<string> _baudRate =
    [
        "9600",
        "19200",
        "115200"
    ];

    [ObservableProperty] private ObservableCollection<string> _parity =
    [
        "Odd",
        "Even",
        "None"
    ];

    [ObservableProperty] private ObservableCollection<string> _dataLength =
    [
        "7",
        "8"
    ];

    [ObservableProperty] private ObservableCollection<string> _stopBits =
    [
        "1",
        "2"
    ];
}