using System;
using System.Globalization;

namespace TT2Master.ValueConverter
{
    /// <summary>
    /// if paremeter given it returns the opposite of the bool value
    /// </summary>
    public class BoolConverter : BaseValueConverter<BoolConverter>
    {
        /// <summary>
        /// Convert bool to bool -.- if parameter is passed then return !bool
        /// </summary>
        /// <param name="value">value to convert</param>
        /// <param name="targetType">not supported</param>
        /// <param name="parameter">reverse bool?</param>
        /// <param name="culture">not supported</param>
        /// <returns></returns>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) => parameter == null ? value : !(bool)value;

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => !(bool)Convert(value, targetType, parameter, culture);
    }
}
