using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using AutoMapper;

namespace CandyKeeper.Presentation.Extensions;

internal class ComboBoxItemToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is ComboBoxItem selectedItem && parameter is string targetContent)
        {
            return selectedItem.Content.ToString() == targetContent ? Visibility.Visible : Visibility.Collapsed;
        }

        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}