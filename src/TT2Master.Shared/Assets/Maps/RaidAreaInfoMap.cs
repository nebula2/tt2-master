using CsvHelper.Configuration;
using TT2Master.Shared.Models;

namespace TT2Master.Shared.Assets.Maps
{
    public sealed class RaidAreaInfoMap : ClassMap<RaidAreaInfo>
    {
        private readonly RaidAreaInfo _me = new RaidAreaInfo();

        public RaidAreaInfoMap()
        {
            Map(m => m.AreaID).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.AreaID)));
            Map(m => m.BorderColor).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BorderColor)));
            Map(m => m.OverlayColor).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.OverlayColor)));
            Map(m => m.FogBackMin).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.FogBackMin)));
            Map(m => m.FogBackMax).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.FogBackMax)));
            Map(m => m.FogFrontMin).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.FogFrontMin)));
            Map(m => m.FogFrontMax).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.FogFrontMax)));
            Map(m => m.BonusTypeA).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BonusTypeA)));
            Map(m => m.BonusAmountA).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BonusAmountA)));
            Map(m => m.BonusTypeB).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BonusTypeB)));
            Map(m => m.BonusAmountB).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BonusAmountB)));
            Map(m => m.BonusTypeC).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BonusTypeC)));
            Map(m => m.BonusAmountC).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BonusAmountC)));
            Map(m => m.BonusTypeD).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BonusTypeD)));
            Map(m => m.BonusAmountD).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BonusAmountD)));
            Map(m => m.BonusTypeE).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BonusTypeE)));
            Map(m => m.BonusAmountE).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BonusAmountE)));
            Map(m => m.IconCount).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.IconCount)));
        }
    }
}
