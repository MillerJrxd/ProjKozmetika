using System.Globalization;
using System.Windows.Data;

namespace ProjKozmetika.Classes
{
    public class BoolToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "Aktív" : "Inaktív";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString() == "Aktív";
        }
    }
}
