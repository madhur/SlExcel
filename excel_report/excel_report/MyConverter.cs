using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Globalization;
using System.Windows.Data;

namespace excel_report
{
    public class MyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime)
            {
                if ((DateTime)value == DateTime.MinValue)
                    return string.Empty;
                else
                    return ((DateTime)value).ToString("M/d/yyyy");
            }
            else if (value is double)
            {
                if ((double)value == double.Parse("0.0") || (double)value==double.Parse("0"))
                    return string.Empty;
                else
                    return value;
            }
            else if (value is decimal)
            {
                if ((decimal)value == decimal.Parse("0.0") || (decimal)value == decimal.Parse("0"))
                    return string.Empty;
                else
                    return value;
            }
            
            else
                return string.Empty;
        }


        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}
