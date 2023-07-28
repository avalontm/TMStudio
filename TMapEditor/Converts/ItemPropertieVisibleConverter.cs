using Avalonia.Data.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TMStudio.Models;

namespace TMStudio.Converts
{
    public class ItemPropertieVisibleConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            int type = (int)value;

            if(type == 0 && parameter.ToString() == "string")
            {
                return true;
            }
            else if (type == 1 && parameter.ToString() == "bool")
            {
                return true;
            }
            else if (type == 2 && parameter.ToString() == "array")
            {
                return true;
            }

            return false;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
