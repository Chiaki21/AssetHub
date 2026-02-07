using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace AssetHub.Converters
{
    // Turns True -> "Active", False -> "Inactive"
    public class BooleanToStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "Active" : "Inactive";
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    // Turns True -> Green, False -> Red
    public class BooleanToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Brushes.Green : Brushes.Red;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}