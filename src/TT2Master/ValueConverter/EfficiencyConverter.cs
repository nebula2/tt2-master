using System;
using System.Globalization;

namespace TT2Master.ValueConverter
{
    /// <summary>
    /// Convert Efficiency to readable string
    /// </summary>
    public class EfficiencyConverter : BaseValueConverter<EfficiencyConverter>
    {
        /// <summary>
        /// Convert Artifact efficiency to readable string
        /// </summary>
        /// <param name="value">value to convert</param>
        /// <param name="targetType">not supported</param>
        /// <param name="parameter">not supported</param>
        /// <param name="culture">not supported</param>
        /// <returns></returns>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !double.TryParse(value.ToString(), out double result))
            {
                return "0";
            }

            if (result < 1000)
            {
                return $"{value:n2}";
            }
            else if (result < 1000000)
            {
                return $"{(result / 1000):n2}K";
            }
            else if (result < 1000000000)
            {
                return $"{(result / 1000000):n2}M";
            }

            return $"very high";
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
