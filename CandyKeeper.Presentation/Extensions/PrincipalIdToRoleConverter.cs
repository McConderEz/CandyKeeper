using System.Globalization;
using System.Windows.Data;
using CandyKeeper.Presentation.ViewModels.Base;

namespace CandyKeeper.Presentation.Extensions;

public class PrincipalIdToRoleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int principalId)
        {
            return UserViewModel.Roles.SingleOrDefault(r => r.PrincipalId == principalId).Name;
        }
        return "Unknown Role"; // Возвращает это значение, если роль не найдена
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}