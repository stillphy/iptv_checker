using System;
using System.Globalization;
using System.Windows.Data;

namespace IPTV_Checker_2
{
    [ValueConversion(typeof(bool), typeof(string))]
    public class EnableDisableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is bool boolean) || !boolean)
            {
                return "#9e9e9e";
            }
            return "#000000";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
