using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace TT2Master.ExtensionMethods
{
    public static class EnumerableExtensions
    {
        public static string ToCsv<T>(this IEnumerable<T> collection)
        {
            try
            {
                var stream = new MemoryStream();
                var writer = new StreamWriter(stream);
                var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
                csv.WriteRecords(collection);
                csv.Flush();
                stream.Position = 0;

                StreamReader reader = new StreamReader(stream);
                string text = reader.ReadToEnd();
                return text;
            }
            catch
            {
                return "";
            }
        }
    }
}
