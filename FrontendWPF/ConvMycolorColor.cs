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

namespace FrontendWPF
{
    public class ConvMycolorColor : IValueConverter
    {
        private static System.Drawing.Color color;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            color = System.Drawing.Color.FromArgb(((Models.Color)value).ColorNumber);
            return new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
