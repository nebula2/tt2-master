using CsvHelper.Configuration;
using TT2Master.Shared.Models;

namespace TT2Master.Shared.Assets.Maps
{
    public sealed class AssetNameMappingMap : ClassMap<AssetNameMapping>
    {
        private readonly AssetNameMapping _me = new AssetNameMapping();

        public AssetNameMappingMap()
        {
            Map(m => m.PropertyName).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.PropertyName)));
            Map(m => m.HeaderName).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.HeaderName)));
        }
    }
}
