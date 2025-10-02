using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace Virtuplex_Calculator.Convertors;

public class BoolToVisibilityConverter : IValueConverter
{
    public bool Invert { get; set; } = false;
    public bool CollapseInsteadOfHidden { get; set; } = true;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not bool boolValue)
            return DependencyProperty.UnsetValue;

        if (Invert)
            boolValue = !boolValue;

        if (boolValue)
            return Visibility.Visible;

        return CollapseInsteadOfHidden ? Visibility.Collapsed : Visibility.Hidden;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Visibility visibility)
        {
            bool result = visibility == Visibility.Visible;
            return Invert ? !result : result;
        }

        return false;
    }
}
