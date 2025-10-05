using Domain;
using Domain.Constants;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Documents;
using Utilities.Extensions;
using Virtuplex_Calculator.ViewModels;

namespace Virtuplex_Calculator;

public partial class MainWindow : Window
{
    readonly MainWindowViewModel viewModel;

    readonly IEvaluator evaluator;
    readonly IEvaluator bigNumbersEvaluator;
    readonly IFileHandler fileHandler;

    public MainWindow(
        MainWindowViewModel viewModel,
        [FromKeyedServices(DependencyInjectionConstants.DataTableEvaluator)] IEvaluator evaluator,
        [FromKeyedServices(DependencyInjectionConstants.BigNumbersEvaluator)] IEvaluator bigNumbersEvaluator,
        IFileHandler fileHandler)
    {
        InitializeComponent();

        this.viewModel = viewModel;
        this.evaluator = evaluator;
        this.bigNumbersEvaluator = bigNumbersEvaluator;
        this.fileHandler = fileHandler;

        DataContext = this.viewModel;

        // Set line height for History Controll
        (History.Document.Blocks.FirstBlock as Paragraph).LineHeight = 1;
    }

    private async void EvaluateButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            string expression = InputField.Text.Trim();
            var results = await ProcessExpressions([expression]);

            await AddResultsToHistory(results);

            if (viewModel.SaveToOutputFolder)
            {
                SaveResults(results);
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

            OverlayText.Content = "Loading...";
            var file = openDialog.FileName;
            var readLines = await fileHandler.Load(file);

            viewModel.LastEvaluatedFile = file;

            var results = await ProcessExpressions(readLines);
            var operationTimeComplexity = (DateTime.Now - operationStart).ToSecondsDisplayTime();

            // TODO: Maybe create an Option for this?
            await AddResultsToHistory(results);

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

    private async Task<List<string>> ProcessExpressions(IEnumerable<string> expressions)
    {
        var results = new List<string>();

        viewModel.IsProcessing = true;
        OverlayText.Content = "Processing...";

        foreach (var line in expressions)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            try
            {
                var result = await evaluator.ProcessAsync(line);
                results.Add(result);
            }
            catch(OverflowException)
            {
                try
                {
                    var bigNumberResult = await bigNumbersEvaluator.ProcessAsync(line);
                    results.Add(bigNumberResult);
                    continue;
                }
                catch
                {
                    // If BigNumbersEvaluator also fails, just continue with the next expression
                    results.Add("Error: Result could not be solved.");
                    continue;
                }
            }
            catch (Exception ex)
            {
                results.Add($"Error: {ex.Message}");
                continue;
            }
        }

        viewModel.IsProcessing = false;

        return results;
    }

    private async Task AddResultsToHistory(IEnumerable<string> results)
    {
        OverlayText.Content = "Adding to History...";

        // Append all results to History Controll on the UI thread
        await Dispatcher.InvokeAsync(() =>
        {
            Task.Run(() =>
            {
                foreach (var entry in results)
                {
                    AddToHistory(entry);
                    Task.Delay(100).GetAwaiter().GetResult();
                }
            });

        });
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