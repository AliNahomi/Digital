using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace Digital
{
    public class StockColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int cantidad)
            {
                if (cantidad < 5)
                    return Color.FromArgb("#FF4C4C"); // rojo crítico
                else if (cantidad < 10)
                    return Color.FromArgb("#FFA500"); // naranja
                else
                    return Color.FromArgb("#32CD32"); // verde
            }

            return Colors.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}


