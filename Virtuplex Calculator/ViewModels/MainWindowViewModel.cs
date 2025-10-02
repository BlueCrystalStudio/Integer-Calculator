using ConfigurationManager = Infrastructure.Configuration.ConfigurationManager;

namespace Virtuplex_Calculator.ViewModels;

public class MainWindowViewModel(ConfigurationManager configurationManager) : PropertyBinding
{
    private bool isProcessing = false;

    public bool IsProcessing
    {
        get => isProcessing;
        set
        {
            isProcessing = value;
            OnPropertyChanged();
        }
    }

    public bool SaveToOutputFolder
    { 
        get => configurationManager.Configuration.IsOutputToFolderEnabled;
        set
        {
            configurationManager.ChangeOutputExportOptionPath(value);
            OnPropertyChanged();
        }
    }

    public string OutputFolderPath
    {
        get => configurationManager.Configuration.OutputPath;
        set
        {
            configurationManager.ChangeOutputFolderPath(value);
            OnPropertyChanged();
        }
    }

    public string LastEvaluatedFile
    {
        get => configurationManager.Configuration.LastEvaluatedFile;
        set
        {
            configurationManager.ChangeLastEvaluatedFile(value);
            OnPropertyChanged();
        }
    }
}
