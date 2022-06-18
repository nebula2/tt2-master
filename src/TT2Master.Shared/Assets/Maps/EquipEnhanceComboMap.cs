using CsvHelper.Configuration;
using TT2Master.Shared.Models;

namespace TT2Master.Shared.Assets.Maps
{
    public sealed class EquipEnhanceComboMap : ClassMap<EquipEnhanceCombo>
    {
        private readonly EquipEnhanceCombo _me = new EquipEnhanceCombo();

        public EquipEnhanceComboMap()
        {
            Map(m => m.Index).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.Index)));
            Map(m => m.Category).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.Category)));
            Map(m => m.Rare1).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.Rare1)));
            Map(m => m.Legendary1).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.Legendary1)));
            Map(m => m.Legendary2).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.Legendary2)));
            Map(m => m.Mythic1).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.Mythic1)));
            Map(m => m.Mythic2).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.Mythic2)));
            Map(m => m.Mythic3).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.Mythic3)));
            Map(m => m.R1).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.R1)));
            Map(m => m.R2).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.R2)));
            Map(m => m.R3).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.R3)));
            Map(m => m.L1).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.L1)));
            Map(m => m.L2).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.L2)));
            Map(m => m.L3).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.L3)));
            Map(m => m.M1).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.M1)));
            Map(m => m.M2).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.M2)));
            Map(m => m.M3).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.M3)));
        }
    }
}
