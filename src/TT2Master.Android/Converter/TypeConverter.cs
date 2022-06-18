using System;
using System.Globalization;

namespace TT2Master.Droid
{
    /// <summary>
    /// Provides simple conversion from <see cref="object"/> to specific base type like integer
    /// Secures that a valid value will be returned
    /// </summary>
    public static class TypeConverter
    {
        /// <summary>
        /// Forces conversion from object to int
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static int ForceInt(object o) => o == null ? 0 : !int.TryParse(o.ToString(), out int result) ? 0 : result;

        /// <summary>
        /// Forces conversion from object to long
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static long ForceLong(object o) => o == null ? 0 : !long.TryParse(o.ToString(), out long result) ? 0 : result;

        /// <summary>
        /// Forces conversion from object to DateTime
        /// if object is useless, then minimum DateTime Value is returned
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static DateTime ForceDate(object o) => o == null ? DateTime.MinValue : !DateTime.TryParse(o.ToString(), out DateTime result) ? DateTime.MinValue : result;

        /// <summary>
        /// Forces conversion from object to double
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static double ForceDoubleUniversal(object o)
        {
            if (o == null)
            {
                return 0;
            }

            var culture = o.ToString().Contains(",") ? CultureInfo.CreateSpecificCulture("de-DE") : CultureInfo.CreateSpecificCulture("en-US");

            return !Double.TryParse(o.ToString(), NumberStyles.Any, culture, out double result) ? 0 : result;
        }

        /// <summary>
        /// Forces standard conversion from object to double
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static double ForceDoubleStandard(object o) => o == null ? 0 : !double.TryParse(o.ToString(), out double result) ? 0 : result;
    }
}