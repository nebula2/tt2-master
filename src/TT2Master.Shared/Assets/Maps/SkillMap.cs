using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TT2Master.Shared.Models;

namespace TT2Master.Shared.Assets.Maps
{
    public sealed class SkillMap : ClassMap<Skill>
    {
        private readonly Skill _me = new Skill();

        public SkillMap()
        {
            Map(m => m.TalentID).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.TalentID)));
            Map(m => m.Class).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.Class)));
            Map(m => m.Branch).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.Branch)));
            Map(m => m.Slot).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.Slot)));
            Map(m => m.SPReq).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.SPReq)));
            Map(m => m.TalentReq).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.TalentReq)));
            Map(m => m.TierNum).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.TierNum)));
            Map(m => m.Tier).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.Tier)));
            Map(m => m.Name).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.Name)));
            Map(m => m.Note).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.Note)));
            Map(m => m.MaxLevel).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.MaxLevel)));
            Map(m => m.S).Convert((x) => RowToListConverter<int>(x.Row, "S"));
            Map(m => m.Cost).Convert((x) => RowToListConverter<int>(x.Row, "Co"));
            Map(m => m.BonusTypeA).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BonusTypeA)));
            Map(m => m.A).Convert((x) => RowToListConverter<double>(x.Row, "A"));
            Map(m => m.BonusTypeB).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BonusTypeB)));
            Map(m => m.B).Convert((x) => RowToListConverter<double>(x.Row, "B"));
            Map(m => m.BonusTypeC).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BonusTypeC)));
            Map(m => m.C).Convert((x) => RowToListConverter<double>(x.Row, "C"));
            Map(m => m.BonusTypeD).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BonusTypeD)));
            Map(m => m.BonusAmountD).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BonusAmountD)));
        }

        private static List<T> RowToListConverter<T>(IReaderRow row, string s) =>
            row.HeaderRecord
                .Where(x => Regex.IsMatch(x, s + "[0-9]{1,3}"))
                .Select(c => row.GetField<T>(c)).ToList();
    }
}
