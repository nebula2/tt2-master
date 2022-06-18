using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TT2Master.Model.Assets;
using TT2Master.Resources;
using TT2Master.Shared;
using TT2Master.Shared.Assets.Maps;
using TT2Master.Shared.Helper;
using TT2Master.Shared.Models;

namespace TT2Master
{
    public static class EquipmentHandler
    {
        #region Properties
        private static List<EquipmentSetMapping> _clipboardSetMapping = new List<EquipmentSetMapping>();

        /// <summary>
        /// All existing equipment from the InfoFile
        /// </summary>
        public static List<Equipment> ExistingEquipment { get; private set; } = new List<Equipment>();

        /// <summary>
        /// Owned equipment
        /// </summary>
        public static List<Equipment> OwnedEquipment { get; private set; } = new List<Equipment>();

        /// <summary>
        /// Equipment I currently have
        /// </summary>
        public static List<EquipEnhanceCombo> EnhanceCombos { get; private set; } = new List<EquipEnhanceCombo>();

        /// <summary>
        /// List of Reductions
        /// </summary>
        public static List<EquipmentReduction> EquipmentReductions { get; private set; } = new List<EquipmentReduction>();

        public static List<EquipEnhScalingInfo> EquipmentEnhancementScalingInfos { get; private set; } = new List<EquipEnhScalingInfo>();

        public static List<string> UnlockedEquipmentIds { get; private set; } = new List<string>();

        public static List<EquipmentSet> EquipmentSets { get; private set; } = new List<EquipmentSet>();
        #endregion

        #region private methods
        #region Load stuff from assets
        /// <summary>
        /// Loads existing equipment (all that you could possibly get)
        /// </summary>
        /// <param name="loadFromResource"></param>
        /// <returns></returns>
        private static bool LoadExistingEquipment()
        {
            try
            {
                OnLogMePlease?.Invoke("EquipmentHandler.LoadExistingEquipment");
                
                ExistingEquipment = AssetReader.GetInfoFile<Equipment, EquipmentMap>(InfoFileEnum.EquipmentInfo);
                OnLogMePlease?.Invoke($"EquipmentHandler: loaded {ExistingEquipment.Count} items");

                return true;
            }
            catch (Exception ex)
            {
                OnLogMePlease?.Invoke($"EquipmentHandler: Exception at LoadAsync: {ex.Message}\n\n {ex.Data}\n\n");
                OnProblemHaving?.Invoke(ex);
                return false;
            }
        }

        private static bool LoadEnhanceCombo()
        {
            try
            {
                OnLogMePlease?.Invoke("EquipmentHandler.LoadEnhanceCombo");

                EnhanceCombos = AssetReader.GetInfoFile<EquipEnhanceCombo, EquipEnhanceComboMap>(InfoFileEnum.EquipmentEnhancementComboInfo);

                OnLogMePlease?.Invoke($"EquipmentHandler.LoadEnhanceCombo loaded {EnhanceCombos.Count} items");

                return true;
            }
            catch (Exception ex)
            {
                OnLogMePlease?.Invoke($"EquipmentHandler: Exception at LoadAsync: {ex.Message}\n\n {ex.Data}\n\n");
                OnProblemHaving?.Invoke(ex);
                return false;
            }
        }

        private static bool LoadEnhancementScaling()
        {
            try
            {
                OnLogMePlease?.Invoke("EquipmentHandler.LoadEnhancementScaling");
                EquipmentEnhancementScalingInfos = AssetReader.GetInfoFile<EquipEnhScalingInfo, EquipEnhScalingInfoMap>(InfoFileEnum.EquipmentEnhancementScalingInfo);
                OnLogMePlease?.Invoke($"EquipmentHandler.LoadEnhancementScaling: Loaded {EquipmentEnhancementScalingInfos.Count} items");
                return true;
            }
            catch (Exception ex)
            {
                OnLogMePlease?.Invoke($"EquipmentHandler: Exception at LoadAsync: {ex.Message}\n\n {ex.Data}\n\n");
                OnProblemHaving?.Invoke(ex);
                return false;
            }
        }

        private static bool LoadReductions()
        {
            OnLogMePlease?.Invoke($"EquipmentHandler: Loading Reductions");

            EquipmentReductions = new List<EquipmentReduction>();

            try
            {
                EquipmentReductions = AssetReader.GetInfoFile<EquipmentReduction, EquipmentReductionMap>("equipmentReductions");

                OnLogMePlease?.Invoke($"EquipmentHandler: added {EquipmentReductions.Count} reductions.");
                
                return true;
            }
            catch (Exception ex)
            {
                OnLogMePlease?.Invoke($"could not read in equip reductions. {ex.Message}");
                return false;
            }
        }

        private static bool LoadSetInformationFromAssets()
        {
            UnlockedEquipmentIds = new List<string>();
            EquipmentSets = new List<EquipmentSet>();

            try
            {
                #region check that existing equipment is loaded
                if (ExistingEquipment.Count == 0)
                {
                    bool existingLoaded = LoadExistingEquipment();
                }
                #endregion

                #region Load in Set information
                // Get Info file
                EquipmentSets = AssetReader.GetInfoFile<EquipmentSet, EquipmentSetMap>(InfoFileEnum.EquipmentSetInfo);

                #endregion

                #region Load Mapping data
                OnLogMePlease?.Invoke($"EquipmentHandler.LoadSetInformationFromAssets: loading equipSetMapping");

                _clipboardSetMapping = AssetReader.GetInfoFile<EquipmentSetMapping, EquipmentSetMappingMap>("equipSetMapping");
                #endregion

                return true;
            }
            catch (Exception e)
            {
                OnLogMePlease?.Invoke($"EquipmentHandler.LoadSetInformationFromAssets Error: {e.Message}");
                return false;
            }
        }
        #endregion

        /// <summary>
        /// Gets enhancement combo value for secondary stat
        /// </summary>
        /// <param name="level">level of equipment</param>
        /// <param name="category">category of equipment</param>
        /// <param name="rarity">rarity of equipment</param>
        /// <param name="boostIndex">boostindex you want the value for. Eg 0 = R1, L1, M1</param>
        /// <returns>Enhancement Value from EquipEnhancementComboInfo</returns>
        private static double GetEnhanceComboValue(Equipment equipment, int boostIndex)
        {
            //Normal
            if ((equipment.Rarity == 1 && !equipment.LimitedTime) || string.IsNullOrEmpty(equipment.EquipmentCategory))
            {
                return 0;
            }

            try
            {
                int index = JfTypeConverter.ForceInt(equipment.Level.ToString("0.0", CultureInfo.InvariantCulture).Replace(',', '.').Split('.')[1]);

                var enh = EnhanceCombos.Where(x => x.Index == index && x.Category == equipment.EquipmentCategory).FirstOrDefault();

                return equipment.Rarity switch
                {
                    //Event
                    1 => boostIndex == 0 ? enh.R1 : 0,
                    //Rare
                    2 => boostIndex == 0 ? enh.R1 : 0,
                    //Legendary
                    3 => boostIndex == 0 ? enh.L1 : boostIndex == 1 ? enh.L2 : 0,
                    //Mythic
                    4 => boostIndex == 0 ? enh.M1 : boostIndex == 1 ? enh.M2 : enh.M3,
                    _ => 0,
                };
            }
            catch (Exception)
            {
                OnLogMePlease?.Invoke($"Could not get EnhanceComboValue for {equipment.Level}-{(equipment.EquipmentCategory ?? "")}-{equipment.Rarity}-{boostIndex}");
                return 0;
            }
        }

        /// <summary>
        /// Gets enhancement combo value for secondary stat
        /// </summary>
        /// <param name="level">level of equipment</param>
        /// <param name="category">category of equipment</param>
        /// <param name="rarity">rarity of equipment</param>
        /// <param name="boostIndex">boostindex you want the value for. Eg 0 = R1, L1, M1</param>
        /// <returns>Enhancement Value from EquipEnhancementComboInfo</returns>
        private static string GetEnhanceComboBoost(Equipment equipment, int boostIndex)
        {
            //Normal
            if ((equipment.Rarity == 1 && !equipment.LimitedTime) || string.IsNullOrEmpty(equipment.EquipmentCategory))
            {
                return "None";
            }

            try
            {
                int index = JfTypeConverter.ForceInt(equipment.Level.ToString("0.0", CultureInfo.InvariantCulture).Replace(',', '.').Split('.')[1]);

                var enh = EnhanceCombos.Where(x => x.Index == index && x.Category == equipment.EquipmentCategory).FirstOrDefault();

                return equipment.Rarity switch
                {
                    //Event
                    1 => boostIndex == 0 ? enh.Rare1 : "None",
                    //Rare
                    2 => boostIndex == 0 ? enh.Rare1 : "None",
                    //Legendary
                    3 => boostIndex == 0 ? enh.Legendary1 : boostIndex == 1 ? enh.Legendary2 : "None",
                    //Mythic
                    4 => boostIndex == 0 ? enh.Mythic1 : boostIndex == 1 ? enh.Mythic2 : enh.Mythic3,
                    _ => "None",
                };
            }
            catch (Exception)
            {
                OnLogMePlease?.Invoke($"Could not get EnhanceComboBoost for {equipment.Level}-{(equipment.EquipmentCategory ?? "")}-{equipment.Rarity}-{boostIndex}");
                return "None";
            }
        }

        #region fill data from data source
        private static void FillEquipFromSavefileData(SaveFile save)
        {
            var allEquip = (JObject)save.EquipmentModel["allEquipment"];

            OwnedEquipment.Clear();

            try
            {
                foreach (var token in allEquip)
                {
                    var eq = new Equipment();
                    string saveId = token.Key;

                    OnLogMePlease?.Invoke($"EquipmentHandler: filling {(saveId ?? "")}");

                    //check if we have equip
                    int checkVal = JfTypeConverter.ForceInt(saveId, 9999);
                    if (checkVal == 9999)
                    {
                        OnLogMePlease?.Invoke($"EquipmentHandler: {(saveId ?? "")} is not equip. continuing");

                        continue;
                    }

                    //Fill properties from Savefile
                    eq.EquipmentId = (string)token.Value.SelectToken("equipmentID.$content");
                    eq.LookId = (string)token.Value.SelectToken("lookID.$content");
                    eq.UniqueId = JfTypeConverter.ForceInt((string)token.Value.SelectToken("uniqueID.$content"));
                    eq.Equipped = JfTypeConverter.ForceBool((string)token.Value.SelectToken("equipped.$content"));
                    eq.Locked = JfTypeConverter.ForceBool((string)token.Value.SelectToken("locked.$content"));
                    eq.Hidden = JfTypeConverter.ForceBool((string)token.Value.SelectToken("hidden.$content"));
                    eq.Level = JfTypeConverter.ForceDoubleUniversal((string)token.Value.SelectToken("level.$content"));
                    eq.IsNew = JfTypeConverter.ForceBool((string)token.Value.SelectToken("isNew.$content"));

                    //Fill properties from ExistingEquipment if present
                    var eqSource = ExistingEquipment.Where(x => x.EquipmentId == eq.EquipmentId).FirstOrDefault();

                    if (eqSource == null)
                    {
                        OnLogMePlease?.Invoke("EquipmentHandler: could not find equip in source. continuing with next");
                        continue;
                    }

                    eq.SortingIndex = eqSource.SortingIndex;
                    eq.EquipmentCategory = eqSource.EquipmentCategory;
                    eq.BonusType = eqSource.BonusType;
                    eq.Rarity = eqSource.Rarity;
                    eq.AttributeBaseAmount = eqSource.AttributeBaseAmount;
                    eq.AttributeBaseInc = eqSource.AttributeBaseInc;
                    eq.AttributeExp1 = eqSource.AttributeExp1;
                    eq.AttributeExp2 = eqSource.AttributeExp2;
                    eq.AttributeExpBase = eqSource.AttributeExpBase;
                    //eq.EquipmentSource = eqSource.EquipmentSource;
                    eq.EquipmentSet = eqSource.EquipmentSet;
                    //eq.Scaling = eqSource.Scaling;
                    eq.LimitedTime = eqSource.LimitedTime;

                    //Fill properties from EnhanceCombo
                    eq.SecondaryBonus1 = GetEnhanceComboBoost(eq, 0);
                    eq.SecondaryBonusStat1 = GetEnhanceComboValue(eq, 0);

                    eq.SecondaryBonus2 = GetEnhanceComboBoost(eq, 1);
                    eq.SecondaryBonusStat2 = GetEnhanceComboValue(eq, 1);

                    eq.SecondaryBonus3 = GetEnhanceComboBoost(eq, 2);
                    eq.SecondaryBonusStat3 = GetEnhanceComboValue(eq, 2);

                    //Add Equipment to Collection
                    OwnedEquipment.Add(eq);
                }
            }
            catch (Exception ex)
            {
                OnLogMePlease?.Invoke($"EquipmentHandler: FillEquipment() -> Error {ex.Message}\n\n{ex.Data}");
                OnProblemHaving?.Invoke(ex);
            }
        }

        private static void FillEquipFromClipboardData()
        {
            // no data available in clipboard
        }

        private static void FillSetInformationFromSavefileData(SaveFile save)
        {
            #region Load unlocked ID's
            string allEquip = save.EquipmentModel == null ? null : save.EquipmentModel["allLookIDs"].ToString();

            if (allEquip == null)
            {
                return;
            }

            var equip = JObject.Parse(allEquip);
            var equipToken = equip.GetValue("$content");
            string[] equipArr = equipToken.Values<string>().ToArray();

            UnlockedEquipmentIds = equipArr.ToList();
            #endregion

            #region recognize completed sets
            //for each item in sets check if all parts are unlocked
            foreach (var item in EquipmentSets)
            {
                var equipNeeded = ExistingEquipment.Where(x => x.EquipmentSet == item.Set).ToList();

                foreach (var part in equipNeeded)
                {
                    //if any part is not unlocked, set completed to false and go on to the next set
                    if (!UnlockedEquipmentIds.Contains(part.EquipmentId))
                    {
                        item.Completed = false;
                        break;
                    }
                }
            }
            #endregion
        }

        /// <summary>
        ///  //TODO CHECK THIS !!!
        /// </summary>
        /// <param name="save"></param>
        private static void FillSetInformationFromClipboardData(SaveFile save)
        {
            #region Load unlocked ID's
            if (save.EquipmentClipboardModel == null)
            {
                return;
            }

            UnlockedEquipmentIds = save.EquipmentClipboardModel.Values<string>().ToList();
            #endregion

            #region recognize completed sets
            //for each item in sets check if all parts are unlocked
            foreach (var item in EquipmentSets)
            {
                // if you can not find a mapping continue and set it as not completed
                if (!_clipboardSetMapping.Any(x => x.InfoId == item.Set))
                {
                    item.Completed = false;
                    continue;
                }

                var mapping = _clipboardSetMapping.FirstOrDefault(x => x.InfoId == item.Set);

                item.Completed = UnlockedEquipmentIds.Contains(mapping.ExportId);
            }
            OnLogMePlease?.Invoke($"found {EquipmentSets.Where(x => x.Completed).Count()} completed sets");
            #endregion
        }
        #endregion
        #endregion

        #region Public methods
        /// <summary>
        /// Loads EquipmentInfo into <see cref="ExistingEquipment"/> and EquipmentEnhanceComboInfo into <see cref="EnhanceCombos"/>
        /// </summary>
        /// <param name="loadFromResource"></param>
        /// <returns></returns>
        public static bool Load()
        {
            bool equipLoaded = LoadExistingEquipment();
            bool enhanceLoaded = LoadEnhanceCombo();
            bool scalingLoaded = LoadEnhancementScaling();
            bool reductionsLoaded = LoadReductions();

            return equipLoaded && enhanceLoaded && reductionsLoaded && scalingLoaded;
        }

        /// <summary>
        /// Fills owned Equipment from Savefile
        /// </summary>
        public static void FillEquipment(SaveFile save)
        {
            OnLogMePlease?.Invoke($"EquipmentHandler: FillEquipment()");

            if (LocalSettingsORM.IsReadingDataFromSavefile)
            {
                FillEquipFromSavefileData(save);
            }
            else
            {
                FillEquipFromClipboardData();
            }
        }

        public static void FillEquipmentFromTestList(List<Equipment> testList)
        {
            OnLogMePlease?.Invoke($"EquipmentHandler: FillEquipmentFromTestList()");

            OwnedEquipment.Clear();

            try
            {
                foreach (var item in testList)
                {
                    var eq = new Equipment();
                    string saveId = item.UniqueId.ToString();

                    OnLogMePlease?.Invoke($"EquipmentHandler: filling {(saveId ?? "")}");

                    //check if we have equip
                    int checkVal = JfTypeConverter.ForceInt(saveId, 9999);
                    if (checkVal == 9999)
                    {
                        OnLogMePlease?.Invoke($"EquipmentHandler: {(saveId ?? "")} is not equip. continuing");

                        continue;
                    }

                    //Fill properties from Savefile
                    eq.EquipmentId = item.EquipmentId;
                    eq.LookId = item.LookId;
                    eq.UniqueId = item.UniqueId;
                    eq.Equipped = item.Equipped;
                    eq.Locked = item.Locked;
                    eq.Hidden = item.Hidden;
                    eq.Level = item.Level;
                    eq.IsNew = item.IsNew;

                    //Fill properties from ExistingEquipment if present
                    var eqSource = ExistingEquipment.Where(x => x.EquipmentId == eq.EquipmentId).FirstOrDefault();

                    if (eqSource == null)
                    {
                        OnLogMePlease?.Invoke("EquipmentHandler: could not find equip in source. continuing with next");
                        continue;
                    }

                    eq.SortingIndex = eqSource.SortingIndex;
                    eq.EquipmentCategory = eqSource.EquipmentCategory;
                    eq.BonusType = eqSource.BonusType;
                    eq.Rarity = eqSource.Rarity;
                    eq.AttributeBaseAmount = eqSource.AttributeBaseAmount;
                    eq.AttributeBaseInc = eqSource.AttributeBaseInc;
                    eq.AttributeExp1 = eqSource.AttributeExp1;
                    eq.AttributeExp2 = eqSource.AttributeExp2;
                    eq.AttributeExpBase = eqSource.AttributeExpBase;
                    //eq.EquipmentSource = eqSource.EquipmentSource;
                    eq.EquipmentSet = eqSource.EquipmentSet;
                    //eq.Scaling = eqSource.Scaling;
                    eq.LimitedTime = eqSource.LimitedTime;

                    //Fill properties from EnhanceCombo
                    eq.SecondaryBonus1 = GetEnhanceComboBoost(eq, 0);
                    eq.SecondaryBonusStat1 = GetEnhanceComboValue(eq, 0);

                    eq.SecondaryBonus2 = GetEnhanceComboBoost(eq, 1);
                    eq.SecondaryBonusStat2 = GetEnhanceComboValue(eq, 1);

                    eq.SecondaryBonus3 = GetEnhanceComboBoost(eq, 2);
                    eq.SecondaryBonusStat3 = GetEnhanceComboValue(eq, 2);

                    //Add Equipment to Collection
                    OwnedEquipment.Add(eq);
                }
            }
            catch (Exception ex)
            {
                OnLogMePlease?.Invoke($"EquipmentHandler: FillEquipment() -> Error {ex.Message}\n\n{ex.Data}");
                OnProblemHaving?.Invoke(ex);
            }
        }

        public static double GetEffWeight(string boostId, EquipAdvSettings settings)
        {
            if (EquipmentReductions == null)
            {
                return 1;
            }

            if (settings == null)
            {
                return 1;
            }

            var red = EquipmentReductions.Where(x => x.BoostId == boostId).FirstOrDefault();

            if (red == null)
            {
                return 1;
            }

            #region Get build Efficiency
            double buildEff = 1;

            switch (settings.CurrentBuild)
            {
                case EquipBuildEnum.Ship:
                    buildEff = red.ShipReduction;
                    break;
                case EquipBuildEnum.Tap:
                    buildEff = red.TapReduction;
                    break;
                case EquipBuildEnum.Pet:
                    buildEff = red.PetReduction;
                    break;
                case EquipBuildEnum.SC:
                    buildEff = red.ShadowCloneReduction;
                    break;
                case EquipBuildEnum.HS:
                    buildEff = red.HeavenlyStrikeReduction;
                    break;
                default:
                    break;
            }
            #endregion

            #region Get gold Efficiency
            double goldEff = 1;
            switch (settings.CurrentGoldType)
            {
                case GoldType.pHoM:
                    goldEff = red.PHoMReduction;
                    break;
                case GoldType.BossGold:
                    goldEff = red.BossGoldReduction;
                    break;
                case GoldType.ChestersonGold:
                    goldEff = red.ChestersonReduction;
                    break;
                case GoldType.NormalMobGold:
                    goldEff = red.AllGoldReduction;
                    break;
                case GoldType.FairyGold:
                    goldEff = red.FairyReduction;
                    break;
                default:
                    break;
            }
            #endregion

            #region Get settings efficiency multiplier
            double settingsEff = 1;

            switch (boostId)
            {
                #region Hero Dmg
                case "RangedHelperDamage":
                    if (settings.CurrentHeroDmgType != HeroDmgType.Ranged && settings.CurrentHeroDmgType != HeroDmgType.NotSet)
                    {
                        settingsEff = 0;
                    }
                    break;
                case "MeleeHelperDamage":
                    if (settings.CurrentHeroDmgType != HeroDmgType.Melee && settings.CurrentHeroDmgType != HeroDmgType.NotSet)
                    {
                        settingsEff = 0;
                    }
                    break;
                case "SpellHelperDamage":
                    if (settings.CurrentHeroDmgType != HeroDmgType.Spell && settings.CurrentHeroDmgType != HeroDmgType.NotSet)
                    {
                        settingsEff = 0;
                    }
                    break;
                #endregion

                #region Hero Range
                case "FlyingHelperDamage":
                    if (settings.CurrentHeroType == HeroBaseType.Ground)
                    {
                        settingsEff = 0;
                    }
                    break;

                case "GroundHelperDamage":
                    if (settings.CurrentHeroType == HeroBaseType.Flying)
                    {
                        settingsEff = 0;
                    }
                    break;
                #endregion
                default:
                    break;
            }
            #endregion

            return buildEff * settingsEff + goldEff * Reductions.GoldToDamageReduction;
        }

        /// <summary>
        /// Loads unlocked equipment ids and informations about sets
        /// </summary>
        public static bool LoadSetInformation(SaveFile save)
        {
            OnLogMePlease?.Invoke($"EquipmentHandler: LoadSetInformationAsync()");

            UnlockedEquipmentIds = new List<string>();
            EquipmentSets = new List<EquipmentSet>();

            try
            {
                LoadSetInformationFromAssets();

                if (LocalSettingsORM.IsReadingDataFromSavefile)
                {
                    FillEquipFromSavefileData(save);
                    FillSetInformationFromSavefileData(save);
                }
                else
                {
                    FillSetInformationFromClipboardData(save);
                }

                return true;
            }
            catch (Exception e)
            {
                OnLogMePlease?.Invoke($"EquipmentHandler Error: {e.Message}");
                return false;
            }
        }
        #endregion

        #region temp extension methods
        /// <summary>
        /// Sets <see cref="EfficiencyValue"/>.Call it when reductions are loaded
        /// </summary>
        public static void SetEfficiency(this Equipment equip, EquipAdvSettings settings)
        {
            equip.PrimaryBonusEff = Math.Pow(equip.PrimaryBonusEfficiency(), EquipmentHandler.GetEffWeight(equip.BonusType, settings));

            equip.SecBonusEff1 = Math.Pow(equip.CalculateEnhEfficiency(equip.SecondaryBonus1), EquipmentHandler.GetEffWeight(equip.SecondaryBonus1, settings));

            equip.SecBonusEff2 = Math.Pow(equip.CalculateEnhEfficiency(equip.SecondaryBonus2), EquipmentHandler.GetEffWeight(equip.SecondaryBonus2, settings));

            equip.SecBonusEff3 = Math.Pow(equip.CalculateEnhEfficiency(equip.SecondaryBonus3), EquipmentHandler.GetEffWeight(equip.SecondaryBonus3, settings));

            try
            {
                equip.EfficiencyValue =
                    (equip.PrimaryBonusEff < 1 ? 1 : equip.PrimaryBonusEff)
                    * (equip.SecBonusEff1 < 1 ? 1 : equip.SecBonusEff1)
                    * (equip.SecBonusEff2 < 1 ? 1 : equip.SecBonusEff2)
                    * (equip.SecBonusEff3 < 1 ? 1 : equip.SecBonusEff3);
            }
            catch (Exception)
            {
                equip.EfficiencyValue = 123456;
            }
        }

        /// <summary>
        /// Returns the build independent enhancement bonus efficiency
        /// </summary>
        /// <param name="boostId"></param>
        /// <returns></returns>
        public static double CalculateEnhEfficiency(this Equipment equip, string boostId)
        {
            if (boostId == "None")
            {
                return 1;
            }

            double result = 1;
            try
            {
                //get scaling object
                var es = EquipmentHandler.EquipmentEnhancementScalingInfos.Where(x => x.BonusType == boostId).FirstOrDefault();

                // couchy
                result = es.AttributeBase + Math.Pow((es.PowerBase + equip.Level * es.PowerInc), es.PowerExp);
            }
            catch (Exception)
            {
                result = 1;
            }

            return result;
        }

        /// <summary>
        /// Sets values for properties that are used for visualization
        /// </summary>
        public static void SetVisualProperties(this Equipment equip)
        {
            equip.RarityColor = equip.GetRarityColor();
            equip.EquippedColor = equip.GetEquippedColor();
            equip.LevelDisplay = $"Lv. {(equip.Level / 10):n0}";
            equip.EffDisplay = $"{equip.EfficiencyValue:n0}";
            equip.Name = equip.EquipmentId.TranslatedString();
            equip.BonusTypeDisplay = equip.BonusType.TranslatedString();
            equip.SecondaryBonus1Display = equip.SecondaryBonus1.TranslatedString();
            equip.SecondaryBonus2Display = equip.SecondaryBonus2.TranslatedString();
            equip.SecondaryBonus3Display = equip.SecondaryBonus3.TranslatedString();
        }
        #endregion

        #region events and delegates
        /// <summary>
        /// Delegate for <see cref="OnProgressMade"/> and <see cref="OnLogMePlease"/>
        /// </summary>
        /// <param name="message"></param>
        public delegate void ProgressCarrier(string message);

        /// <summary>
        /// Raised when i think something should be logged
        /// </summary>
        public static event ProgressCarrier OnLogMePlease;

        /// <summary>
        /// Delegate for occuring problems
        /// </summary>
        /// <param name="data"></param>
        public delegate void HoustonWeGotAProblem(Exception e);

        /// <summary>
        /// Fires when this instance gets trouble
        /// </summary>
        public static event HoustonWeGotAProblem OnProblemHaving;
        #endregion
    }
}