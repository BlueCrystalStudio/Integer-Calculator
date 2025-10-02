using Domain;
using Infrastructure;
using Infrastructure.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using Virtuplex_Calculator.ViewModels;
using ConfigurationManager = Infrastructure.Configuration.ConfigurationManager;

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
        services.AddSingleton<ConfigurationManager>();
        services.AddSingleton<MainWindowViewModel>();

        services.AddTransient<IEvaluator, Evaluator>();
        services.AddTransient<IFileHandler, FileHandler>();
        services.AddTransient<IXMLHandler, GenericXMLHandler>();
    }
}
