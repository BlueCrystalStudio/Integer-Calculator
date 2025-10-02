namespace Virtuplex_Calculator.ViewModels;

public class MainWindowViewModel : PropertyBinding
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
}
