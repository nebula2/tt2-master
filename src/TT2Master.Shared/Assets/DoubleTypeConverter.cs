using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace TT2Master.Shared.Assets
{
    /// <summary>
    /// Type converter for doubles used by <see cref="AssetHandler"/>
    /// </summary>
    class DoubleTypeConverter : ITypeConverter
    {
        public object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData) => Helper.JfTypeConverter.ForceDoubleUniversal(text);

        public string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData) => Helper.JfTypeConverter.ForceDoubleUniversal(value).ToString();
    }
}
