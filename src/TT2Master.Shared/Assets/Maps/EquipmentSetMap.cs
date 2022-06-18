using CsvHelper.Configuration;
using TT2Master.Shared.Models;

namespace TT2Master.Shared.Assets.Maps
{
    public sealed class EquipmentSetMap : ClassMap<EquipmentSet>
    {
        private readonly EquipmentSet _me = new EquipmentSet();

        public EquipmentSetMap()
        {
            Map(m => m.Set).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.Set)));
            Map(m => m.Enabled).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.Enabled)));
            Map(m => m.SetType).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.SetType)));
            Map(m => m.CraftCost1).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.CraftCost1)));
            Map(m => m.CraftCost2).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.CraftCost2)));
            Map(m => m.CraftCost3).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.CraftCost3)));
            Map(m => m.CraftCost4).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.CraftCost4)));
            Map(m => m.CraftCost5).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.CraftCost5)));
            Map(m => m.StageReq).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.StageReq)));
            Map(m => m.BonusType1).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BonusType1)));
            Map(m => m.Amount1).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.Amount1)));
            Map(m => m.Inc1).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.Inc1)));
            Map(m => m.Expo1).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.Expo1)));
            Map(m => m.BonusType2).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BonusType2)));
            Map(m => m.Amount2).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.Amount2)));
            Map(m => m.Inc2).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.Inc2)));
            Map(m => m.Expo2).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.Expo2)));
            Map(m => m.BonusType3).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BonusType3)));
            Map(m => m.Amount3).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.Amount3)));
            Map(m => m.Inc3).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.Inc3)));
            Map(m => m.Expo3).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.Expo3)));
        }
    }
}
