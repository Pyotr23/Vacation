using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using FrontendWPF.Models;
//using System.Windows.Media;

namespace FrontendWPF
{
    public class ConvIntColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Dictionary<System.Drawing.Color, string> ColorNames = new Dictionary<Color, string>();
            foreach (var color in typeof(Colors).GetProperties())
            {
                ColorNames[(System.Windows.Media.Color)color.GetValue(null)] = color.Name;
            }
            return ((IEnumerable<Models.Color>)value).Select(x => System.Drawing.Color.FromArgb(x.ColorNumber));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null; 
        }
    }
}
