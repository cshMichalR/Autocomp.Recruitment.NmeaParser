using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Autocomp.Nmea.Desktop.Converters
{
    public class TimeSpanToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is TimeSpan)
            {
                var timeSpanObj = (TimeSpan)value ;
                return $"{timeSpanObj.Hours.ToString("00")}:{timeSpanObj.Minutes.ToString("00")}:{timeSpanObj.Seconds.ToString("00")}.{timeSpanObj.Milliseconds.ToString("000")}";
            }
            return string.Empty;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
