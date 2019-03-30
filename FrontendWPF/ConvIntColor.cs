using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using FrontendWPF.Models;
//using System.Windows.Media;

namespace FrontendWPF
{
    public class ConvIntColor : IValueConverter
    {
        Dictionary<int, string> colorDict = new Dictionary<int, string>{
            [-16777216] = "Чёрный",            
            [-16744448] = "Зелёный",
            [-16776961] = "Синий",
            [-65536] = "Красный",
            [-256] = "Жёлтый",
            [-8388480] = "Фиолетовый",
            [-23296] = "Оранжевый",
            [-8355712] = "Серый",
            [-16711936] = "Лайм",
            [-16181] = "Розовый"
        };
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {            
            return colorDict[((Color)value).ColorNumber];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null; 
        }
    }
}
