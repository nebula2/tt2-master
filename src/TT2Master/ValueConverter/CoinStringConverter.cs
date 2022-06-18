using System;
using System.Globalization;
using TT2Master.Shared.Helper;

namespace TT2Master.ValueConverter
{
    /// <summary>
    /// Converts double to user friendly number
    /// </summary>
    public class CoinStringConverter : BaseValueConverter<CoinStringConverter>
    {
        /// <summary>
        /// Convert double to scientific
        /// </summary>
        /// <param name="value">value to convert</param>
        /// <param name="targetType">not supported</param>
        /// <param name="parameter">not supported</param>
        /// <param name="culture">not supported</param>
        /// <returns></returns>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return 0;
            }

            //return string.Format("{0:#.###e+00}",(double)value);
            if (!double.TryParse(value.ToString(), out double result))
            {
                if(result < 1000)
                {
                    return $"{value:n2}";
                }
                if(result < 1000000)
                {
                    return $"{(result / 1000):n2}K";
                }

                return $"{value:n0}";
            };

            return JfTypeConverter.DoubleToTTNumber(result, LocalSettingsORM.GetConvertNumbersScientific());

        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
