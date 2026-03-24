using System.Collections.Generic;
using System.Collections.ObjectModel;
using Barca_Dyeing_Screen.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Barca_Dyeing_Screen.ViewModels;

public partial class AlarmsViewModel : ViewModelBase
{
    private readonly ConnectionViewModel _connectionViewModel;
    [ObservableProperty] private ObservableCollection<AlarmModel> _alarmsCollection;

    public bool AlarmOneTrigger => _connectionViewModel.AlmOne;
    

    public AlarmsViewModel(ConnectionViewModel connectionViewModel)
    {
        _connectionViewModel = connectionViewModel;
        var alarm = new List<AlarmModel>
        {
            new AlarmModel("Drive", "Overcurrent. Check motor"),
            new AlarmModel("Temperature", "Temperature out of range."),
            new AlarmModel("Drive", "Speed out of range."),
            new AlarmModel("Temperature", "PT100 break."),
        };

        AlarmsCollection = new ObservableCollection<AlarmModel>(alarm);
    }
}