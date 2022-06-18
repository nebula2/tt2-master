using CsvHelper.Configuration;
using TT2Master.Shared.Models;

namespace TT2Master.Shared.Assets.Maps
{
    public sealed class RaidCardMap : ClassMap<RaidCard>
    {
        private readonly RaidCard _me = new RaidCard();

        public RaidCardMap()
        {
            Map(m => m.CardId).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.CardId)));
            Map(m => m.Name).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.Name)));
            Map(m => m.IsActive).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.IsActive)));
            Map(m => m.Note).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.Note)));
            Map(m => m.Category).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.Category)));
            Map(m => m.CardType).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.CardType)));
            Map(m => m.Tier).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.Tier)));
            Map(m => m.BestAgainst).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BestAgainst)));
            Map(m => m.MaxStacks).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.MaxStacks)));
            Map(m => m.Duration).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.Duration)));
            Map(m => m.Chance).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.Chance)));
            Map(m => m.SpatialLength).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.SpatialLength)));
            Map(m => m.BaseCooldown).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BaseCooldown)));
            Map(m => m.MaxLevel).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.MaxLevel)));
            Map(m => m.Color).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.Color)));
        }
    }
}
