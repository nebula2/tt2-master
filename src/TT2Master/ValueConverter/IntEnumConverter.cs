using System;
using System.Globalization;

namespace TT2Master.ValueConverter
{
    /// <summary>
    /// Converts int to enum
    /// </summary>
    public class IntEnumConverter : BaseValueConverter<IntEnumConverter>
    {
        /// <summary>
        /// Test Ctor to check if Bindung works
        /// </summary>
        public IntEnumConverter()
        {

        }

        /// <summary>
        /// Convert int to enum
        /// </summary>
        /// <param name="value">value to convert</param>
        /// <param name="targetType">not supported</param>
        /// <param name="parameter">not supported</param>
        /// <param name="culture">not supported</param>
        /// <returns></returns>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value is Enum ? (int)value : (object)0;

        /// <summary>
        /// Convert int back to enum
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value is int ? Enum.ToObject(targetType, value) : 0;
    }
}
