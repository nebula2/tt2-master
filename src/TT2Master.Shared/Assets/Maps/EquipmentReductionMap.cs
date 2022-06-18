using CsvHelper.Configuration;
using TT2Master.Shared.Models;

namespace TT2Master.Shared.Assets.Maps
{
    public sealed class EquipmentReductionMap : ClassMap<EquipmentReduction>
    {
        private readonly EquipmentReduction _me = new EquipmentReduction();

        public EquipmentReductionMap()
        {
            Map(m => m.BoostId).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BoostId)));
            Map(m => m.ShipReduction).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.ShipReduction)));
            Map(m => m.TapReduction).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.TapReduction)));
            Map(m => m.PetReduction).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.PetReduction)));
            Map(m => m.ShadowCloneReduction).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.ShadowCloneReduction)));
            Map(m => m.HeavenlyStrikeReduction).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.HeavenlyStrikeReduction)));
            Map(m => m.ChestersonReduction).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.ChestersonReduction)));
            Map(m => m.FairyReduction).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.FairyReduction)));
            Map(m => m.AllGoldReduction).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.AllGoldReduction)));
            Map(m => m.PHoMReduction).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.PHoMReduction)));
            Map(m => m.BossGoldReduction).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BossGoldReduction)));
        }
    }
}
