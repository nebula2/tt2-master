using CsvHelper.Configuration;
using System;
using System.Linq;
using TT2Master.Shared.Models;

namespace TT2Master.Shared.Assets.Maps
{
    public sealed class ClanTraitMap : ClassMap<ClanTrait>
    {
        private readonly ClanTrait _me = new ClanTrait();

        public ClanTraitMap()
        {
            Map(m => m.ClanLevel).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.ClanLevel)));
            Map(m => m.ClanExp).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.ClanExp)));
            Map(m => m.AdvancedStart).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.AdvancedStart)));
        }

        /// <summary>
        /// Expression for row skipping.
        /// The flipped info file contains a row containing notes for the properties which should be skipped
        /// </summary>
        /// <returns></returns>
        public static Func<string[], bool> GetSkipExpression() => (string[] row) => 
            // do not skip header line
            !row[0].StartsWith("ClanTrait") 
            // skip rows where every column contains words
            && row.All(x => char.IsLetter(x.ToString().FirstOrDefault()));
    }
}
