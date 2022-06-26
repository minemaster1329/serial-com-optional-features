using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SerialCom.Frontend
{
    internal class BooleanToStringValueConverter : IValueConverter
    {
        public string TrueString { get; set; } = "True String";
        public string FalseString { get; set; } = "False String";
        public string NullString { get; set; } = "Null String";
        public string InvalidString { get; set; } = "Invalid String";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                null => NullString,
                bool b => b ? TrueString : FalseString,
                _ => InvalidString
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
