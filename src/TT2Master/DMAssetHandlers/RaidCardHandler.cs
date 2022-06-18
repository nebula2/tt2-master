using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using TT2Master.Helpers;
using TT2Master.Model.Assets;
using TT2Master.Shared;
using TT2Master.Shared.Assets.Maps;
using TT2Master.Shared.Helper;
using TT2Master.Shared.Models;

namespace TT2Master.Model.Clan
{
    /// <summary>
    /// Handles raid cards and decks (or sets - however you want to call it)
    /// </summary>
    public static class RaidCardHandler
    {
        /// <summary>
        /// List of card images that are available in this app
        /// </summary>
        public static List<string> CardImages { get; private set; } = new List<string>()
        {
            "MoonBeam",
            "Fragmentize",
            "SkullBash",
            "RazorWind",
            "WhipOfLightning",
            "BurstCount",
            "Purify",
            "LimbBurst",
            "BurningAttack",
            "PoisonAttack",
            "DecayingAttack",
            "Fuse",
            "Shadow",
            "PlagueAttack",
            "Disease",
            "ExecutionersAxe",
            "MentalFocus",
            "ImpactAttack",
            "InnerTruth",
            "FinisherAttack",
            "SuperheatMetal",
            "BurstBoost",
            "LimbSupport",
            "Swarm",
            "TotemFairySkill",
            "FlakShot",
            "Haymaker",
            "RuneAttack",
            "CrushingVoid",
        };

        /// <summary>
        /// List of available cards
        /// </summary>
        public static List<RaidCard> RaidCards { get; set; } = new List<RaidCard>();

        /// <summary>
        /// List of players sets
        /// </summary>
        public static List<RaidCardSet> RaidCardSets { get; set; } = new List<RaidCardSet>();

        /// <summary>
        /// Loads Raid cards from info file
        /// </summary>
        /// <returns></returns>
        public static bool LoadItemsFromInfofile()
        {
            try
            {
                OnLogMePlease?.Invoke("RaidCardHandler", new InformationEventArgs("RaidCardHandler.LoadItemsFromInfofile"));

                RaidCards = AssetReader.GetInfoFile<RaidCard, RaidCardMap>(InfoFileEnum.RaidSkillInfo);

                return true;
            }
            catch (Exception ex)
            {
                OnLogMePlease?.Invoke("RaidCardHandler", new InformationEventArgs($"RaidCardHandler: Exception at LoadPetsFromInfofile: {ex.Message}\n\n {ex.Data}\n\n"));
                OnProblemHaving?.Invoke("RaidCardHandler", new CustErrorEventArgs(ex));
                return false;
            }
        }

        /// <summary>
        /// Fills raid card information either from savefile or from clipboard
        /// </summary>
        /// <returns></returns>
        public static bool FillItems() => LocalSettingsORM.IsReadingDataFromSavefile
                ? FillItemsFromSavefile() && FillSetsFromSavefile()
                : FillItemsFromClipboard();

        /// <summary>
        /// Fills raid cards from clipboard
        /// </summary>
        /// <returns></returns>
        private static bool FillItemsFromClipboard()
        {
            OnLogMePlease?.Invoke("RaidCardHandler", new InformationEventArgs($"RaidCardHandler: FillItemsFromClipboard()"));

            if (App.Save.RaidCardModel == null)
            {
                OnLogMePlease?.Invoke("RaidCardHandler", new InformationEventArgs($"RaidCardHandler: FillItemsFromClipboard() raidcardmodel is null"));
                return false;
            }


            foreach (var token in App.Save.RaidCardModel)
            {
                try
                {
                    int index = RaidCards.FindIndex(x => x.Name.ToLower().Trim() == token.Key.ToLower().Trim());
                
                    if (index >= 0)
                    {
                        RaidCards[index].Level = JfTypeConverter.ForceInt((string)token.Value["level"]);
                        RaidCards[index].CardAmount = JfTypeConverter.ForceInt((string)token.Value["cards"]);
                    }
                }
                catch (Exception ex)
                {
                    OnLogMePlease?.Invoke("RaidCardHandler", new InformationEventArgs($"RaidCardHandler: FillItemsFromSavefile() -> Error {ex.Message}\n\n{ex.Data}"));
                    OnProblemHaving?.Invoke("RaidCardHandler", new CustErrorEventArgs(ex));
                    return false;
                }
            }

            return true;
        }
        /// <summary>
        /// Fills raid cards from save file
        /// </summary>
        /// <returns></returns>
        private static bool FillItemsFromSavefile()
        {
            OnLogMePlease?.Invoke("RaidCardHandler", new InformationEventArgs($"RaidCardHandler: FillItemsFromSavefile()"));

            var msgObj = App.Save.RaidCardModel["cards"];
            string msgString = msgObj.SelectToken("$content").ToString();
            var allItems = JArray.Parse(msgString);

            try
            {
                foreach (JObject token in allItems.Children())
                {
                    if (!token.TryGetValue("skill_name", out var val))
                    {
                        continue;
                    }

                    string cardId = token.GetValue("skill_name").SelectToken("$content").ToString();

                    var item = RaidCards.Where(x => x.CardId == cardId).FirstOrDefault();

                    if (item == null)
                    {
                        OnLogMePlease?.Invoke("RaidCardHandler", new InformationEventArgs($"RaidCardHandler: item is null"));
                        return false;
                    }

                    item.Level = JfTypeConverter.ForceInt((string)token.GetValue("level").SelectToken("$content").ToString());
                    int received = JfTypeConverter.ForceInt((string)token.GetValue("quantity_received").SelectToken("$content").ToString());
                    int spent = JfTypeConverter.ForceInt((string)token.GetValue("quantity_spent").SelectToken("$content").ToString());
                    item.CardAmount = received - spent;
                    OnLogMePlease?.Invoke("RaidCardHandler", new InformationEventArgs($"RaidCardHandler: filled level for item {item.CardId} with {item.Level}"));
                }
            }
            catch (Exception ex)
            {
                OnLogMePlease?.Invoke("RaidCardHandler", new InformationEventArgs($"RaidCardHandler: FillItemsFromSavefile() -> Error {ex.Message}\n\n{ex.Data}"));
                OnProblemHaving?.Invoke("RaidCardHandler", new CustErrorEventArgs(ex));
                return false;
            }

            return true;
        }

        /// <summary>
        /// Fills sets from save file
        /// </summary>
        /// <returns></returns>
        private static bool FillSetsFromSavefile()
        {
            OnLogMePlease?.Invoke("RaidCardHandler", new InformationEventArgs($"RaidCardHandler: FillSetsFromSavefile()"));
            RaidCardSets = new List<RaidCardSet>();
            var allItems = (JObject)App.Save.RaidCardModel["raidCardDecks"];

            try
            {
                foreach (var token in allItems)
                {
                    if (token.Key == "$type")
                    {
                        continue;
                    }

                    if(RaidCards.Count == 0)
                    {
                        OnLogMePlease?.Invoke("RaidCardHandler", new InformationEventArgs($"RaidCardHandler: I can not init sets without cards. Call FillItemsFromSavefile() first"));
                        return false;
                    }

                    string deckId = token.Key;

                    if (string.IsNullOrEmpty(token.Key))
                    {
                        OnLogMePlease?.Invoke("RaidCardHandler", new InformationEventArgs($"RaidCardHandler: token.Key is null"));
                        return false;
                    }

                    var set = new RaidCardSet
                    {
                        DeckId = token.Key
                    };

                    set.Slot0 = RaidCards.Where(x => x.CardId == (string)token.Value.SelectToken("slot0.$content")).FirstOrDefault();
                    if (set.Slot0 == null)
                    {
                        OnLogMePlease?.Invoke("RaidCardHandler", new InformationEventArgs($"RaidCardHandler: Slot0 is null"));
                    }
                    else
                    {
                        OnLogMePlease?.Invoke("RaidCardHandler", new InformationEventArgs($"RaidCardHandler: filled set {set.DeckId} with card {set.Slot0.Name}"));
                    }

                    set.Slot1 = RaidCards.Where(x => x.CardId == (string)token.Value.SelectToken("slot1.$content")).FirstOrDefault();
                    if (set.Slot1 == null)
                    {
                        OnLogMePlease?.Invoke("RaidCardHandler", new InformationEventArgs($"RaidCardHandler: Slot1 is null"));
                    }
                    else
                    {
                        OnLogMePlease?.Invoke("RaidCardHandler", new InformationEventArgs($"RaidCardHandler: filled set {set.DeckId} with card {set.Slot1.Name}"));
                    }

                    set.Slot2 = RaidCards.Where(x => x.CardId == (string)token.Value.SelectToken("slot2.$content")).FirstOrDefault();
                    if (set.Slot2 == null)
                    {
                        OnLogMePlease?.Invoke("RaidCardHandler", new InformationEventArgs($"RaidCardHandler: Slot2 is null"));
                    }
                    else
                    {
                        OnLogMePlease?.Invoke("RaidCardHandler", new InformationEventArgs($"RaidCardHandler: filled set {set.DeckId} with card {set.Slot2.Name}"));
                    }

                    RaidCardSets.Add(set);
                }
            }
            catch (Exception ex)
            {
                OnLogMePlease?.Invoke("RaidCardHandler", new InformationEventArgs($"RaidCardHandler: FillSetsFromSavefile() -> Error {ex.Message}\n\n{ex.Data}"));
                OnProblemHaving?.Invoke("RaidCardHandler", new CustErrorEventArgs(ex));
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns the image path for given card id ready to use in Xamarin.Forms UI
        /// </summary>
        /// <param name="cardId"></param>
        /// <returns></returns>
        public static string GetImagePathForCardId(string cardId) 
        {
            var item = CardImages.FirstOrDefault(x => x.ToLower().Trim() == cardId.ToLower().Trim());
            return item == null ? "notfound" : $"{cardId}";
        }

        /// <summary>
        /// Returns the image path for given card id ready to use in SkiaSharp
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetImagePathForDrawerId(string id) => CardImages.Contains(id) ? id : "notfound";

        #region events and delegates
        /// <summary>
        /// Delegate for <see cref="OnProgressMade"/> and <see cref="OnLogMePlease"/>
        /// </summary>
        /// <param name="message"></param>
        public delegate void ProgressCarrier(object sender, InformationEventArgs e);

        /// <summary>
        /// Raised when i think something should be logged
        /// </summary>
        public static event ProgressCarrier OnLogMePlease;

        /// <summary>
        /// Delegate for occuring problems
        /// </summary>
        /// <param name="data"></param>
        public delegate void HoustonWeGotAProblem(object sender, CustErrorEventArgs e);

        /// <summary>
        /// Fires when this instance gets trouble
        /// </summary>
        public static event HoustonWeGotAProblem OnProblemHaving;
        #endregion
    }
}
