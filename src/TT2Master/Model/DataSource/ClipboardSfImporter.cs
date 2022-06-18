using Microsoft.AppCenter.Analytics;
using Newtonsoft.Json.Linq;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TT2Master.Loggers;
using TT2Master.Resources;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TT2Master.Model.DataSource
{
    public class ClipboardSfImporter
    {
        private IPageDialogService _dialogService;

        /// <summary>
        /// Big Ad
        /// </summary>
        private IAdInterstitial _adInterstitial;

        public ClipboardSfImporter(IPageDialogService dialogService)
        {
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _adInterstitial = Xamarin.Forms.DependencyService.Get<IAdInterstitial>();
        }

        /// <summary>
        /// Imports savefile clipboard data
        /// </summary>
        /// <param name="silent"></param>
        /// <returns></returns>
        public async Task<ClipboardSfImporterResult> ImportSfFromClipboardAsync(bool silentMode)
        {
            // get clipboard content
            var text = await Clipboard.GetTextAsync();

            #region checks
            // check if it is not empty and convertable into a jcontainer
#if DEBUG
            if (string.IsNullOrWhiteSpace(text))
            {
                text = "{\"playerStats\": {\"Max Prestige Stage\": \"100054\",\"Artifacts Collected\": \"97\",\"Crafting Power\": \"25\",\"Total Pet Levels\": \"11818\",\"Skill Points Owned\": \"3010\",\"Hero Weapon Upgrades\": \"2189\",\"Hero Scroll Upgrades\": \"1642\",\"Tournaments Joined\": \"289\",\"Undisputed Wins\": \"36\",\"Tournament Points\": \"34705\",\"Lifetime Relics\": \"3.859E+154\"},\"raidStats\": {\"Raid Level\": \"196\",\"Raid Damage\": \"295\",\"Total Raid Experience\": \"113399\",\"Total Raid Attacks\": \"2671\",\"Total Raid Card Levels\": \"529\",\"Raid Cards Owned\": \"27\",\"Lifetime Raid Tickets\": \"2994\"},\"artifacts\": {\"Book of Shadows\": {\"enchanted\": \"True\",\"level\": \"2.057E+44\"},\"Charged Card\": {\"enchanted\": \"True\",\"level\": \"3.765E+54\"},\"Stone of the Valrunes\": {\"enchanted\": \"True\",\"level\": \"6.675E+54\"},\"Chest of Contentment\": {\"enchanted\": \"True\",\"level\": \"1.080E+51\"},\"Heroic Shield\": {\"enchanted\": \"True\",\"level\": \"3.751E+54\"},\"Book of Prophecy\": {\"enchanted\": \"True\",\"level\": \"6.213E+47\"},\"Khrysos Bowl\": {\"enchanted\": \"False\",\"level\": \"4.438E+54\"},\"Zakynthos Coin\": {\"enchanted\": \"False\",\"level\": \"1.485E+51\"},\"Great Fay Medallion\": {\"enchanted\": \"False\",\"level\": \"1.057E+51\"},\"Neko Sculpture\": {\"enchanted\": \"False\",\"level\": \"4.947E+54\"},\"Coins of Ebizu\": {\"enchanted\": \"False\",\"level\": \"1.128E+51\"},\"The Bronzed Compass\": {\"enchanted\": \"False\",\"level\": \"4.141E+54\"},\"Evergrowing Stack\": {\"enchanted\": \"True\",\"level\": \"6.379E+47\"},\"Flute of the Soloist\": {\"enchanted\": \"False\",\"level\": \"9.655E+47\"},\"Heavenly Sword\": {\"enchanted\": \"True\",\"level\": \"6.366E+47\"},\"Divine Retribution\": {\"enchanted\": \"True\",\"level\": \"8.846E+50\"},\"Drunken Hammer\": {\"enchanted\": \"True\",\"level\": \"1.012E+52\"},\"Samosek Sword\": {\"enchanted\": \"False\",\"level\": \"4.543E+47\"},\"The Retaliator\": {\"enchanted\": \"False\",\"level\": \"6.423E+56\"},\"Stryfe's Peace\": {\"enchanted\": \"False\",\"level\": \"7.744E+50\"},\"Hero's Blade\": {\"enchanted\": \"False\",\"level\": \"5.714E+56\"},\"The Sword of Storms\": {\"enchanted\": \"False\",\"level\": \"3.227E+56\"},\"Furies Bow\": {\"enchanted\": \"False\",\"level\": \"3.784E+56\"},\"Charm of the Ancient\": {\"enchanted\": \"False\",\"level\": \"5.198E+55\"},\"Tiny Titan Tree\": {\"enchanted\": \"False\",\"level\": \"5.263E+56\"},\"Helm of Hermes\": {\"enchanted\": \"False\",\"level\": \"5.618E+55\"},\"Fruit of Eden\": {\"enchanted\": \"False\",\"level\": \"6.677E+46\"},\"Influential Elixir\": {\"enchanted\": \"False\",\"level\": \"4.874E+56\"},\"O'Ryan's Charm\": {\"enchanted\": \"False\",\"level\": \"3.805E+54\"},\"Heart of Storms\": {\"enchanted\": \"False\",\"level\": \"1.162E+48\"},\"Apollo Orb\": {\"enchanted\": \"False\",\"level\": \"8.816E+47\"},\"Sticky Fruit\": {\"enchanted\": \"False\",\"level\": \"7.396E+47\"},\"Hades Orb\": {\"enchanted\": \"False\",\"level\": \"4.811E+47\"},\"Earrings of Portara\": {\"enchanted\": \"False\",\"level\": \"6.873E+56\"},\"Avian Feather\": {\"enchanted\": \"False\",\"level\": \"3.236E+56\"},\"Corrupted Rune Heart\": {\"enchanted\": \"False\",\"level\": \"9.298E+52\"},\"Durendal Sword\": {\"enchanted\": \"True\",\"level\": \"4.606E+50\"},\"Helheim Skull\": {\"enchanted\": \"True\",\"level\": \"8.233E+50\"},\"Oath's Burden\": {\"enchanted\": \"False\",\"level\": \"4.960E+44\"},\"Crown of the Constellation\": {\"enchanted\": \"False\",\"level\": \"9.151E+47\"},\"Titania's Sceptre\": {\"enchanted\": \"True\",\"level\": \"4.152E+47\"},\"Fagin's Grip\": {\"enchanted\": \"True\",\"level\": \"5.225E+47\"},\"Ring of Calisto\": {\"enchanted\": \"True\",\"level\": \"1.077E+48\"},\"Blade of Damocles\": {\"enchanted\": \"False\",\"level\": \"9.242E+50\"},\"Helmet of Madness\": {\"enchanted\": \"False\",\"level\": \"1.025E+51\"},\"Titanium Plating\": {\"enchanted\": \"False\",\"level\": \"1.145E+51\"},\"Moonlight Bracelet\": {\"enchanted\": \"False\",\"level\": \"8.994E+50\"},\"Amethyst Staff\": {\"enchanted\": \"False\",\"level\": \"1.242E+51\"},\"Sword of the Royals\": {\"enchanted\": \"True\",\"level\": \"6.492E+47\"},\"Spearit's Vigil\": {\"enchanted\": \"True\",\"level\": \"5.393E+47\"},\"The Cobalt Plate\": {\"enchanted\": \"True\",\"level\": \"6.946E+47\"},\"Sigils of Judgement\": {\"enchanted\": \"True\",\"level\": \"5.952E+47\"},\"Foliage of the Keeper\": {\"enchanted\": \"True\",\"level\": \"6.800E+47\"},\"Invader's Gjallarhorn\": {\"enchanted\": \"True\",\"level\": \"4.606E+54\"},\"Titan's Mask\": {\"enchanted\": \"False\",\"level\": \"9.028E+52\"},\"Royal Toxin\": {\"enchanted\": \"False\",\"level\": \"3.894E+56\"},\"Laborer's Pendant\": {\"enchanted\": \"False\",\"level\": \"3.560E+56\"},\"Bringer of Ragnarok\": {\"enchanted\": \"False\",\"level\": \"8.994E+52\"},\"Parchment of Foresight\": {\"enchanted\": \"False\",\"level\": \"4.216E+56\"},\"Elixir of Eden\": {\"enchanted\": \"True\",\"level\": \"8.961E+52\"},\"Hourglass of the Impatient\": {\"enchanted\": \"False\",\"level\": \"4.000E+1\"},\"Phantom Timepiece\": {\"enchanted\": \"False\",\"level\": \"3.000E+1\"},\"Forbidden Scroll\": {\"enchanted\": \"False\",\"level\": \"3.000E+1\"},\"Ring of Fealty\": {\"enchanted\": \"False\",\"level\": \"3.000E+1\"},\"Glacial Axe\": {\"enchanted\": \"False\",\"level\": \"3.000E+1\"},\"Aegis\": {\"enchanted\": \"False\",\"level\": \"3.000E+1\"},\"Swamp Gauntlet\": {\"enchanted\": \"False\",\"level\": \"3.000E+1\"},\"Infinity Pendulum\": {\"enchanted\": \"False\",\"level\": \"2.000E+1\"},\"Glove of Kuma\": {\"enchanted\": \"False\",\"level\": \"3.000E+1\"},\"Titan Spear\": {\"enchanted\": \"False\",\"level\": \"4.000E+1\"},\"Oak Staff\": {\"enchanted\": \"False\",\"level\": \"3.000E+1\"},\"The Arcana Cloak\": {\"enchanted\": \"False\",\"level\": \"4.000E+1\"},\"Hunter's Ointment\": {\"enchanted\": \"False\",\"level\": \"4.000E+1\"},\"Ambrosia Elixir\": {\"enchanted\": \"False\",\"level\": \"4.000E+1\"},\"Mystic Staff\": {\"enchanted\": \"False\",\"level\": \"4.000E+1\"},\"Mystical Beans of Senzu\": {\"enchanted\": \"False\",\"level\": \"4.000E+1\"},\"Egg of Fortune\": {\"enchanted\": \"False\",\"level\": \"4.000E+1\"},\"Divine Chalice\": {\"enchanted\": \"False\",\"level\": \"5.000E+1\"},\"Invader's Shield\": {\"enchanted\": \"False\",\"level\": \"5.000E+1\"},\"Axe of Muerte\": {\"enchanted\": \"False\",\"level\": \"4.000E+1\"},\"Essence of the Kitsune\": {\"enchanted\": \"False\",\"level\": \"4.000E+1\"},\"Boots of Hermes\": {\"enchanted\": \"False\",\"level\": \"4.000E+1\"},\"Unbound Gauntlet\": {\"enchanted\": \"False\",\"level\": \"4.000E+1\"},\"Oberon Pendant\": {\"enchanted\": \"False\",\"level\": \"4.000E+1\"},\"Lucky Foot of Al-mi'raj\": {\"enchanted\": \"False\",\"level\": \"4.000E+1\"},\"Lost King's Mask\": {\"enchanted\": \"False\",\"level\": \"4.000E+1\"},\"Staff of Radiance\": {\"enchanted\": \"False\",\"level\": \"4.000E+1\"},\"Morgelai Sword\": {\"enchanted\": \"False\",\"level\": \"5.000E+1\"},\"Ringing Stone\": {\"enchanted\": \"False\",\"level\": \"5.000E+1\"},\"Quill of Scrolls\": {\"enchanted\": \"False\",\"level\": \"5.000E+1\"},\"Old King's Stamp\": {\"enchanted\": \"False\",\"level\": \"5.000E+1\"},\"The Master's Sword\": {\"enchanted\": \"False\",\"level\": \"4.000E+1\"},\"The Magnifier\": {\"enchanted\": \"False\",\"level\": \"4.000E+1\"},\"The Treasure of Fergus\": {\"enchanted\": \"False\",\"level\": \"4.000E+1\"},\"The White Dwarf\": {\"enchanted\": \"False\",\"level\": \"5.000E+1\"},\"Aram Spear\": {\"enchanted\": \"False\",\"level\": \"4.000E+1\"},\"Ward of the Darkness\": {\"enchanted\": \"False\",\"level\": \"6.000E+1\"}},\"splashStats\": {\"Splash Damage\": \"9.120E+59\",\"Base Max Splash Count\": \"15\",\"All Splash Skip\": \"1.114\",\"Heavenly Strike Splash Count\": \"21\",\"Lightning Burst Splash Count\": \"15\",\"Clan Ship Splash Count\": \"33\",\"Heavenly Strike Splash Skip\": \"152.6\",\"Pet Splash Skip\": \"152.6\",\"Clan Ship Splash Skip\": \"232.8\",\"Shadow Clone Splash Skip\": \"152.6\"},\"raidCards\": {\"Moon Beam\": {\"level\": 21,\"cards\": 50},\"Fragmentize\": {\"level\": 20,\"cards\": 17},\"Skull Bash\": {\"level\": 20,\"cards\": 128},\"Razor Wind\": {\"level\": 21,\"cards\": 140},\"Whip of Lightning\": {\"level\": 19,\"cards\": 50},\"Clanship Barrage\": {\"level\": 21,\"cards\": 24},\"Purifying Blast\": {\"level\": 21,\"cards\": 93},\"Psychic Chain\": {\"level\": 21,\"cards\": 3},\"Flak Shot\": {\"level\": 16,\"cards\": 111},\"Cosmic Haymaker\": {\"level\": 17,\"cards\": 13},\"Blazing Inferno\": {\"level\": 18,\"cards\": 9},\"Acid Drench\": {\"level\": 18,\"cards\": 91},\"Decaying Strike\": {\"level\": 18,\"cards\": 123},\"Fusion Bomb\": {\"level\": 19,\"cards\": 11},\"Grim Shadow\": {\"level\": 20,\"cards\": 78},\"Thriving Plague\": {\"level\": 20,\"cards\": 121},\"Radioactivity\": {\"level\": 19,\"cards\": 86},\"Ravenous Swarm\": {\"level\": 17,\"cards\": 67},\"Crushing Instinct\": {\"level\": 21,\"cards\": 110},\"Rancid Gas\": {\"level\": 20,\"cards\": 153},\"Inspiring Force\": {\"level\": 20,\"cards\": 138},\"Soul Fire\": {\"level\": 22,\"cards\": 46},\"Victory March\": {\"level\": 19,\"cards\": 107},\"Prismatic Rift\": {\"level\": 19,\"cards\": 122},\"Ancestral Favor\": {\"level\": 21,\"cards\": 127},\"Grasping Vines\": {\"level\": 21,\"cards\": 47},\"Totem of Power\": {\"level\": 20,\"cards\": 37}},\"equipmentSets\": [\"Ruthless Necromancer\",\"Angelic Guardian\",\"Treasure Hunter\",\"Dark Predator\",\"Fatal Samurai\",\"Ancient Warrior\",\"Anniversary Gold\",\"Phantom Presence\",\"Cybernetic Enhancements\",\"Dragon Slayer\",\"Corrupt Emerald Knight\",\"Defender of the Egg\",\"The Heartbreaker\",\"Snow Master\",\"Midnight Raven\",\"The Rockstar\",\"Surf Strike\",\"Viking King\",\"The Sly Wolf\",\"Amazon Princess\",\"Heartly Queen\",\"Captain Titan\",\"Chained Clockwork\",\"Solar Paragon\",\"Frost Warden\",\"Toxic Slayer\",\"Defiant Spellslinger\",\"Titan Attacker\",\"Scarecrow Jack\",\"Sled Season\",\"Shadow Disciple\",\"Anniversary Platinum\",\"Mechanized Sword\",\"Lunar Festival\",\"Eternal Monk\",\"Thundering Deity\",\"Blessed Bishop\",\"Noble Fencer\",\"Dedicated Fan\",\"Bone Mender\",\"Celestial Enchanter\",\"Combo Breaker\",\"Grim Reaper\",\"Jack Frost\",\"Anniversary Diamond\",\"Nimble Hunter\",\"Sweets and Treats\",\"Hidden Viper\",\"Heir of Shadows\",\"Heir of Light\",\"Ignus, the Volcanic Phoenix\",\"Ironheart, the Crackling Tiger\",\"Kor, the Whispering Wave\",\"Styxsis, the Single Touch\",\"Digital Idol\",\"Azure Knight\",\"Reckless Firepower\",\"Grill Master\",\"Golden Monarch\",\"Beast Rancher\",\"Black Knight\"],\"passiveSkills\": {\"Intimidating Presence\": 129,\"Power Surge\": 36,\"Anti-Titan Cannon\": 108,\"Mystical Impact\": 36,\"Arcane Bargain\": 101,\"Silent March\": 252},\"petLevels\": {\"Nova\": 587,\"Toto\": 624,\"Cerberus\": 553,\"Mousy\": 573,\"Harker\": 598,\"Bubbles\": 630,\"Demos\": 536,\"Tempest\": 542,\"Basky\": 595,\"Scraps\": 551,\"Zero\": 593,\"Polly\": 541,\"Hamy\": 626,\"Phobos\": 571,\"Fluffers\": 903,\"Kit\": 594,\"Soot\": 114,\"Klack\": 137,\"Cooper\": 173,\"Jaws\": 128,\"Xander\": 114,\"Griff\": 172,\"Basil\": 173,\"Bash\": 160,\"Violet\": 154,\"Annabelle\": 143,\"Effie\": 172,\"Percy\": 181,\"Cosmos\": 199,\"Taffy\": 181},\"skillTree\": {\"Knight's Valor\": 2,\"Chivalric Order\": 1,\"Pet Evolution\": 0,\"Heart of Midas\": 16,\"Cleaving Strike\": 19,\"Summon Inferno\": 0,\"Lightning Burst\": 0,\"Barbaric Fury\": 0,\"Flash Zip\": 0,\"Master Commander\": 17,\"Spoils of War\": 1,\"Heroic Might\": 18,\"Aerial Assault\": 17,\"Tactical Insight\": 16,\"Searing Light\": 19,\"Coordinated Offensive\": 19,\"Astral Awakening\": 13,\"Anchoring Shot\": 14,\"Limit Break\": 2,\"Midas Ultimate\": 12,\"Angelic Radiance\": 1,\"Phantom Vengeance\": 1,\"Fairy Charm\": 0,\"Mana Siphon\": 1,\"Eternal Darkness\": 1,\"Manni Mana\": 0,\"Lightning Strike\": 9,\"Dimensional Shift\": 5,\"Master Thief\": 13,\"Ambush\": 1,\"Assassinate\": 7,\"Summon Dagger\": 1,\"Stroke Of Luck\": 7,\"Dagger Storm\": 3,\"Cloaking\": 1,\"Forbidden Contract\": 0,\"Poison Edge\": 21,\"Deadly Focus\": 15}}";
            }
#endif

            if (string.IsNullOrWhiteSpace(text))
            {
                await NotifyUserAsync(AppResources.ErrorHeader, "Clipboard is empty", silentMode);
                return new ClipboardSfImporterResult(false, ClipboardSfImporterError.ClipboardEmpty);
            }

            // check if valid
            var checkResult = IsValidInput(text);
            if (!checkResult.success)
            {
                Logger.WriteToLogFile($"ImportSfFromClipboardCommand ERROR: {checkResult.exception.Message}\n{checkResult.exception.Data}.");
                await NotifyUserAsync(AppResources.ErrorHeader, "Invalid data provided", silentMode);
                return new ClipboardSfImporterResult(false, ClipboardSfImporterError.MalformattedClipboardData);
            }

            // check if current import differs from last import data in LocalSettingsORM
            var isDifferent = IsDifferentFromPreviousImport(text);
            if (!isDifferent)
            {
                Logger.WriteToLogFile($"ImportSfFromClipboardAsync: Input is the same as before");
            }
            #endregion

            ShowBigAd();

            SaveNewClipboardImportText(text);

            return new ClipboardSfImporterResult(true);
        }

        private void SaveNewClipboardImportText(string text)
        {
            // save clipboard content
            Xamarin.Forms.DependencyService.Get<IDirectory>().SaveTapTitansExportFile(text);

            // save text as current clipboard data in LocalSettingsORM
            LocalSettingsORM.CurrentSavefileString = text;
        }

        private bool IsDifferentFromPreviousImport(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            // get previous import
            var previous = LocalSettingsORM.CurrentSavefileString;

            if (string.IsNullOrWhiteSpace(previous))
            {
                return true;
            }

            // if previous import is not valid make it empty
            var prevValid = IsValidInput(previous);
            if (!prevValid.success)
            {
                Logger.WriteToLogFile($"IsDifferentFromPreviousImport: previous is not valid. making it null");
                LocalSettingsORM.CurrentSavefileString = null;
                return true;
            }

            // compare current text with previous import and return result
            return text != previous;
        }

        public static (bool success, Exception exception) IsValidInput(string text)
        {
            JToken sfObj;
            try
            {
                sfObj = JContainer.Parse(text);

                var profileData = (JObject)sfObj.SelectToken("playerStats");

                if (profileData == null) return (false, new Exception("input does not contain player stats"));
                
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex);
            }
        }

        private async Task<bool> NotifyUserAsync(string header, string body, bool silentMode)
        {
            if (!silentMode) await _dialogService.DisplayAlertAsync(header, body, AppResources.OKText);
            return true;
        }

        private void ShowBigAd()
        {
            Logger.WriteToLogFile($"Dashboard ShowBigAd start");
            if (!PurchaseableItems.GetBigAdVisible()) return;

            Logger.WriteToLogFile($"Dashboard ShowBigAd going to load and show ad");
            LogAdvertisementEventToAppCenter();

            _adInterstitial.LoadAd();
            _adInterstitial.ShowAd();
            if (LocalSettingsORM.IsReadingDataFromSavefile)
            {
                App.HaveIShownTheBigAd = true;
            }
        }

        private void LogAdvertisementEventToAppCenter()
        {
            try
            {
                var dict = new Dictionary<string, string>()
            {
                {"OS", Device.RuntimePlatform },
                {"Data Source", LocalSettingsORM.IsReadingDataFromSavefile ? "Savefile" : "Clipboard" },
            };
                Analytics.TrackEvent("Advertisement request", dict);
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile($"ERROR Could not log advertisement event: {ex.Message} - {ex.Data}");
            }
        }
    }
}
