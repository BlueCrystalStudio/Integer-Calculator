using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Virtuplex_Calculator.ViewModels;
public class PropertyBinding : INotifyPropertyChanged
{
#pragma warning disable CS8612 // Nullability of reference types in type doesn't match implicitly implemented member.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
#pragma warning restore CS8612 // Nullability of reference types in type doesn't match implicitly implemented member.
    public void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
