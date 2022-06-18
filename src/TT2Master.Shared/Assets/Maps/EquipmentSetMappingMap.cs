using CsvHelper.Configuration;
using TT2Master.Shared.Models;

namespace TT2Master.Shared.Assets.Maps
{
    public sealed class EquipmentSetMappingMap : ClassMap<EquipmentSetMapping>
    {
        private readonly EquipmentSetMapping _me = new EquipmentSetMapping();

        public EquipmentSetMappingMap()
        {
            Map(m => m.InfoId).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.InfoId)));
            Map(m => m.ExportId).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.ExportId)));
        }
    }
}
