using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;
using System.Threading;

namespace TT2Master
{
    public static class StringExtensions
    {
        const string _resourceId = "TT2Master.Resources.AppResources";

        static readonly Lazy<ResourceManager> _resmgr =
            new Lazy<ResourceManager>(()
            => new ResourceManager(_resourceId, typeof(StringExtensions).GetTypeInfo().Assembly));

        /// <summary>
        /// This SplitCamelCase method will take a string that is camel cased and split it out into separate words 
        /// such as "ThisIsMyValue".SplitCamelCase(); // "This Is My Value"
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string SplitCamelCase(this string str)
        {
            return Regex.Replace(Regex.Replace(str, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
        }

        public static string TranslatedString(this string str)
        {
            if (str == null)
            {
                return "";
            }
            try
            {
                var currentLanguage = LocalSettingsORM.GetCurrentLanguage();
                var ci = CultureInfo.GetCultureInfo(currentLanguage);

                string translation = _resmgr.Value.GetString(str, ci);

                if (translation == null)
                {
                    translation = str; // returns the key, which GETS DISPLAYED TO THE USER
                }
                return translation;
            }
            catch (Exception)
            {
                return str;
            }
        }
    }
}