using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TT2Master.Helpers;
using TT2Master.Loggers;
using TT2Master.Model.Tournament;
using TT2Master.Resources;
using TT2Master.Shared.Models;

namespace TT2Master.Model.Arti
{
    /// <summary>
    /// Class to do the artifact optimization
    /// </summary>
    public class ArtifactOptimization
    {
        static readonly List<int> GoldArtifacts = new List<int>() { 19,1,44,45,79,95,2,20,23,43,66,82,53,97,9 };

        #region member
        public ArtOptSettings ArtOptSettings { get; set; } = new ArtOptSettings();

        public ArtifactBuild ArtBuild { get; set; } = new ArtifactBuild();

        private List<ArtifactToOptimize> _optimizeList = new List<ArtifactToOptimize>();

        private double _currentRelics;

        /// <summary>
        /// BoS-Percentage to calculate with
        /// </summary>
        private double _boSToCalculate;

        /// <summary>
        /// True if player is currently in a tournament
        /// False if not in a tournament
        /// </summary>
        private bool _isInTournament = false;

        private readonly bool _isNotificationAllowed = true;
        private readonly IPageDialogService _dialogService;
        private readonly SaveFile _save;
        #endregion

        #region private helper
        /// <summary>
        /// Checks if player is in a tournament and if so it sets the correct Bos-percentage
        /// </summary>
        private void CheckIfPlayerIsInTournament()
        {
            _isInTournament = TournamentHandler.IsPlayerInTournament();

            _boSToCalculate = _isInTournament ? ArtOptSettings.BosTourneyRoyalty : ArtOptSettings.BoSRoyalty;
            OnOptiLogMePlease?.Invoke("ArtifactHandler", new InformationEventArgs($"CheckIfPlayerIsInTournament: {_isInTournament}. Picked {_boSToCalculate}% for royalty calculation"));
        }

        /// <summary>
        /// Populates <see cref="_optimizeList"/> for Optimization
        /// only Artifacts with a level greater than 0 are added
        /// Also the ignoration stuff is checked before add
        /// </summary>
        private bool PopulateArtifactsToOptimize()
        {
            _optimizeList = new List<ArtifactToOptimize>();

            for (int i = 0; i < ArtifactHandler.Artifacts.Count; i++)
            {
                OnOptiLogMePlease?.Invoke(this, new InformationEventArgs($"PopulateArtifactsToOptimize: InitArtifacts: {ArtifactHandler.Artifacts[i].ID}"));

                bool ignored = ArtIgnored(ArtifactHandler.Artifacts[i]);

                //only add Artifacts that the user has
                if (ArtifactHandler.Artifacts[i].Level > 0 && !ignored)
                {
                    OnLogMePlease?.Invoke(this, new InformationEventArgs($"PopulateArtifactsToOptimize: InitArtifacts: Adding {ArtifactHandler.Artifacts[i].ID} to the list"));

                    var newArt = new ArtifactToOptimize(ArtifactHandler.Artifacts[i]);

                    //Set weight
                    WeightArtifact(newArt);

                    _optimizeList.Add(newArt);
                }
                else
                {
                    OnLogMePlease?.Invoke(this, new InformationEventArgs($"PopulateArtifactsToOptimize: InitArtifacts: Not adding {ArtifactHandler.Artifacts[i].ID} - {ArtifactHandler.Artifacts[i].Name}. Level is {ArtifactHandler.Artifacts[i].Level} and ignored is {ignored}"));
                }
            }

            return true;
        }

        /// <summary>
        /// Action for <see cref="CalculateCommand"/>
        /// </summary>
        /// <returns></returns>
        private async Task<ArtifactOptimizationResult> CalculateArtifactRanksAsync()
        {
            #region Setting things up
            OnLogMePlease?.Invoke(this, new InformationEventArgs($"CalculateArtifactRanksAsync: Saving settings. Start of run at {DateTime.Now}"));

            if (ArtBuild == null)
            {
                OnLogMePlease?.Invoke(this, new InformationEventArgs("CalculateArtifactRanksAsync: current build == null"));

                return new ArtifactOptimizationResult()
                {
                    OptimizedList = new List<ArtifactToOptimize>(),
                    Header = AppResources.InfoHeader,
                    Content = AppResources.SelectCurrentBuildPromptText,
                    IsMessageNeeded = true,
                };
            }

            //Set current Relics
            if (App.Save.CurrentRelics == 0)
            {
                OnOptiLogMePlease?.Invoke(this, new InformationEventArgs($"CalculateArtifactRanksAsync: App.Save.Relics is 0. Setting CurrentRelics to near LTR value..."));

                try
                {
                    _currentRelics = ArtifactHandler.GetLifeTimeSpentOnAll() + ArtifactCostHandler.CostSum(ArtifactHandler.Artifacts.Where(x => x.Level > 0).Count(), ArtifactHandler.Artifacts.Where(x => x.EnchantmentLevel > 0).Count());
                }
                catch (Exception e)
                {
                    OnLogMePlease?.Invoke(this, new InformationEventArgs($"CalculateArtifactRanksAsync: Could not set CurrentRelics because of an Exception: {e.Message}\n I try to set it to LifeTimeSpentOnAll"));
                    OnOptiLogMePlease?.Invoke(this, new InformationEventArgs($"CalculateArtifactRanksAsync: Could not set CurrentRelics because of an Exception: {e.Message}\n I try to set it to LifeTimeSpentOnAll"));


                    try
                    {
                        _currentRelics = ArtifactHandler.GetLifeTimeSpentOnAll();
                    }
                    catch (Exception ex)
                    {
                        OnOptiLogMePlease?.Invoke(this, new InformationEventArgs($"CalculateArtifactRanksAsync: That also failed: {ex.Message}"));

                        return new ArtifactOptimizationResult()
                        {
                            OptimizedList = new List<ArtifactToOptimize>(),
                            Header = AppResources.ErrorOccuredText,
                            Content = ex.Message,
                            IsMessageNeeded = true,
                        };
                    }
                }
            }
            else
            {
                _currentRelics = App.Save.CurrentRelics;
            }

            OnOptiLogMePlease?.Invoke(this, new InformationEventArgs($"CalculateArtifactRanksAsync: You have {_currentRelics} relics. Going to init Arts"));

            // Reload Arts for optimization to know which arts should be calculated
            PopulateArtifactsToOptimize();

            // Temporary list in which the optimization result is hold
            var temporaryList = new List<ArtifactToOptimize>();
            bool iHaveSomethingToDo = true;
            int inc = 1;
            #endregion

            #region Optimization
            // Optimize and take the best into the list
            while (iHaveSomethingToDo)
            {
                try
                {
                    // optimize
                    double availableRelics = GetAvailableRelics(ArtOptSettings.StepAmountId, _currentRelics);
                    OnOptiLogMePlease?.Invoke(this, new InformationEventArgs($"Calculating with {availableRelics} available relics to spent on this artifact"));

                    var cleanedList = GetOptimizedList(availableRelics);

                    if (cleanedList == null)
                    {
                        OnOptiLogMePlease?.Invoke(this, new InformationEventArgs($"CalculateArtifactRanksAsync: cleaned list is null. end"));
                        iHaveSomethingToDo = false;
                        continue;
                    }

                    // take the first
                    var winner = cleanedList.OrderByDescending(x => x.Efficiency).FirstOrDefault();

                    if (winner == null)
                    {
                        OnOptiLogMePlease?.Invoke(this, new InformationEventArgs($"CalculateArtifactRanksAsync: winner is null. end"));
                        iHaveSomethingToDo = false;
                        if (_isNotificationAllowed)
                        {
                            await ToastSender.SendToastAsync(AppResources.ErrorOccuredText, _dialogService);
                        }
                        continue;
                    }

                    winner.SetRank(inc);

                    // is it worth to process?
                    if (winner.Efficiency < ArtOptSettings.MinEfficiency)
                    {
                        OnOptiLogMePlease?.Invoke(this, new InformationEventArgs($"CalculateArtifactRanksAsync: I am done. winner efficiency is below MinEfficiency of {ArtOptSettings.MinEfficiency}: {winner.Efficiency}. Remaining Relics: {_currentRelics}. I set that dude to MAX"));
                        iHaveSomethingToDo = false;

                        // max out item before if there is one
                        if (temporaryList.Count > 0)
                        {
                            var winnerBefore = temporaryList[temporaryList.Count - 1];

                            winnerBefore.InPercent = 101;
                            winnerBefore.Amount = 9999;
                            winnerBefore.ClickAmount = 9999;
                        }

                        continue;
                    }

                    if (temporaryList.Count == ArtOptSettings.MaxArtifactAmount - 1)
                    {
                        OnOptiLogMePlease?.Invoke(this, new InformationEventArgs($"CalculateArtifactRanksAsync: I am done. reached max. amount of artifacts ({ArtOptSettings.MaxArtifactAmount}): {winner.Efficiency}. Remaining Relics: {_currentRelics}. I set that dude to MAX"));
                        winner.InPercent = 101;
                        winner.Amount = 9999;
                        winner.ClickAmount = 9999;
                        iHaveSomethingToDo = false;
                    }

                    // Check if winner is the same as the previous one. If so, sum it
                    if (temporaryList.Count > 0)
                    {
                        if (temporaryList[temporaryList.Count - 1].ID == winner.ID)
                        {
                            OnOptiLogMePlease?.Invoke(this, new InformationEventArgs($"CalculateArtifactRanksAsync: winner is same as before. adding them together"));
                            temporaryList[temporaryList.Count - 1].AddValuesFromOtherArtifact(winner);

                            // check if we are upgrading the same artifact over and over again (if this is the case you have the step amount way too low)
                            if (temporaryList[temporaryList.Count - 1].SubstitutionCount > 25)
                            {
                                OnOptiLogMePlease?.Invoke(this, new InformationEventArgs($"CalculateArtifactRanksAsync: winner is same as before for the 25th time. step amount is shitty"));

                                temporaryList[temporaryList.Count - 1].InPercent = 101;
                                temporaryList[temporaryList.Count - 1].Amount = 9999;
                                temporaryList[temporaryList.Count - 1].ClickAmount = 9999;
                                iHaveSomethingToDo = false;
                            }
                        }
                        else
                        {
                            temporaryList.Add((ArtifactToOptimize)winner.Clone());
                        }
                    }
                    else
                    {
                        temporaryList.Add((ArtifactToOptimize)winner.Clone());
                    }

                    OnOptiLogMePlease?.Invoke(this, new InformationEventArgs($"CalculateArtifactRanksAsync: adding {winner.Name} as winner for run {inc}"));


                    //set values for next opt run
                    var toInc = _optimizeList.Where(x => x.ID == winner.ID).FirstOrDefault();

                    OnOptiLogMePlease?.Invoke(this, new InformationEventArgs($"CalculateArtifactRanksAsync: CurrentRelics now: {_currentRelics}"));
                    _currentRelics -= winner.CostToLevel(winner.Amount);
                    OnOptiLogMePlease?.Invoke(this, new InformationEventArgs($"CalculateArtifactRanksAsync: CurrentRelics after: {_currentRelics}"));


                    toInc.SetNewValues();
                    toInc.SetRank(inc);
                    OnOptiLogMePlease?.Invoke(this, new InformationEventArgs($"CalculateArtifactRanksAsync: Increasing post calculation"));

                    //check if we have more to do - that is not the case if we can not affort to buy stuff
                    if (winner.InPercent >= 100 || _currentRelics <= 0)
                    {
                        OnOptiLogMePlease?.Invoke(this, new InformationEventArgs($"CalculateArtifactRanksAsync: I am done. winner percent: {winner.InPercent}. Remaining Relics: {_currentRelics}"));
                        iHaveSomethingToDo = false;
                    }

                    //increase counter
                    inc++;
                }
                catch (Exception e)
                {
                    OnOptiLogMePlease?.Invoke(this, new InformationEventArgs($"CalculateArtifactRanksAsync: Exception: {e.Message}"));
                }
            }

            // End
            _optimizeList = new List<ArtifactToOptimize>(temporaryList);
            #endregion

            #region Post optimization
            // Alert user when only one art is calculated
            if (_optimizeList.Count == 1 && _optimizeList.First().InPercent > 100)
            {
                if (_isNotificationAllowed)
                {
                    await ToastSender.SendToastAsync(_optimizeList[0].ID == "Artifact22"
                        ? AppResources.PushBoS
                        : inc == 1
                            ? AppResources.RelicsTooLow
                            : AppResources.CheckStepAmountOrEfficiency, _dialogService);
                }
            }
            else if (_optimizeList.Count == 0)
            {
                if (_isNotificationAllowed)
                {
                    await ToastSender.SendToastAsync(AppResources.RelicsTooLow, _dialogService);
                }
            }

            // End log if needed
            if (OptimizeLogger.WriteLog)
            {
                foreach (var item in _optimizeList)
                {
                    OnOptiLogMePlease?.Invoke(this, new InformationEventArgs($"Final List: {item.ID} - {item.Name}: {item.ClickAmount} Efficiency: {item.Efficiency}"));
                }
            }
            #endregion

            return new ArtifactOptimizationResult()
            {
                OptimizedList = _optimizeList,
                IsMessageNeeded = false,
                Header = "",
                Content = "",
            };
        }

        /// <summary>
        /// Does the optimization
        /// </summary>
        /// <returns></returns>
        private List<ArtifactToOptimize> GetOptimizedList(double availableRelics)
        {
            ArtifactHandler.RecalculateDivider();

            //for each artifact do the calculation
            for (int i = 0; i < _optimizeList.Count; i++)
            {
                var tmp = _optimizeList[i];
                tmp.Efficiency = 0;

                OnOptiLogMePlease?.Invoke(this, new InformationEventArgs($"ArtProps: {tmp.ToString()}"));

                #region Set CurrPercentage
                try
                {
                    //Calculate the amount of relics that has been spent on this artifact
                    double perc = ArtifactHandler.CalculateLifeTimeSpentPercentage(tmp.RelicsSpent);
                    tmp.CurrPercentage = Math.Round(perc, 2);

                    OnOptiLogMePlease?.Invoke(this, new InformationEventArgs($"GetOptimizedList: Artifact {tmp.ID} -> Calculated {tmp.CurrPercentage} as Percentage"));
                }
                catch (Exception)
                {
                    tmp.CurrPercentage = 1;
                    OnOptiLogMePlease?.Invoke(this, new InformationEventArgs($"GetOptimizedList: Could not load CurrPercentage for {tmp.Name}"));
                }
                #endregion

                tmp.Amount = GetLevelUpAmount(tmp, ArtOptSettings.StepAmountId, _boSToCalculate, availableRelics);

                double w = tmp.CostToLevel(tmp.Amount);

                tmp.InPercent = Math.Round(w * 100 / _currentRelics, 2, MidpointRounding.AwayFromZero);

                tmp.ClickAmount = CalculateClickAmount(tmp, ArtOptSettings.StepAmountId);
                OnOptiLogMePlease?.Invoke(this, new InformationEventArgs($"GetOptimizedList: Calculated Amount: {tmp.Amount}\tCost to level: {w}\t{tmp.InPercent}%\t{tmp.ClickAmount}"));

                //Calculate Efficiency
                tmp.Efficiency = GetEfficiency(tmp, _boSToCalculate);
                OnOptiLogMePlease?.Invoke(this, new InformationEventArgs($"GetOptimizedList: Calculated Efficiency: {tmp.Efficiency}"));
            }

            return _optimizeList.Where(x => x.Amount > 0 && x.Efficiency > 0).ToList();
        }
        #endregion

        #region Optimization
        #region Weighting
        /// <summary>
        /// Checks if an Artifact should be ignored. If yes, this returnes true
        /// </summary>
        /// <param name="art"></param>
        /// <returns></returns>
        private bool ArtIgnored(Artifact art)
        {
            //Ignore if Level is 0 or if you reached Max Level
            if (art.Level == 0 || (art.Level >= art.MaxLevel) && art.MaxLevel > 0)
            {
                OnOptiLogMePlease?.Invoke(this, new InformationEventArgs($"Artifact ignored because of Level: {art.Level} (max level: {art.MaxLevel})"));
                return true;
            }

            //manual ignoration
            if (ArtBuild.ArtsIgnored.Where(x => x.IsIgnored && x.ArtifactID == art.ID).Count() > 0)
            {
                OnOptiLogMePlease?.Invoke(this, new InformationEventArgs($"Artifact ignored because of manual igno in build"));
                return true;
            }

            switch (art.ID)
            {
                #region Gold Type
                case "Artifact19": //Chest of Contentment
                    return ArtBuild.GoldSource == GoldType.ChestersonGold || ArtBuild.GoldSource == GoldType.FairyGold ? false : true;

                case "Artifact1": //Heroic Shield
                    return ArtBuild.GoldSource == GoldType.BossGold || ArtBuild.GoldSource == GoldType.pHoM ? false : true;

                case "Artifact44": //Great Fay Medallion
                    return ArtBuild.GoldSource == GoldType.FairyGold ? false : true;

                case "Artifact45": // EX: (Coins of Ebizu. Now Neko Sculpture)
                    return ArtBuild.GoldSource == GoldType.pHoM ? false : true;

                case "Artifact79": // Coins of Ebizu
                    return ArtBuild.GoldSource == GoldType.ChestersonGold ? false : true;
                #endregion

                #region Damage Type
                case "Artifact32": //The Sword of Storms
                    return ((HeroDmgType)ArtOptSettings.HeroDamageInt == HeroDmgType.Melee || (HeroDmgType)ArtOptSettings.HeroDamageInt == HeroDmgType.NotSet) ? false : true;

                case "Artifact33": //Furies Bow
                    return ((HeroDmgType)ArtOptSettings.HeroDamageInt == HeroDmgType.Ranged || (HeroDmgType)ArtOptSettings.HeroDamageInt == HeroDmgType.NotSet) ? false : true;

                case "Artifact34": //Charm of the Ancient
                    return ((HeroDmgType)ArtOptSettings.HeroDamageInt == HeroDmgType.Spell || (HeroDmgType)ArtOptSettings.HeroDamageInt == HeroDmgType.NotSet) ? false : true;

                case "Artifact61": //Tiny Titan Tree
                    return ((HeroBaseType)ArtOptSettings.HeroBaseTypeInt == HeroBaseType.Ground || (HeroBaseType)ArtOptSettings.HeroBaseTypeInt == HeroBaseType.NotSet) ? false : true;

                case "Artifact62": //Helm of Hermes
                    return ((HeroBaseType)ArtOptSettings.HeroBaseTypeInt == HeroBaseType.Flying || (HeroBaseType)ArtOptSettings.HeroBaseTypeInt == HeroBaseType.NotSet) ? false : true;
                #endregion

                #region Raid Card based artifacts
                case "Artifact95" when _save?.ThisPlayer != null: // Charged Card
                    return _save.ThisPlayer.RaidTotalCardLevel == 0;

                case "Artifact94" when _save?.ThisPlayer != null: // Evergrowing Stack
                    return _save.ThisPlayer.RaidTotalCardLevel == 0;

                #endregion
                default:
                    return false;
            }
        }

        /// <summary>
        /// Increase artifact weight from hero choice
        /// </summary>
        /// <param name="art"></param>
        private void AdjustWeightFromHeroChoice(ArtifactToOptimize art)
        {
            if (art.NumericID < 86 || art.NumericID > 90)
            {
                return;
            }

            bool haveToInc = false;

            ArtifactWeight weight;
            weight = ArtBuild.CategoryWeights.Where(x => x.ArtifactId == art.ID).FirstOrDefault();

            string artToTakeWeightFrom = "";

            if (art.ID == "Artifact86"
                && (HeroDmgType)ArtOptSettings.HeroDamageInt == HeroDmgType.Melee)
            //Sword of the Royals (sword (Blade of Damocles:Artifact25) and melee (Sword of the storms:Artifact32))
            {
                haveToInc = true;
                artToTakeWeightFrom = "Artifact25";
            }
            if (art.ID == "Artifact87"
                && (HeroBaseType)ArtOptSettings.HeroBaseTypeInt == HeroBaseType.Flying)
            //Spearit's Vigil (helmet (Helmet of Madness:Artifact17) and flying (Helmet of Hermes:Artifact62))
            {
                haveToInc = true;
                artToTakeWeightFrom = "Artifact17";
            }
            if (art.ID == "Artifact88"
                && (HeroBaseType)ArtOptSettings.HeroBaseTypeInt == HeroBaseType.Ground)
            //The Cobalt Plate (armor (Titanium Plating:Artifact23) and ground (Tiny titan tree:Artifact61))
            {
                haveToInc = true;
                artToTakeWeightFrom = "Artifact23";
            }
            if (art.ID == "Artifact89"
                && (HeroDmgType)ArtOptSettings.HeroDamageInt == HeroDmgType.Spell)
            //Sigils of Judgement (aura (Moonlight bracelet:Artifact73) and spell (Charm of the ancient:Artifact34))
            {
                haveToInc = true;
                artToTakeWeightFrom = "Artifact73";
            }
            if (art.ID == "Artifact90"
                && (HeroDmgType)ArtOptSettings.HeroDamageInt == HeroDmgType.Ranged)
            //Foliage of the Keeper (slash (Amethyst staff:Artifact28) and ranged (Furies Bow:Artifact33))
            {
                haveToInc = true;
                artToTakeWeightFrom = "Artifact28";
            }

            if (haveToInc)
            {
                var weightToTake = ArtBuild.CategoryWeights.Where(x => x.ArtifactId == artToTakeWeightFrom).FirstOrDefault();

                if (weightToTake != null)
                {
                    art.ActiveRatio += weightToTake.Weight;
                }
            }
        }

        private double SetMaxHeroStuff(ArtifactToOptimize art)
        {
            if(ArtOptSettings.HasHerosMaxed && GoldArtifacts.Contains(art.NumericID))
            {
                OnOptiLogMePlease?.Invoke(this, new InformationEventArgs($"{art.ID} ({art.Name}) has no eff because HasHerosMaxed is set"));
                return 0;
            }

            return art.ActiveRatio;
        }


        /// <summary>
        /// Weights an <see cref="ArtifactToOptimize"/> for Optimization
        /// </summary>
        /// <param name="art"></param>
        private void WeightArtifact(ArtifactToOptimize art)
        {
            try
            {
                var weight = ArtBuild.CategoryWeights.Where(x => x.ArtifactId == art.ID).FirstOrDefault();

                //check if weight is set - else take default
                if (weight != null)
                {
                    art.ActiveRatio = weight.Weight;

                    //hero choice increasement
                    AdjustWeightFromHeroChoice(art);

                    art.ActiveRatio = SetMaxHeroStuff(art);
                }
                else
                {
                    art.ActiveRatio = 0.5;
                    OnOptiLogMePlease?.Invoke(this, new InformationEventArgs($"WeightArtifact: Could not load weight for {art.ID} - {art.Name}"));
                }
            }
            catch (Exception)
            {
                art.ActiveRatio = 0.5;
                OnOptiLogMePlease?.Invoke(this, new InformationEventArgs($"WeightArtifact: Could not load weight for {art.ID} - {art.Name}"));
            }

            OnOptiLogMePlease?.Invoke(this, new InformationEventArgs($"WeightArtifact: Weighting Artifact {art.ID} with {art.ActiveRatio}"));
        }
        #endregion

        /// <summary>
        /// Calculates the artifact effect for the current level
        /// </summary>
        /// <returns></returns>
        private double EffectAtLevel(Artifact art, double lvlIncrease = 0) => 1 + art.EffectPerLevel * Math.Pow(art.Level + (lvlIncrease > 0 ? lvlIncrease : 0), art.GrowthExpo);

        /// <summary>
        /// Calculates the Efficiency Rate for given Artifact
        /// Only called for BoS
        /// </summary>
        /// <returns></returns>
        private double EfficiencyRate(Artifact art, double royalty)
        {
            OnOptiLogMePlease?.Invoke(this, new InformationEventArgs($"EfficiencyRate (BoS): calculating {art.ID} with royalty: {royalty}"));
            double percentageSpentOnBoS = CalculateLifeTimeSpentPercentage(art.RelicsSpent);

            //Have I spent less percent of my all time Relics on BoS than I want? then decrease EfficiencyRate to get this up
            double result = percentageSpentOnBoS < royalty ? 999999999 : -999999999;

            OnOptiLogMePlease?.Invoke(this, new InformationEventArgs($"EfficiencyRate (BoS): {art.ID} percentageSpentOnBoS: {percentageSpentOnBoS}"));
            OnOptiLogMePlease?.Invoke(this, new InformationEventArgs($"EfficiencyRate (BoS): {art.ID} art.CurrentCostSum(): {art.CurrentCostSum()}"));
            OnOptiLogMePlease?.Invoke(this, new InformationEventArgs($"EfficiencyRate (BoS): {art.ID} art.Cost(): {art.Cost()}"));
            OnOptiLogMePlease?.Invoke(this, new InformationEventArgs($"EfficiencyRate(BoS): calculating {art.ID}: result = {result}"));

            return result;
        }

        /// <summary>
        /// Calculates percentage of LifetimeSpent
        /// </summary>
        /// <param name="relicsYouGot">relics you got</param>
        /// <returns>percentage of relicsYouGot from LifeTimeSpentOnAll</returns>
        private double CalculateLifeTimeSpentPercentage(double relicsYouGot)
        {
            if (relicsYouGot == 0)
            {
                OnOptiLogMePlease?.Invoke(this, new InformationEventArgs($"CalculateLifeTimeSpentPercentage: Relics you got is 0. returning"));

                return 0;
            }

            double result = relicsYouGot * 100 / GetLifeTimeSpentOnAll();

            OnOptiLogMePlease?.Invoke(this, new InformationEventArgs($"CalculateLifeTimeSpentPercentage: Calculated {result}"));

            return result;
        }

        /// <summary>
        /// Optimizer
        /// </summary>
        /// <returns></returns>
        private double ArtDmgEff(Artifact art, double lvlUpAmount = 0)
        {
            double heavenlySwordEffect = GetHeavenlySwordEffect();

            double adperc = ADBoostSum();

            double result = (art.ADBoost(lvlUpAmount) * heavenlySwordEffect + adperc) / adperc;

            return result;
        }

        /// <summary>
        /// Returns Effect from Heavenly Sword
        /// </summary>
        /// <returns></returns>
        private double GetHeavenlySwordEffect()
        {
            var artifact = _optimizeList.Where(x => x.ID == "Artifact26").FirstOrDefault(); // Heavenly Sword

            double heavenlySwordEffect = 1;

            if (artifact != null)
            {
                heavenlySwordEffect = EffectAtLevel(artifact);
            }

            return heavenlySwordEffect;
        }

        /// <summary>
        /// Muss für alle gemacht werden
        /// </summary>
        /// <returns></returns>
        private double ADBoostSum()
        {
            double result = 0;

            for (int i = 0; i < _optimizeList.Count; i++)
            {
                result += _optimizeList[i].ADBoost();
            }

            //multiplicate with effect of Heavenly Sword
            result *= GetHeavenlySwordEffect();

            //return at least 1
            return result < 1 ? 1 : result;
        }

        /// <summary>
        /// Returnes the sum of LifeTimeSpent from all Relics in sum
        /// </summary>
        /// <returns></returns>
        private double GetLifeTimeSpentOnAll()
        {
            double tmp = 0; //return val
            int i = 1; //counter for logger

            foreach (var item in _optimizeList)
            {
                tmp += item.RelicsSpent;

                i++;
            }

            OnOptiLogMePlease?.Invoke(this, new InformationEventArgs($"GetLifeTimeSpentOnAll: sum is {tmp}"));

            return tmp;
        }

        /// <summary>
        /// Amount of Level you need to pass royalty
        /// </summary>
        /// <returns></returns>
        private double BosLevelAim(Artifact art, double royalty)
        {
            //how much money did I spent on everything?
            double spent = GetLifeTimeSpentOnAll();
            //how much money should I have spent on BoS?
            double moneyIShouldHaveInvested = spent * royalty / 100;
            double whatLevelCouldIBe = art.LevelForMoney(moneyIShouldHaveInvested);

            OnOptiLogMePlease?.Invoke(this, new InformationEventArgs($"BosLevelAim: calculated spent with GetLifeTimeSpentOnAll: {spent}, moneyIShouldHaveInvested with: {moneyIShouldHaveInvested}, whatLevelCouldIBe with: {whatLevelCouldIBe}"));

            return whatLevelCouldIBe > 0 ? whatLevelCouldIBe : 0;
        }

        /// <summary>
        /// Calculates the efficiency for this artifact
        /// </summary>
        /// <returns></returns>
        private double GetEfficiency(ArtifactToOptimize art, double bosRoyalty)
        {
            //BoS Royalty
            if (art.ID == "Artifact22")
            {
                double rate = EfficiencyRate(art, bosRoyalty);
                OnOptiLogMePlease?.Invoke(this, new InformationEventArgs($"GetEfficiency: calculated {art.ID} - {art.Name} to ratio {rate} with royalty: {bosRoyalty}"));
                return rate;
            }

            double dadaAdRatio = ArtDmgEff(art, art.Amount);

            double effectAtLevel = EffectAtLevel(art);
            double effectNextLevel = EffectAtLevel(art, art.Amount);
            double effRatio = Math.Pow(effectNextLevel / effectAtLevel, art.ActiveRatio);

            // set upgrade reason
            art.UpgradeReason = dadaAdRatio > effRatio ? "AD" : "EFF";

            double finalRatio = dadaAdRatio * effRatio;
            OnOptiLogMePlease?.Invoke(this, new InformationEventArgs($"GetEfficiency: calculated {art.ID} - {art.Name} to ratio {finalRatio} with royalty: {bosRoyalty}"));
            return finalRatio;
        }

        private double GetAvailableRelics(int stepAmount, double remainingRelics)
        {
            var sa = ArtStepAmounts.StepAmounts.Where(x => x.ID == stepAmount).FirstOrDefault();
            return remainingRelics * sa.Value / 100;
        }

        /// <summary>
        /// Rounds the amount to fit with desired step amount
        /// </summary>
        /// <param name="art">Artifact to calculate</param>
        /// <param name="stepAmount">Ingame-Step amount</param>
        /// <param name="percentage">Lifetime%</param>
        /// <param name="bosRoyalty">BoS-Royalty</param>
        /// /// <param name="availableRelics">The relics that are left at this moment</param>
        /// <returns>Amount to level up the given Artifact</returns>
        private double GetLevelUpAmount(ArtifactToOptimize art, int stepAmount, double bosRoyalty, double availableRelics)
        {
            // BoS calculation
            double boSLevel = art.ID == "Artifact22" ? BosLevelAim(art, bosRoyalty) : 0;

            var sa = ArtStepAmounts.StepAmounts.Where(x => x.ID == stepAmount).FirstOrDefault();

            double amount = sa.IsInPercent ? art.LevelForMoney(availableRelics) : sa.Value;

            return boSLevel > 0 ? boSLevel : amount;

        }

        /// <summary>
        /// Gets the Click-Amount for the step amount
        /// </summary>
        /// <param name="art">artifact</param>
        /// <param name="stepAmountId">step amount id</param>
        /// <returns>the amount you have to click on the upgrade button ingame</returns>
        private int CalculateClickAmount(ArtifactToOptimize art, int stepAmountId)
        {
            int result = 0;
            var sa = ArtStepAmounts.StepAmounts.Where(x => x.ID == stepAmountId).FirstOrDefault();

            //how many times can I afford to click? If InPercent is less than the Step Amount value i should only do one - else InPercent / StepAmount
            //Calculate Upgrade Cost in Percentage of Current Relics
            result = sa.IsInPercent
                ? art.InPercent >= 100 ? 9999 : art.InPercent < sa.Value ? 1 : (int)Math.Ceiling(art.InPercent / sa.Value)
                : _save.RoundedUpgrade
                    ? GetRoundingAmount(sa, art)
                    : art.Amount < sa.Value
                        ? 1 
                        : (int)Math.Ceiling(art.Amount / sa.Value);

            return result;
        }

        /// <summary>
        /// Returns Click amount when rounding is enabled
        /// </summary>
        /// <param name="artStep"></param>
        /// <param name="artifact"></param>
        /// <returns></returns>
        private int GetRoundingAmount(ArtStepAmount artStep, ArtifactToOptimize artifact)
        {
            if(artStep.Value == 1)
            {
                return (int)Math.Ceiling(artifact.Amount);
            }

            double[] intervals = GetRoundingIntervals(artifact.Level, artifact.Amount, artStep.Value);

            return intervals.Length;
        }

        /// <summary>
        /// Returns an array for rounding shit
        /// </summary>
        /// <param name="startValue">start level</param>
        /// <param name="range">range until end</param>
        /// <param name="step">interval</param>
        /// <returns></returns>
        private static double[] GetRoundingIntervals(double startValue, double range, double step)
        {
            var vs = new List<double>();

            // do we currently have an unrounded value? then insert a step to normalize it
            double rest = startValue % step;
            double normalizedStart = rest != 0 ? startValue - rest + step : startValue;
            if (rest != 0)
            {
                vs.Add(normalizedStart);
            }

            // calculate normalized end point
            double restEnd = (startValue + range) % step;
            double normalizedEnd = startValue + range - restEnd;

            for (int i = (int)normalizedStart + (int)step; i <= normalizedEnd; i += (int)step)
            {
                vs.Add(i);
            }

            return vs.ToArray();
        }
        #endregion

        #region Ctor
        /// <summary>
        /// Constructs the ArtifactOptimization object
        /// </summary>
        /// <param name="settings">Artifact optimization settings</param>
        /// <param name="build">Artifact build</param>
        /// <param name="notificationAllowed">is this object allowed to send notifications to the user on it's own?</param>
        /// <param name="dialogService">dialog service to send pop up messages</param>
        public ArtifactOptimization(ArtOptSettings settings, ArtifactBuild build, SaveFile save, bool notificationAllowed, IPageDialogService dialogService = null)
        {
            ArtOptSettings = settings;
            ArtBuild = build;
            _save = save;
            _isNotificationAllowed = notificationAllowed;
            _dialogService = dialogService;
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Reloads Artifact Levels Async
        /// </summary>
        /// <returns></returns>
        public async Task<ArtifactOptimizationResult> GetOptimizedList()
        {
            #region Load and fill Artifacts
            SaveFile.OnError += Save_OnError;
            //Load Save file -> Only Arts
            if (!await App.Save.Initialize(loadPlayer: false, loadAccountModel: false, loadArtifacts: true, loadSkills: false, loadClan: false, loadTournament: true, loadEquipment: false))
            {
                return new ArtifactOptimizationResult()
                {
                    OptimizedList = new List<ArtifactToOptimize>(),
                    IsMessageNeeded = true,
                    Header = AppResources.ErrorOccuredText,
                    Content = AppResources.CouldNotLoadFileText,
                };
            }
            SaveFile.OnError -= Save_OnError;

            //Populate ArtifactConstants from which the current artifact data is taken
            ArtifactHandler.OnProblemHaving += ArtifactHandler_OnProblemHaving;

            if (!ArtifactHandler.LoadArtifacts())
            {
                ArtifactHandler.OnProblemHaving -= ArtifactHandler_OnProblemHaving;

                return new ArtifactOptimizationResult()
                {
                    OptimizedList = new List<ArtifactToOptimize>(),
                    IsMessageNeeded = true,
                    Header = AppResources.ErrorOccuredText,
                    Content = AppResources.CouldNotFillDataText,
                };
            }

            ArtifactHandler.FillArtifacts(_save);

            ArtifactHandler.OnProblemHaving -= ArtifactHandler_OnProblemHaving;
            #endregion

            //Check if in a tournament
            CheckIfPlayerIsInTournament();

            var optResult = CalculateArtifactRanksAsync();

            return await optResult;
        }
        #endregion

        #region E + D
        private void Save_OnError(object sender, CustErrorEventArgs e) => OnLogMePlease?.Invoke(this, new InformationEventArgs($"Save file error {sender.ToString()}: {e.MyException.Message}\n{e.MyException.Data}"));

        /// <summary>
        /// When ArtifactConstants crashes
        /// </summary>
        /// <param name="e"></param>
        private async void ArtifactHandler_OnProblemHaving(object sender, CustErrorEventArgs e)
        {
            await _dialogService?.DisplayAlertAsync(AppResources.ErrorHeader, $"{e.MyException.Message}", AppResources.OKText);
            OnLogMePlease?.Invoke(this, new InformationEventArgs($"ARTIFACT ERROR {sender.ToString()} -> {e.MyException.Message} \n{e.MyException.Data}"));
        }

        /// <summary>
        /// Delegate for <see cref="OnProgressMade"/>
        /// </summary>
        /// <param name="message"></param>
        public delegate void LogCarrier(object sender, InformationEventArgs e);

        /// <summary>
        /// Raised when <see cref="Initialize"/> has something to log
        /// </summary>
        public event LogCarrier OnLogMePlease;
        public event LogCarrier OnOptiLogMePlease;
        #endregion
    }
}