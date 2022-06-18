using CsvHelper.Configuration;
using TT2Master.Shared.Models;

namespace TT2Master.Shared.Assets.Maps
{
    public sealed class PetMap : ClassMap<Pet>
    {
        private readonly Pet _me = new Pet();

        public PetMap()
        {
            Map(m => m.PetId).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.PetId)));
            Map(m => m.PetName).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.PetName)));
            Map(m => m.BonusType).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BonusType)));
            Map(m => m.PetType).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.PetType)));
        }
    }
}
