using CsvHelper.Configuration;
using TT2Master.Shared.Models;

namespace TT2Master.Shared.Assets.Maps
{
    public sealed class RaidEnemyInfoMap : ClassMap<RaidEnemyInfo>
    {
        private readonly RaidEnemyInfo _me = new RaidEnemyInfo();

        public RaidEnemyInfoMap()
        {
            Map(m => m.EnemyId).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.EnemyId)));
            Map(m => m.Name).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.Name)));
            Map(m => m.Color).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.Color)));
            Map(m => m.EnchantColor).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.EnchantColor)));
            Map(m => m.BodyHeadMult).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BodyHeadMult)));
            Map(m => m.BodyTorsoMult).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BodyTorsoMult)));
            Map(m => m.BodyArmsMult).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BodyArmsMult)));
            Map(m => m.BodyLegsMult).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BodyLegsMult)));
            Map(m => m.ArmorHeadMult).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.ArmorHeadMult)));
            Map(m => m.ArmorTorsoMult).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.ArmorTorsoMult)));
            Map(m => m.ArmorArmsMult).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.ArmorArmsMult)));
            Map(m => m.ArmorLegsMult).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.ArmorLegsMult)));
            Map(m => m.TotalInArmour).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.TotalInArmour)));
            Map(m => m.TotalInBody).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.TotalInBody)));
            Map(m => m.BodyPhysicalMult).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BodyPhysicalMult)));
            Map(m => m.BodyElementalMult).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BodyElementalMult)));
            Map(m => m.BodyArcaneMult).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BodyArcaneMult)));
            Map(m => m.ArmorPhysicalMult).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.ArmorPhysicalMult)));
            Map(m => m.ArmorElementalMult).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.ArmorElementalMult)));
            Map(m => m.ArmorArcaneMult).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.ArmorArcaneMult)));
            Map(m => m.HPWeightBody).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.HPWeightBody)));
            Map(m => m.HPWeightArmor).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.HPWeightArmor)));
            Map(m => m.HPWeightHead).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.HPWeightHead)));
            Map(m => m.HPWeightChest).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.HPWeightChest)));
            Map(m => m.HPWeightArm).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.HPWeightArm)));
            Map(m => m.HPWeightLeg).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.HPWeightLeg)));
            Map(m => m.BonusTypeA).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BonusTypeA)));
            Map(m => m.BonusAmountA).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BonusAmountA)));
            Map(m => m.BonusTypeB).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BonusTypeB)));
            Map(m => m.BonusAmountB).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BonusAmountB)));
            Map(m => m.BonusTypeC).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BonusTypeC)));
            Map(m => m.BonusAmountC).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BonusAmountC)));
            Map(m => m.BonusTypeD).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BonusTypeD)));
            Map(m => m.BonusAmountD).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BonusAmountD)));
            Map(m => m.BonusTypeE).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BonusTypeE)));
            Map(m => m.BonusAmountE).Name(AssetMapNameProvider.GetMappingNames(nameof(_me.BonusAmountE)));
        }
    }
}
