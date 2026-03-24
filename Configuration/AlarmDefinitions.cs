using System.Collections.ObjectModel;
using Barca_Dyeing_Screen.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Barca_Dyeing_Screen.Configuration;

public static class AlarmDefinitions 
{
    public static ObservableCollection<AlarmModel> AlarmModels =>
    [
        new AlarmModel { AlarmId = 0, AlarmName = "Drive Fault", AlarmDescription = "Overcurrent. Check motor" },
        new AlarmModel
            { AlarmId = 1, AlarmName = "Temperature Fault", AlarmDescription = "Overtemperature. Check PT100" }
    ];
}