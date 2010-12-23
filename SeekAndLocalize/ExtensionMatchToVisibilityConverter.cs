using System;
using System.Windows;
using System.Windows.Data;

namespace SeekAndLocalize.Presentation
{
    class ExtensionMatchToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || parameter == null)
                return Visibility.Collapsed;
            var isMatch = value.ToString().ToLower() == parameter.ToString().ToLower();
            return isMatch ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
