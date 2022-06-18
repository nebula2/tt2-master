using CsvHelper;
using System.Linq;
using TT2Master.Shared.Assets;
using TT2Master.Shared.Assets.Maps;
using TT2Master.Shared.Models;
using TT2Master.Shared.Extensions;
using Xunit;

namespace TT2Master.Shared.Tests
{
    public class AssetHandlerTests
    {

        private void BuildAdditionalMappings()
        {
            var result = AssetHandler.GetMappedEntitiesFromCsvFile<AssetNameMapping, AssetNameMappingMap>("Resources\\Div\\assetNameMapping.csv");
            AssetMapNameProvider.AddAdditionalMappings(result);
        }

        [Fact]
        public void AssetHandler_ThrowsHeaderValidationException()
        {
            AssetMapNameProvider.ClearAdditionalMappings();
            Assert.Throws<HeaderValidationException>(() => AssetHandler.GetMappedEntitiesFromCsvFile<RaidLevelInfo, RaidLevelInfoMap>("Resources\\RaidLevelInfo.csv"));
        }

        [Fact]
        public void AssetHandler_ShouldLoadRaidInfofileAssets()
        {
            BuildAdditionalMappings();

            Assert.True(AssetHandler.GetMappedEntitiesFromCsvFile<RaidEnemyInfo, RaidEnemyInfoMap>("Resources\\RaidEnemyInfo.csv").Count > 0);
            Assert.True(AssetHandler.GetMappedEntitiesFromCsvFile<RaidAreaInfo, RaidAreaInfoMap>("Resources\\RaidAreaInfo.csv").Count > 0);
            Assert.True(AssetHandler.GetMappedEntitiesFromCsvFile<ClanTrait, ClanTraitMap>("Resources\\RaidClanInfo.csv", ClanTraitMap.GetSkipExpression(), true).Count > 0);
            Assert.True(AssetHandler.GetMappedEntitiesFromCsvFile<RaidCard, RaidCardMap>("Resources\\RaidSkillInfo.csv").Count > 0);

            var rli = AssetHandler.GetMappedEntitiesFromCsvFile<RaidLevelInfo, RaidLevelInfoMap>("Resources\\RaidLevelInfo.csv");
            Assert.True(rli.Count > 0);
            rli = rli.OrderBy(x => x.TierID).ThenBy(x => x.LevelID).ToList();
            Assert.Equal(3, rli.First().AttacksPerReset);
            Assert.Equal(5, rli.Last().AttacksPerReset);

            AssetMapNameProvider.ClearAdditionalMappings();
        }

        [Fact]
        public void AssetHandler_ShouldLoadArtifactInfofileAssets()
        {
            BuildAdditionalMappings();

            Assert.True(AssetHandler.GetMappedEntitiesFromCsvFile<Artifact, ArtifactMap>("Resources\\ArtifactInfo.csv").Count > 0);
            Assert.True(AssetHandler.GetMappedEntitiesFromCsvFile<ArtifactCost, ArtifactCostMap>("Resources\\ArtifactCostInfo.csv").Count > 0);

            

            AssetMapNameProvider.ClearAdditionalMappings();
        }

        [Fact]
        public void AssetHandler_ShouldLoadSkillInfofileAssets()
        {
            BuildAdditionalMappings();

            var skills = AssetHandler.GetMappedEntitiesFromCsvFile<Skill, SkillMap>("Resources\\SkillTreeInfo2.0.csv");
            Assert.True(skills.Count > 0);

            var fireSword = skills.Where(x => x.TalentID == "TapBoostMultiCastSkill").Single();

            Assert.Equal(1.0, fireSword.B[1]);
            Assert.Equal(2.0, fireSword.B[6]);
            Assert.Equal(3.0, fireSword.B[11]);

            Assert.True(AssetHandler.GetMappedEntitiesFromCsvFile<ActiveSkill, ActiveSkillMap>("Resources\\ActiveSkillInfo.csv").Count > 0);
            Assert.True(AssetHandler.GetMappedEntitiesFromCsvFile<PassiveSkill, PassiveSkillMap>("Resources\\PassiveSkillInfo.csv").Count > 0);

            AssetMapNameProvider.ClearAdditionalMappings();
        }

        [Fact]
        public void AssetHandler_ShouldLoadEquipInfofileAssets()
        {
            BuildAdditionalMappings();

            Assert.True(AssetHandler.GetMappedEntitiesFromCsvFile<Equipment, EquipmentMap>($"Resources\\{InfoFileEnum.EquipmentInfo.GetDescription()}.csv").Count > 0);
            Assert.True(AssetHandler.GetMappedEntitiesFromCsvFile<EquipmentSet, EquipmentSetMap>("Resources\\EquipmentSetInfo.csv").Count > 0);
            Assert.True(AssetHandler.GetMappedEntitiesFromCsvFile<EquipEnhanceCombo, EquipEnhanceComboMap>("Resources\\EquipmentEnhancementComboInfo.csv").Count > 0);
            Assert.True(AssetHandler.GetMappedEntitiesFromCsvFile<EquipEnhScalingInfo, EquipEnhScalingInfoMap>("Resources\\EquipmentEnhancementScalingInfo.csv").Count > 0);

            AssetMapNameProvider.ClearAdditionalMappings();
        }

        [Fact]
        public void AssetHandler_ShouldLoadPetInfofileAssets()
        {
            BuildAdditionalMappings();

            Assert.True(AssetHandler.GetMappedEntitiesFromCsvFile<Pet, PetMap>("Resources\\PetInfo.csv").Count > 0);

            AssetMapNameProvider.ClearAdditionalMappings();
        }

        [Fact]
        public void AssetHandler_ShouldLoadDivAssets()
        {
            BuildAdditionalMappings();

            Assert.True(AssetHandler.GetMappedEntitiesFromCsvFile<AssetNameMapping, AssetNameMappingMap>("Resources\\Div\\assetNameMapping.csv").Count > 0);
            Assert.True(AssetHandler.GetMappedEntitiesFromCsvFile<DefaultArtifactBuildEntry, DefaultArtifactBuildEntryMap>("Resources\\Div\\defaultBuilds.csv").Count > 0);
            Assert.True(AssetHandler.GetMappedEntitiesFromCsvFile<EquipmentReduction, EquipmentReductionMap>("Resources\\Div\\equipmentReductions.csv").Count > 0);
            Assert.True(AssetHandler.GetMappedEntitiesFromCsvFile<EquipmentSetMapping, EquipmentSetMappingMap>("Resources\\Div\\equipSetMapping.csv").Count > 0);
            Assert.True(AssetHandler.GetMappedEntitiesFromCsvFile<OfficialStore, OfficialStoreMap>("Resources\\Div\\officialStores.csv").Count > 0);
            Assert.True(AssetHandler.GetMappedEntitiesFromCsvFile<SPSkillReduction, SPSkillReductionMap>("Resources\\Div\\skillReductions.csv").Count > 0);

            AssetMapNameProvider.ClearAdditionalMappings();
        }

        [Fact]
        public void AssetHandler_HandlesIndexCorrectly()
        {
            BuildAdditionalMappings();

            var artifacts = AssetHandler.GetMappedEntitiesFromCsvFile<Artifact, ArtifactMap>("Resources\\ArtifactInfo.csv");
            Assert.True(artifacts.Count > 0);

            var firstArtifact = artifacts[0];
            var lastArtifact = artifacts.Last();

            Assert.Equal("Artifact22", firstArtifact.ID);
            // artifacts.csv got a header row and the reader acts 1 based. as we substract 1 from there the first entry should have a sort index of 1
            Assert.Equal(1, firstArtifact.SortIndex);
            // and the last index should be equal to the amount of items in the collection
            Assert.Equal(artifacts.Count, lastArtifact.SortIndex);

            var equips = AssetHandler.GetMappedEntitiesFromCsvFile<Equipment, EquipmentMap>($"Resources\\{InfoFileEnum.EquipmentInfo.GetDescription()}.csv");
            Assert.True(equips.Count > 0);
            Assert.Equal(1, equips[0].SortingIndex);
            Assert.Equal(equips.Count, equips.Last().SortingIndex);

        }
    }
}
