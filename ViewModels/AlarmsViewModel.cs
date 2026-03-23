using System.Collections.Generic;
using System.Collections.ObjectModel;
using Barca_Dyeing_Screen.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Barca_Dyeing_Screen.ViewModels;

public partial class AlarmsViewModel : ViewModelBase
{
    [ObservableProperty] private ObservableCollection<AlarmModel> _alarmsCollection;
    

    public AlarmsViewModel()
    {
        var alarm = new List<AlarmModel>
        {
            new AlarmModel("Drive", "Overcurrent. Check motor"),
            new AlarmModel("Temperature", "Temperature out of range."),
        };

        AlarmsCollection = new ObservableCollection<AlarmModel>(alarm);
    }
}