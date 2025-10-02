using Domain;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Documents;

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

        AddToHistory("TEST");
        AddToHistory("TEST");
        AddToHistory("TEST");
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
            var openDialog = new OpenFileDialog { Filter = "Text Files|*.txt" };
            if (openDialog.ShowDialog() != true)
                return;

            var readLines = fileHandler.Load(openDialog.FileName);

            viewModel.IsProcessing = true;

            await Parallel.ForEachAsync(readLines, async (line, ct) =>
            {
                //var result = await evaluator.ProcessAsync(line);
                //AddToHistorySafe(line + " = " + result);

                var result = evaluator.Process(line);
                AddToHistorySafe(line + " = " + result);
            });

            viewModel.IsProcessing = false;

            MessageBox.Show("Processing complete!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void AddToHistorySafe(string content)
    {
        bool isCurrentThreadUiThread = Thread.CurrentThread == Dispatcher.Thread;

        if (!isCurrentThreadUiThread)
        {
            Dispatcher.Invoke(() => AddToHistory(content));
        }
        else
        {
            AddToHistory(content);
        }
    }

    private void AddToHistory(string content) => History.AppendText($"{DateTime.Now:HH:MM:ss} {content} {Environment.NewLine}");
}