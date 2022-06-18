using System;
using System.Globalization;

namespace TT2Master.Shared.Helper
{
    /// <summary>
    /// Provides simple conversion from <see cref="object"/> to specific base type like integer
    /// Secures that a valid value will be returned
    /// </summary>
    public static class JfTypeConverter
    {
        /// <summary>
        /// converts a double to float value in relation to the max value a float can handle. 
        /// Meaning that for the passed double it will be checked how much it is filled to double.MaxVal.
        /// The returned float value has then the same percentual amount to the max value that the type can hold
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static float ConvertDoubleToFloatInRelation(double val) => (float)(val / float.MaxValue);

        /// <summary>
        /// Forces conversion from object to int
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static int ForceInt(object o, int errVal = 0) => o == null ? errVal : !int.TryParse(o.ToString(), out int result) ? errVal : result;

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

            CultureInfo culture = o.ToString().Contains(",") ? CultureInfo.CreateSpecificCulture("de-DE") : CultureInfo.CreateSpecificCulture("en-US");

            return !double.TryParse(o.ToString(), NumberStyles.Any, culture, out double result) ? 0 : result;
        }

        /// <summary>
        /// Forces conversion from object to bool
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool ForceBool(object o) => o != null && o.ToString().ToLower() == "true";
        public static bool ForceBoolFromInt(object o) => o != null && ForceInt(o) > 0;

        /// <summary>
        /// Forces standard conversion from object to double
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static double ForceDoubleStandard(object o, double errval = 0) => o == null ? errval : !double.TryParse(o.ToString(), out double result) ? errval : result;

        /// <summary>
        /// Converts Double to Number. Scientific or TT2 Number format depending on settings
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DoubleToTTNumber(double value, bool convertToScientific)
        {
            try
            {
                //Scientific
                if (convertToScientific)
                {
                    return ConvertDoubleToScientificString(value);
                }

                //TT2
                return value < 1000
                    ? value.ToString()
                    : value < 1e6
                    ? ShortenDouble(value, 1e3) + "K"
                    : value < 1e9
                    ? ShortenDouble(value, 1e6) + "M"
                    : value < 1e12
                    ? ShortenDouble(value, 1e9) + "B"
                    : value < 1e15 ? ShortenDouble(value, 1e12) + "T" : DoubleToAlphabeticString(value);
            }

            catch (Exception) { return value.ToString(); }
        }

        /// <summary>
        /// Converts double Value to Scientific notation string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ConvertDoubleToScientificString(double value) => value < 1000
            ? $"{value:n2}"
            : value < 1000000
                ? $"{value / 1000:n2}K"
                : string.Format("{0:#.###e+00}", value);

        #region private
        /// <summary>
        /// Divides and rounds a double value
        /// </summary>
        /// <param name="value">value to shorten</param>
        /// <param name="divider">amount to devide</param>
        /// <returns></returns>
        private static string ShortenDouble(double value, double divider) => Math.Round(value / divider, 2).ToString();

        /// <summary>
        /// Converts a double value to TT2 Number String
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string DoubleToAlphabeticString(double value)
        {
            string x = value.ToString("#.##E00");

            string[] tmp = x.Split('E');

            return ForceInt(tmp[1]) > 2042
                ? ConvertDoubleToScientificString(value)
                : MoveTTDoubleValueLeft(tmp) + GetCharsFromE(ForceInt(tmp[1]));
        }

        /// <summary>
        /// Moves the comma in scientific notation value to the correct place and returnes the number in front of e.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string MoveTTDoubleValueLeft(string[] value)
        {
            double valToMove = ForceDoubleUniversal(value[0]);

            int stringIndexer = ForceInt(value[1]);

            int moveAmount = stringIndexer < 93 ? (stringIndexer - 15) % 3 : ((stringIndexer - 15) - 78 * ((stringIndexer - 15) / 78)) % 3;

            return (valToMove * ForceDoubleUniversal($"1e{moveAmount}")).ToString();
        }

        /// <summary>
        /// returns TT2 Number Post-String (aa to zz)
        /// </summary>
        /// <param name="value">e-value. eg: 1.54E6 - pass 6</param>
        /// <returns></returns>
        private static string GetCharsFromE(int value)
        {
            //TODO third letter (aaa) starts at 1e2043

            int moveConstant = 15; //aa starts at e15
            int asciiValOfa = 97; //ascii-value of a
            int firstCharIncreaseInterval = 78; //every 78 e the first letter increases
            //int thirdLetterStartValue = 26; //third letter (aaa) starts at 1e2043 (2043-15) / 78 = 26

            //get normalized starting point
            int normalizedVal = value - moveConstant;

            //index of first char in a table, where 'a' is zero and 'z' is 24
            int firstIndex = normalizedVal / firstCharIncreaseInterval;

            //get first char of string
            char first = (char)(firstIndex + asciiValOfa);

            //get the remaining chars from value to calculate second char in string
            int remainder = normalizedVal - firstCharIncreaseInterval * firstIndex;

            //calculate second char index
            int secIndex = (remainder / 3);

            //secIndex has to be at least zero
            if (secIndex < 0)
            {
                secIndex = 0;
            }

            //get second char in string
            char second = (char)(secIndex + asciiValOfa);

            return first.ToString() + second.ToString();
        }
        #endregion
    }
}
