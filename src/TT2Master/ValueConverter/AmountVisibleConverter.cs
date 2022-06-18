using System;
using System.Globalization;
using TT2Master.Shared.Helper;

namespace TT2Master.ValueConverter
{
    /// <summary>
    /// Convert Artifact percentage to readable string
    /// </summary>
    public class AmountVisibleConverter : BaseValueConverter<ArtStepConverter>
    {
        /// <summary>
        /// Convert Artifact percentage to readable string
        /// </summary>
        /// <param name="value">value to convert</param>
        /// <param name="targetType">not supported</param>
        /// <param name="parameter">not supported</param>
        /// <param name="culture">not supported</param>
        /// <returns></returns>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value == null 
            ? false 
            : JfTypeConverter.ForceInt(value) > 1 
                ? true 
                : false;

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
