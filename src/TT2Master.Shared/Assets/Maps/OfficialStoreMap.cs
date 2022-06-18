using CsvHelper.Configuration;
using TT2Master.Shared.Models;

namespace TT2Master.Shared.Assets.Maps
{
    public sealed class OfficialStoreMap : ClassMap<OfficialStore>
    {
        private readonly OfficialStore _me = new OfficialStore();

        public OfficialStoreMap()
        {
            Map(m => m.StoreIds).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.StoreIds)));
        }
    }
}
