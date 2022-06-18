using CsvHelper.Configuration;
using TT2Master.Shared.Models;

namespace TT2Master.Shared.Assets.Maps
{
    public sealed class SPSkillReductionMap : ClassMap<SPSkillReduction>
    {
        private readonly SPSkillReduction _me = new SPSkillReduction();

        public SPSkillReductionMap()
        {
            Map(m => m.SkillId).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.SkillId)));
            Map(m => m.ShipReduction).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.ShipReduction)));
            Map(m => m.PetReduction).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.PetReduction)));
            Map(m => m.ShadowCloneReduction).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.ShadowCloneReduction)));
            Map(m => m.HeavenlyStrikeReduction).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.HeavenlyStrikeReduction)));
            Map(m => m.ChestersonReduction).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.ChestersonReduction)));
            Map(m => m.FairyReduction).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.FairyReduction)));
            Map(m => m.AllGoldReduction).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.AllGoldReduction)));
            Map(m => m.PHoMReduction).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.PHoMReduction)));
            Map(m => m.BossGoldReduction).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BossGoldReduction)));
            Map(m => m.IsDmgRelevant).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.IsDmgRelevant)));
            Map(m => m.IsGoldRelevant).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.IsGoldRelevant)));
            Map(m => m.IsOnline).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.IsOnline)));
            Map(m => m.IsOffline).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.IsOffline)));
            Map(m => m.IsSecondaryRelevant).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.IsSecondaryRelevant)));
        }
    }
}
