using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System.Collections.Generic;

namespace TT2Master.Shared.Assets
{
    /// <summary>
    /// Type converter for doubles used by <see cref="AssetHandler"/>
    /// </summary>
    class InlineDoubleListConverter : ITypeConverter
    {
        public object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            var result = new List<double>();

            if (string.IsNullOrWhiteSpace(text)) return result;

            var entries = text.Split('/');

            foreach (var item in entries)
            {
                result.Add(Helper.JfTypeConverter.ForceDoubleUniversal(item));
            }

            return result;
        }

        public string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            if (value == null) return string.Empty;

            if (!(value is List<double> lst)) return string.Empty;

            var result = "";

            for (int i = 0; i < lst.Count; i++)
            {
                if (i == 0) 
                { 
                    result = lst[i].ToString(); 
                }
                else
                {
                    result += $"/{lst[i]}";
                }
            }

            return result;
        }
    }
}
