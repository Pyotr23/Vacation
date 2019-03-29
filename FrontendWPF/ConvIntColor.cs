using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using FrontendWPF.Models;

namespace FrontendWPF
{
    public class ConvIntColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {            
            return ((IEnumerable<Models.Color>)value).Select(x => System.Drawing.Color.FromArgb(x.ColorNumber));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null; ;
        }
    }
}
