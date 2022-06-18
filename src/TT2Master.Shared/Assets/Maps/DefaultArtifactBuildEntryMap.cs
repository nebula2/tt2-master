using CsvHelper.Configuration;
using TT2Master.Shared.Models;

namespace TT2Master.Shared.Assets.Maps
{
    public sealed class DefaultArtifactBuildEntryMap : ClassMap<DefaultArtifactBuildEntry>
    {
        private readonly DefaultArtifactBuildEntry _me = new DefaultArtifactBuildEntry();

        public DefaultArtifactBuildEntryMap()
        {
            Map(m => m.BuildId).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BuildId)));
            Map(m => m.ArtifactId).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.ArtifactId)));
            Map(m => m.DefaultCategory).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.DefaultCategory)));
        }
    }
}
