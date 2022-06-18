using System.Collections.Generic;
using System.Linq;
using TT2Master.Shared.Models;

namespace TT2Master.Shared.Assets
{
    public static class AssetMapNameProvider
    {
        public static List<AssetNameMapping> AdditionalMappings { get; private set; } = new List<AssetNameMapping>();

        /// <summary>
        /// Returns an array of possible header names for given propertyName
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static string[] GetMappingNames(string propertyName)
        {
            List<string> result = new List<string>();

            if (AdditionalMappings.Count > 0)
            {
                result.AddRange(AdditionalMappings.Where(x => x.PropertyName == propertyName).Select(x => x.HeaderName).ToList());
            }

            result.Add(propertyName);

            return result.ToArray();
        }

        /// <summary>
        /// Add additional mappings as you need them
        /// </summary>
        /// <param name="valuePairs">Key = propertyName, Value = possible csv header</param>
        public static void AddAdditionalMappings(List<AssetNameMapping> mappings)
        {
            foreach (var item in mappings)
            {
                if (!AdditionalMappings.Contains(item))
                {
                    AdditionalMappings.Add(item);
                }
            }
        }

        public static void ClearAdditionalMappings() => AdditionalMappings = new List<AssetNameMapping>();
    }
}
