using CsvHelper.Configuration;
using TT2Master.Shared.Models;

namespace TT2Master.Shared.Assets.Maps
{
    public sealed class EquipEnhScalingInfoMap : ClassMap<EquipEnhScalingInfo>
    {
        private readonly EquipEnhScalingInfo _me = new EquipEnhScalingInfo();

        public EquipEnhScalingInfoMap()
        {
            Map(m => m.BonusType).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BonusType)));
            Map(m => m.SpriteIndex).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.SpriteIndex)));
            Map(m => m.AttributeBase).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.AttributeBase)));
            Map(m => m.PowerBase).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.PowerBase)));
            Map(m => m.PowerInc).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.PowerInc)));
            Map(m => m.PowerExp).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.PowerExp)));
            Map(m => m.SpriteIndex).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.SpriteIndex)));
        }
    }
}
