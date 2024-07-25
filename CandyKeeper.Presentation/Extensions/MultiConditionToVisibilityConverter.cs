using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CandyKeeper.Presentation.Extensions;

public class MultiConditionToVisibilityConverter: IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length == 0)
            return Visibility.Collapsed;

        
        return !values.Contains(true) ? Visibility.Collapsed : Visibility.Visible;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}