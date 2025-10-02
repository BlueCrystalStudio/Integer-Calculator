using Domain;
using Microsoft.Win32;
using System.Collections.Concurrent;
using System.Windows;
using System.Windows.Documents;
using Utilities.Extensions;
using Virtuplex_Calculator.ViewModels;
using ConfigurationManager = Infrastructure.Configuration.ConfigurationManager;

namespace Virtuplex_Calculator;

public partial class MainWindow : Window
{
    MainWindowViewModel viewModel;
    ConfigurationManager configurationManager;  // remove?

    IEvaluator evaluator;
    IFileHandler fileHandler;

    public MainWindow(
        MainWindowViewModel viewModel,
        ConfigurationManager configManager,
        IEvaluator evaluator,
        IFileHandler fileHandler)
    {
        InitializeComponent();

        this.viewModel = viewModel;
        this.evaluator = evaluator;
        this.fileHandler = fileHandler;
        configurationManager = configManager;

        DataContext = this.viewModel;

        // Set line height for History Controll
        (History.Document.Blocks.FirstBlock as Paragraph).LineHeight = 1;
    }

    private void EvaluateButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            string expression = InputField.Text.Trim();
            string result = evaluator.Process(expression);

            AddToHistory($"{expression} = {result}");

            if (viewModel.SaveToOutputFolder)
            {
                SaveResults([result]);
            }
        }
        catch(Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

    private async void LoadFileButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var operationStart = DateTime.Now;
            var openDialog = new OpenFileDialog { Filter = "Text Files|*.txt", DefaultDirectory = viewModel.LastEvaluatedFile };
            if (openDialog.ShowDialog() != true)
                return;

            var file = openDialog.FileName;
            var readLines = fileHandler.Load(file);
            var results = new ConcurrentBag<string>();

            viewModel.IsProcessing = true;
            viewModel.LastEvaluatedFile = file;

            foreach (var line in readLines)
            {
                var result = await evaluator.ProcessAsync(line);
                results.Add(result);
            }

            // Append all results to History Controll on the UI thread
            await Dispatcher.InvokeAsync(() =>
            {
                foreach (var entry in results)
                {
                    AddToHistory(entry);
                }
            });

            viewModel.IsProcessing = false;

            var operationTimeComplexity = (DateTime.Now - operationStart).ToSecondsDisplayTime();
            MessageBox.Show($"Processing of {file} complete! Operation took: {operationTimeComplexity}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            if (viewModel.SaveToOutputFolder)
            {
                SaveResults(results);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void SaveResults(IEnumerable<string> results)
    {
        if(!results.Any())
            return;

        var outputFilePath = viewModel.OutputFolderPath;
        _ = fileHandler.Save(outputFilePath, results.ToArray());
        MessageBox.Show($"Results saved to: {outputFilePath}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void AddToHistory(string content) => History.AppendText($"{DateTime.Now:HH:mm:ss} {content} {Environment.NewLine}");
    private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) => DragMove();

    private void PathEdit_Click(object sender, RoutedEventArgs e)
    {
        var saveDialog = new SaveFileDialog { Filter = "Text Files|*.txt" };
        if (saveDialog.ShowDialog() != true)
            return;

        viewModel.OutputFolderPath = saveDialog.FileName;
    }

    // Why there isn't a single event for this :D, WPF is weird sometimes
    private void OutputFolderEnable_Checked(object sender, RoutedEventArgs e) => viewModel.SaveToOutputFolder = true;
    private void OutputFolderEnable_Unchecked(object sender, RoutedEventArgs e) => viewModel.SaveToOutputFolder = false;
}