using CsvHelper.Configuration;
using TT2Master.Shared.Models;

namespace TT2Master.Shared.Assets.Maps
{
    public sealed class ArtifactMap : ClassMap<Artifact>
    {
        private readonly Artifact _me = new Artifact();

        public ArtifactMap()
        {
            Map(m => m.ID).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.ID)));
            Map(m => m.SortIndex).Convert(row => row.Row.Context.Parser.RawRow -1);
            Map(m => m.InternalName).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.InternalName)));
            Map(m => m.GeneralTier).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.GeneralTier)));
            Map(m => m.Description).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.Description)));
            Map(m => m.MaxLevel).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.MaxLevel)));
            Map(m => m.Effect).Ignore();
            Map(m => m.EffectPerLevel).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.EffectPerLevel)));
            Map(m => m.GrowthExpo).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.GrowthExpo)));
            Map(m => m.DamageBonus).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.DamageBonus)));
            Map(m => m.CostCoefficient).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.CostCoefficient)));
            Map(m => m.CostExpo).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.CostExpo)));
            Map(m => m.DiscoveryPool).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.DiscoveryPool)));
            Map(m => m.EnchantmentPool).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.EnchantmentPool)));
            Map(m => m.EnchantmentMagnitude).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.EnchantmentMagnitude)));
            Map(m => m.EnchantmentMult).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.EnchantmentMult)));
            Map(m => m.BonusIcon).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BonusIcon)));
        }
    }
}
