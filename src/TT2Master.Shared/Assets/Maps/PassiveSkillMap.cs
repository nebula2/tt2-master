using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Linq;
using System.Collections.Generic;
using TT2Master.Shared.Models;
using System.Text.RegularExpressions;

namespace TT2Master.Shared.Assets.Maps
{
    public sealed class PassiveSkillMap : ClassMap<PassiveSkill>
    {
        private readonly PassiveSkill _me = new PassiveSkill();

        public PassiveSkillMap()
        {
            Map(m => m.PassiveSkillId).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.PassiveSkillId)));
            Map(m => m.Name).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.Name)));
            Map(m => m.Note).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.Note)));
            Map(m => m.UpgradeType).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.UpgradeType)));
            Map(m => m.BonusType).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BonusType)));
            Map(m => m.TextSpriteIndex).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.TextSpriteIndex)));
            Map(m => m.MaxLevel).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.MaxLevel)));
            Map(m => m.CostValueDict).Convert((x) => GetCostValueDictFromRow(x.Row));
        }

        private static List<LevelCostValue> GetCostValueDictFromRow(IReaderRow row)
        {
            var result = new List<LevelCostValue>();

            var costs = row.HeaderRecord
                .Where(x => Regex.IsMatch(x, "C[0-9]{1,3}"))
                .Select(c => new
                {
                    Header = c.TrimStart('C'),
                    Value = row.GetField<double>(c)
                }).ToList();

            var values = row.HeaderRecord
                .Where(x => Regex.IsMatch(x, "A[0-9]{1,3}"))
                .Select(c => new
                {
                    Header = c.TrimStart('A'),
                    Value = row.GetField<double>(c)
                }).ToList();

            for (int i = 0; i < costs.Count; i++)
            {
                result.Add(new LevelCostValue
                {
                    Level = i + 1,
                    Cost = costs[i].Value,
                    Value = Helper.JfTypeConverter.ForceDoubleUniversal(values.Where(x => x.Header == costs[i].Header)?.FirstOrDefault()?.Value),
                });
            }
            
            return result;
        }
    }
}
