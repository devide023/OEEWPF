using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace LBJOEE.Convert
{
    public class TimeConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double seconds;
            var isok = double.TryParse(value.ToString(), out seconds);
            if(!isok)
            {
                return "0";
            }
            TimeSpan ts = TimeSpan.FromSeconds(seconds);
            if (seconds == 0)
            {
                return 0.ToString();
            }
            else if (seconds <= 60)
            {
                return $"{seconds}秒";
            }
            else if (seconds > 60 && seconds < 3600)
            {
                return $"{ts.Minutes}分{ts.Seconds}秒";
            }
            else if (seconds >= 3600 && seconds < 3600 * 24)
            {
                return $"{ts.Hours}时{ts.Minutes}分{ts.Seconds}秒";
            }
            else
            {
                return $"{ts.Days}天{ts.Hours}时{ts.Minutes}分{ts.Seconds}秒";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
