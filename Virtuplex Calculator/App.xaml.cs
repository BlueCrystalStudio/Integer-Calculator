using Domain;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Configuration;
using System.Data;
using System.Windows;
using Virtuplex_Calculator.ViewModels;

namespace Virtuplex_Calculator;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Dependency injection setup
        ServiceCollection services = new();
        ConfigureServices(services);

        ServiceProvider servicesProvider = services.BuildServiceProvider();

        var mainWindow = servicesProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    public void ConfigureServices(ServiceCollection services)
    {
        services.AddSingleton<MainWindow>();
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<IEvaluator, Evaluator>();
        services.AddTransient<IFileHandler, FileHandler>();
    }
}
