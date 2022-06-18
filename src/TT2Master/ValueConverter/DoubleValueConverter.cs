using System;
using System.Globalization;
using TT2Master.Shared.Helper;

namespace TT2Master.ValueConverter
{
    /// <summary>
    /// Converts double to user friendly number
    /// </summary>
    public class DoubleValueConverter : BaseValueConverter<DoubleValueConverter>
    {
        /// <summary>
        /// Convert anything to double
        /// </summary>
        /// <param name="value">value to convert</param>
        /// <param name="targetType">not supported</param>
        /// <param name="parameter">not supported</param>
        /// <param name="culture">not supported</param>
        /// <returns>double</returns>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return 0;
            }
            else
            {
                return JfTypeConverter.ForceDoubleUniversal(
                    (value.ToString().EndsWith(".") || value.ToString().EndsWith(",")) 
                    ? value.ToString() + "0" 
                    : value.ToString());
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Convert(value, targetType, parameter, culture);
    }
}
