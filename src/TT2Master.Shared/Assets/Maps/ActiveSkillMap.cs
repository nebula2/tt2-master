using CsvHelper.Configuration;
using TT2Master.Shared.Models;

namespace TT2Master.Shared.Assets.Maps
{
    public sealed class ActiveSkillMap : ClassMap<ActiveSkill>
    {
        private readonly ActiveSkill _me = new ActiveSkill();
        private readonly InlineDoubleListConverter inlDoubleConv = new InlineDoubleListConverter();

        public ActiveSkillMap()
        {
            Map(m => m.ActiveSkillID).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.ActiveSkillID)));
            Map(m => m.BonusTypeA).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BonusTypeA)));
            Map(m => m.Amount).TypeConverter(inlDoubleConv).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.Amount)));
            Map(m => m.BonusTypeB).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BonusTypeB)));
            Map(m => m.SecondaryAmount).TypeConverter(inlDoubleConv).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.SecondaryAmount)));
            Map(m => m.Cost).TypeConverter(inlDoubleConv).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.Cost)));
            Map(m => m.Duration).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.Duration)));
            Map(m => m.Cooldown).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.Cooldown)));
            Map(m => m.UnlockLevel).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.UnlockLevel)));
            Map(m => m.DiamondCost).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.DiamondCost)));
            Map(m => m.ManaCost).TypeConverter(inlDoubleConv).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.ManaCost)));
            Map(m => m.RunWhileInactive).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.RunWhileInactive)));
            Map(m => m.SkillType).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.SkillType)));
        }
    }
}
