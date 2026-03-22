using System;
using Barca_Dyeing_Screen.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace Barca_Dyeing_Screen.Services;

public partial class NavigationService (IServiceProvider serviceProvider) : ViewModelBase, INavigationService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    
    [ObservableProperty]
    private ViewModelBase? _currentViewModel;

    public void NavigateTo<T>() where T : ViewModelBase
    {
        CurrentViewModel = _serviceProvider.GetRequiredService<T>();
    }
}