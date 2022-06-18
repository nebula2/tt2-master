using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace TT2Master.Shared.Assets
{
    public class AssetHandler
    {
        /// <summary>
        /// Returns an IEnumerable of given type parsed from a csv file
        /// <para/> generic version of this thing here 
        /// <seealso href="https://joshclose.github.io/CsvHelper/examples/configuration/class-maps/mapping-by-name"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="csvFilePath">file path to read csv string from</param>
        /// <param name="flipRecordsBefore">if true, columns and rows will be flipped before parsing</param>
        /// <returns></returns>
        public static List<T> GetMappedEntitiesFromCsvFile<T, U>(string csvFilePath
            , Func<string[], bool> rowSkipExpression = null
            , bool flipRecordsBefore = false) where T : class where U : ClassMap
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            CsvWriter csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);
            StreamReader csvReader;

            if (flipRecordsBefore)
            {
                var flippedRecords = new List<dynamic>();

                var flippedConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = false,
                };

                using (StreamReader freader = new StreamReader(csvFilePath))
                using (CsvReader fcsv = new CsvReader(freader, flippedConfig))
                {
                    // Get the records from the CSV file.
                    var records = fcsv.GetRecords<dynamic>().ToList();

                    // Rotate the records into a new dynamic list.
                    var rows = new List<IDictionary<string, object>>();

                    foreach (var row in records)
                    {
                        rows.Add(row as IDictionary<string, object>);
                    }

                    for (int i = 2; i <= rows[0].Count; i++)
                    {
                        var flippedRecord = new ExpandoObject() as IDictionary<string, object>;

                        foreach (var row in rows)
                        {
                            flippedRecord.Add((string)row["Field1"], row["Field" + i]);
                        }

                        flippedRecords.Add(flippedRecord);
                    }
                }

                // Write the new list to memory
                csvWriter.WriteRecords(flippedRecords);
                writer.Flush();
                stream.Position = 0;
                csvReader = new StreamReader(stream);
            }
            else
            {
                csvReader = new StreamReader(csvFilePath);
            }

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = (header) => header.Header.ToLower(),
            };

            if (rowSkipExpression != null)
                config.ShouldSkipRecord = (args) => rowSkipExpression(args.Record);

            using CsvReader csv = new CsvReader(csvReader, config);
            
            csv.Context.TypeConverterCache.AddConverter<double>(new DoubleTypeConverter());
            csv.Context.TypeConverterCache.AddConverter<int>(new IntTypeConverter());
            csv.Context.RegisterClassMap<U>();

            
            var result = csv.GetRecords<T>().ToList();

            csvReader?.Dispose();
            csvWriter?.Dispose();
            writer?.Dispose();
            stream?.Dispose();

            return result;
        }

        public static List<T> GetMappedEntitiesFromCsvString<T, U>(string csvString
            , Func<string[], bool> rowSkipExpression = null
            ) where T : class where U : ClassMap
        {

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = (header) => header.Header.ToLower(),
            };

            if (rowSkipExpression != null)
                config.ShouldSkipRecord = (args) => rowSkipExpression(args.Record);

            using Stream stream = GenerateStreamFromString(csvString);
            using StreamReader csvReader = new StreamReader(stream);
            using CsvReader csv = new CsvReader(csvReader, config);

            csv.Context.TypeConverterCache.AddConverter<double>(new DoubleTypeConverter());
            csv.Context.TypeConverterCache.AddConverter<int>(new IntTypeConverter());
            csv.Context.RegisterClassMap<U>();

            var result = csv.GetRecords<T>().ToList();

            return result;
        }

        private static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
