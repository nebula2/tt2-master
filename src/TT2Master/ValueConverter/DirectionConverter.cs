using System;
using System.Globalization;
using TT2Master.Model.Arti;

namespace TT2Master.ValueConverter
{
    /// <summary>
    /// if paremeter given it returns the opposite of the bool value
    /// </summary>
    public class DirectionConverter : BaseValueConverter<DirectionConverter>
    {
        /// <summary>
        /// Convert bool to bool -.- if parameter is passed then return !bool
        /// </summary>
        /// <param name="value">value to convert</param>
        /// <param name="targetType">not supported</param>
        /// <param name="parameter">reverse bool?</param>
        /// <param name="culture">not supported</param>
        /// <returns></returns>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value == null)
            {
                return null;
            }

            var mode = (ArtifactOptimizerDirectionMode)value;

            return mode == ArtifactOptimizerDirectionMode.Column ? Xamarin.Forms.FlexDirection.Column : Xamarin.Forms.FlexDirection.Row;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Convert(value, targetType, parameter, culture);
    }
}
