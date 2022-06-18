using CsvHelper.Configuration;
using TT2Master.Shared.Models;

namespace TT2Master.Shared.Assets.Maps
{
    public sealed class ArtifactCostMap : ClassMap<ArtifactCost>
    {
        private readonly ArtifactCost _me = new ArtifactCost();

        public ArtifactCostMap()
        {
            Map(m => m.Count).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.Count)));
            Map(m => m.RelicCost).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.RelicCost)));
        }
    }
}
