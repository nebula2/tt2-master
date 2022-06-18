using CsvHelper.Configuration;
using TT2Master.Shared.Models;

namespace TT2Master.Shared.Assets.Maps
{
    public sealed class EquipmentMap : ClassMap<Equipment>
    {
        private readonly Equipment _me = new Equipment();

        public EquipmentMap()
        {
            Map(m => m.EquipmentId).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.EquipmentId)));
            Map(m => m.SortingIndex).Convert(row => row.Row.Context.Parser.RawRow - 1);
            Map(m => m.EquipmentCategory).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.EquipmentCategory)));
            Map(m => m.BonusType).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BonusType)));
            Map(m => m.Rarity).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.Rarity)));
            Map(m => m.AttributeBaseAmount).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.AttributeBaseAmount)));
            Map(m => m.AttributeBaseMult).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.AttributeBaseMult)));
            Map(m => m.AttributeBaseInc).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.AttributeBaseInc)));
            Map(m => m.AttributeExp1).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.AttributeExp1)));
            Map(m => m.AttributeExp2).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.AttributeExp2)));
            Map(m => m.AttributeExpBase).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.AttributeExpBase)));
            //Map(m => m.EquipmentSource).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.EquipmentSource)));
            Map(m => m.LimitedTime).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.LimitedTime)));
            Map(m => m.EquipmentSet).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.EquipmentSet)));
        }
    }
}
