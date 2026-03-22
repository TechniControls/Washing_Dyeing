using Barca_Dyeing_Screen.ViewModels;

namespace Barca_Dyeing_Screen.Services;

public interface INavigationService
{
    ViewModelBase CurrentViewModel { get; }
    void NavigateTo<T>() where T: ViewModelBase;
}