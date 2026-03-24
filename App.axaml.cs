using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Barca_Dyeing_Screen.Services;
using Barca_Dyeing_Screen.Stores;
using Barca_Dyeing_Screen.ViewModels;
using Barca_Dyeing_Screen.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Barca_Dyeing_Screen;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var collection = new ServiceCollection();
        
        // Services
        collection.AddSingleton<ConnectionService>();
        collection.AddSingleton<PlcService>();
        collection.AddSingleton<AlarmService>();
        collection.AddSingleton<INavigationService, NavigationService>();
        
        // Stores
        collection.AddSingleton<ConnectionStore>();
        
        // ViewModels
        collection.AddSingleton<MainWindowViewModel>();
        collection.AddSingleton<ConnectionViewModel>();
        collection.AddTransient<ControlViewModel>();
        collection.AddTransient<SettingsViewModel>();
        collection.AddSingleton<AlarmsViewModel>();
        
        var serviceProvider = collection.BuildServiceProvider();
        var mainViewModel = serviceProvider.GetRequiredService<MainWindowViewModel>();
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = new MainWindow
            {
                DataContext = mainViewModel,
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}