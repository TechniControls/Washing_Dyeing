using System.Collections.ObjectModel;
using Barca_Dyeing_Screen.Configuration;
using Barca_Dyeing_Screen.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Barca_Dyeing_Screen.Stores;

public partial class AlarmStore : ObservableObject
{
    [ObservableProperty] private ObservableCollection<AlarmModel> _activeAlarms = new();

   
}