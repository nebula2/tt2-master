using CsvHelper.Configuration;
using TT2Master.Shared.Models;

namespace TT2Master.Shared.Assets.Maps
{
    public sealed class RaidLevelInfoMap : ClassMap<RaidLevelInfo>
    {
        private readonly RaidLevelInfo _me = new RaidLevelInfo();

        public RaidLevelInfoMap()
        {
            Map(m => m.TierID).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.TierID)));
            Map(m => m.LevelID).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.LevelID)));
            Map(m => m.AreaID).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.AreaID)));
            Map(m => m.EnemyIDs).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.EnemyIDs)));
            Map(m => m.TicketCost).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.TicketCost)));
            Map(m => m.TicketClanReward).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.TicketClanReward))).Optional();
            Map(m => m.XPClanReward).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.XPClanReward)));
            Map(m => m.XPPlayerReward).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.XPPlayerReward)));
            Map(m => m.DustPlayerReward).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.DustPlayerReward)));
            Map(m => m.CardPlayerReward).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.CardPlayerReward)));
            Map(m => m.ScrollPlayerReward).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.ScrollPlayerReward)));
            Map(m => m.FortuneScrollPlayerReward).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.FortuneScrollPlayerReward)));
            Map(m => m.HolidayCurrencyPerAttack).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.HolidayCurrencyPerAttack)));
            Map(m => m.AttacksPerReset).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.AttacksPerReset)));
            Map(m => m.BaseHP).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BaseHP)));
            Map(m => m.TitanCount).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.TitanCount)));
            Map(m => m.HasArmor).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.HasArmor)));
            Map(m => m.AreaBonuses).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.AreaBonuses)));
            Map(m => m.EnemyBonuses).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.EnemyBonuses)));
            Map(m => m.TotalHP).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.TotalHP)));
            Map(m => m.HPRatio).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.HPRatio)));
            Map(m => m.ClanXPCost).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.ClanXPCost)));
        }
    }
}
