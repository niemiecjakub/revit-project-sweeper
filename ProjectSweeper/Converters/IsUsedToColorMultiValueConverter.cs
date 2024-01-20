using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace ProjectSweeper.Converters
{
    internal class IsUsedToColorMultiValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is bool CanBeDeleted && !CanBeDeleted)
            {
                return Brushes.Green;
            }
            if (values[1] is bool isUsed && isUsed)
            {
                return Brushes.Black;
            }
            return Brushes.Red;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
