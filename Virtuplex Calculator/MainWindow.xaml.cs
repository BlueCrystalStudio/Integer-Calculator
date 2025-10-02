using Domain;
using Microsoft.Win32;
using System.Collections.Concurrent;
using System.Windows;
using System.Windows.Documents;
using Utilities.Extensions;
using Virtuplex_Calculator.ViewModels;

namespace Virtuplex_Calculator;

public partial class MainWindow : Window
{
    MainWindowViewModel viewModel;

    IEvaluator evaluator;
    IFileHandler fileHandler;

    public MainWindow(MainWindowViewModel viewModel, IEvaluator evaluator, IFileHandler fileHandler)
    {
        InitializeComponent();

        this.viewModel = viewModel;
        this.evaluator = evaluator;
        this.fileHandler = fileHandler;

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

            // add result to History Controll
            AddToHistory($"{expression} = {result}");
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
            var openDialog = new OpenFileDialog { Filter = "Text Files|*.txt" };
            if (openDialog.ShowDialog() != true)
                return;

            var file = openDialog.FileName;
            var readLines = fileHandler.Load(file);
            var results = new ConcurrentBag<string>();

            viewModel.IsProcessing = true;

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
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void AddToHistory(string content) => History.AppendText($"{DateTime.Now:HH:mm:ss} {content} {Environment.NewLine}");
    private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) => DragMove();
}