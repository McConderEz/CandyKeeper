﻿using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CandyKeeper.Presentation.Extensions;

public class BooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return !boolValue ? Visibility.Collapsed : Visibility.Visible;
        }

        return Visibility.Collapsed;
    }
}