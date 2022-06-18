using System.Collections.Generic;
using TT2Master.Shared.Models;

namespace TT2Master
{
    public static class ChangelogList
    {
        public static List<ChangelogItem> Changelog = new List<ChangelogItem>()
        {
            new ChangelogItem()
            {
                Version = 231,
                Changes = "" +
                "\n\n * Added new artifact from 5.9.0" +
                "\n\n * Added Raid Seed module as a little visualizer for the json file which you can obtain from the official GH discord server." +
                "\n\n * Fixed raid analysis wave calculation." +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 229,
                Changes = "" +
                "\n\n * Updated for 5.7.1" +
                "\n\n * SP optimizer is not ready yet!" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 228,
                Changes = "" +
                "\n\n * Updated for 5.6.0" +
                "\n\n * Fixed crashes on devices running below Android 7" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 226,
                Changes = "" +
                "\n\n * Made changes for 5.5.1" +
                "\n\n * Fixed an issue with advertisement being shown in split screen" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 225,
                Changes = "" +
                "\n\n * Snapshots: Added setting to configure amount of automatic daily snapshots" +
                "\n\n * Export: Added a very basic csv export to get the diff in morale and MS between two given snapshots" +
                "\n\n * Clipboard: Fixed BoS% for tournaments (setting was not properly detected)" +
                "\n\n * Artifact Optimizer Settings: Fixed an issue where step amount could be out of bounds" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 223,
                Changes = "" +
                "\n\n * Updated app to be compatible with Tap Titans 2 version 5.4.0" +
                "\n\n * Clipboard users can now set BoS% for tournaments but they need to specify if they are in a tournament in optimizer settings" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 222,
                Changes = "" +
                "\n\n * Fixed issue where you could not create snapshots" +
                "\n\n * Android: Added support for pending purchases" +
                "\n\n * iOS: Added support for App Tracking Transparency" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 221,
                Changes = "" +
                "\n\n * Fixed dashboard shortcuts not showing content" +
                "\n\n * Fixed SP configuration not getting updated" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 220,
                Changes = "" +
                "\n\n * Updated for version 5.3.0" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 219,
                Changes = "" +
                "\n\n * Fixed raid attack result popup" +
                "\n\n * Fixed loop after upgrade to android 11" +
                "\n\n * Removed RpM service as it is obsolete since GH added ingame number for this" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 218,
                Changes = "" +
                "\n\n * Finetuning on profile export" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 217,
                Changes = "" +
                "\n\n * Clan raid result import now works for all localized versions" +
                "\n\n * Added images for the new cards to profile export" +
                "\n\n * Added export page for clipboard users" +
                "\n\n * Artifact Optimizer: Raid Card artifacts are now automatically ignored if you have no raid cards" +
                "\n\n * Android SF - Artifact Optimizer now picks BoS-T% when in abyssal mode" +
                "\n\n * iOS - Raid Analysis: fixed background for attack flaws list view" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 216,
                Changes = "" +
                "\n\n * Added a button to switch between default and abyssal savefile" +
                "\n\n * Equip Advisor: Fixed secondary stat detection" +
                "\n\n * Added Tournament result view for clan messages" +
                "",
                Hyperlink = @"https://fistelmann.de/tt2master-abyssal-tournaments/",
            },
            new ChangelogItem()
            {
                Version = 215,
                Changes = "" +
                "\n\n * Fixed an issue which could cause TT2Master not to start" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 214,
                Changes = "" +
                "\n\n * SP Optimizer: Does not suggest leveling unefficient skills to spend every last skill point anymore" +
                "\n\n * SP Optimizer: You can now change between different optimization modes from the menu without the need to edit the build" +
                "\n\n * SP Optimizer: You can now add a description to your build configurations" +
                "\n\n * Raid Import: improved parsing from clipboard to be more reliable" +
                "\n\n * Raid Import: fixed an issue where enemy information could not be loaded" + 
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 213,
                Changes = "" +
                "\n\n * SP Optimizer: added text to show amount of skill points left" +
                "\n\n * SP Optimizer - creational builds: now spend the expected amount of skill points" +
                "\n\n * SP Optimizer - creational builds: added text to show which skill you need to upgrade next and how much it costs" +
                "\n\n * SP Optimizer - continuous builds: added text to show that a skill has been added which you can not affort" +
                "\n\n * Artifact Optimizer: Fixed an issue where text was stripped away" +
                "\n\n * Settings: Fixed display issue on current asset version" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 212,
                Changes = "" +
                "\n\n * Updated for 3.15.0" +
                "\n\n * SP Optimizer: fixed an issue where the optimizer thought you have a myth set completed even though this is not the case" +
                "\n\n * SP Optimizer: you can now set any fixed level for a skill as long as the value is above -1" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 211,
                Changes = "" +
                "\n\n * SP Optimizer: fixed level detection" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 210,
                Changes = "" +
                "\n\n * Added guide on logging with information on how to send logs" +
                "\n\n * Artifact Optimizer - Default List View: Added sort id before artifact name" +
                "\n\n * Skill Optimizer - Added amount of SP spent on each branch" +
                "\n\n * Raid Module: Added images for attack order (1 to 8)" +
                "\n\n * Startup: Fixed a bug with server communication which could lead to a crash" +
                "\n\n * Increased free amount of custom sp optimizer configurations" +
                "\n\n * Changed custom artifact builds to be accessible for every player" +
                "\n\n * Android Automations: Fixed a bug which leads to empty exports and automation service not working" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 208,
                Changes = "" +
                "\n\n * Fixed clipboard import issue in artifact optimizer" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 207,
                Changes = "" +
                "\n\n * Added simple crafting advisor. input equipment type and boost type. You will get a list with craftable equipment ordered by primary efficiency desc." +
                "\n\n The list will only contain equipment for which you completed the set." +
                "\n\n * You can now import clipboard data from inside the modules by tapping the reload button" +
                "\n\n * Bug fixes and stability improvements" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 206,
                Changes = "" +
                "\n\n * Raid Module: Added back csv exports" +
                "\n\n * Fixed some weird issue with purchasement recognition" +
                "\n\n Android" +
                "\n\n * Fixed clan msg export (sorting issue) with max amount" +
                "\n\n * Added some logging to automatic clan member export" +
                "\n\n iOS" +
                "\n\n * When a mail is sent we try to add logs" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 205,
                Changes = "" +
                "\n\n * Fixed clipboard export loading" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 204,
                Changes = "" +
                "\n\n * Merged announcements and changelog" +
                "\n\n Android only:" +
                "\n\n* Fixed \"too many interstitial ads\"" +
                "\n\n iOS only:" +
                "\n\n* subscriptions have been temporarily removed" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem
            {
                Version = 203,
                Changes = "" +
                "\n\n* Fixed duplicate equipment in euqip advisor" +
                "\n\n* Added setting for \"All Heros maxed\" in artifact optimizer to ignore gold artifacts" +
                "\n\n* Hide and ignore tournament BoS royalty in artifact optimizer when loading data from clipboard export" +
                "\n\n  iOS only:" +
                "\n\n* Fixed purchasement validation" +
                "\n\n* Fixed margins of navigation menu so users can open settings" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 201,
                Changes = "" +
                "\n\n* Implemented support for 3.13.0's clipboard export" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 199,
                Changes = "" +
                "\n\n* Important stability improvements" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 198,
                Changes = "" +
                "\n\n* Changed file handling for Android 11" +
                "\n\n* Added 100% as a step for artifact optimizer" +
                "\n\n* Fix: announcements are now sorted correctly" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 197,
                Changes = "" +
                "\n\n* Required update for 3.12.0 because of structural changes on artifacts and raid cards" +
                "\n\n* Added better error message for wrong strategy config on raid analysis" +
                "\n\n* Fixed some translations" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 196,
                Changes = "" +
                "\n\n* Added announcement system" +
                "\n\n* Fixed UI issue on artifact optimizer image view" +
                "\n\n* Fixed equip advisor bug" +
                "\n\n* Framework updates and stability improvements" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 195,
                Changes = "" +
                "\n\n* Added info text for absolute attacks in raid tolerance" +
                "\n\n* Added parts analysis export to show overkill on parts" +
                "\n\n* bugfixes and stability improvements" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 194,
                Changes = "" +
                "\n\n* added toast info for when raid results got pasted" +
                "\n\n* reduced the amount of big ads being shown" +
                "\n\n* fixed wave tolerance calculation" +
                "\n\n* fixed wave tolerance display in analysis" +
                "\n\n* fixed issues with automatic asset update" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 193,
                Changes = "" +
                "\n\n* Ready for 3.11" +
                "\n\n* Added a simple raid module" +
                "\n\n* Dashboard: The dashboard is now configurable." +
                "\n\n* Dashboard: Merged equip drops and todo" +
                "\n\n* Artifact Optimizer: You can now mark entries as done by tapping on them" +
                "\n\n* Settings: added a button to export the database" +
                "\n\n* You can now reset your daily prestige amount by clicking on the corresponding chart inside the dashbaord" +
                "\n\n* The prestige text now differs when the last snapshot is older than a day" +
                "\n\n* Improved performance on charts" +
                "\n\n* Fixed potential bug with missing artifact optimizer settings" +
                "",
                Hyperlink = @"https://www.reddit.com/r/TapTitans2/comments/h861cg/tt2master_update_193/",
            },
            new ChangelogItem()
            {
                Version = 184,
                Changes = "" +
                "\n\n* You can now display an in app report from inside a snapshot" +
                "\n\n* SP Optimizer: Added UI logic for new myth set" +
                "\n\n* Fix: Statistics with ID 0 could not be exported" +
                "\n\n* Fix: SP Optimizer not initializing in some very rare cases" +
                "\n\n* Added some logging to startup" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 183,
                Changes = "" +
                "\n\n* Added tiny reporting functionality. this will grow in future and become customizable" +
                "\n\n* ATTENTION: Support mail address changed to tt2master@fistelmann.de" +
                "\n\n* Artifact optimizer: Fixed image layout not taking full height" +
                "\n\n* Artifact builds: Fixed that unignoring an artifact was not saved" +
                "\n\n* SP Optimizer: Fixed suggestions above max level" +
                "\n\n* Fixed a case where mail sending does not work" +
                "\n\n* Fixed clan member comparison" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 180,
                Changes = "" +
                "\n\n* Set 1.01 as min. efficiency" +
                "\n\n* Added alternative result view for artifact optimizer." +
                "\n\n* Added a new setting to automatically create a snapshot when you hit the dashboard refresh button" +
                "\n\n* Removed Pushing Type" +
                "\n\n* Made SP Optimizer Eff display 4 digits long" +
                "\n\n* Moded BoS display setting to dashboard" +
                "\n\n* Reorganized profile view" +
                "\n\n* Dashboard: BoS% is now pink when beneath desired value" +
                "\n\n* Fixed skill names" +
                "\n\n* Fixed free equipment drop amount" +
                "\n\n* Fixed passive skill levels" +
                "\n\n* Fixed tournament recognition in artifact optimizer reload logic" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 173,
                Changes = "" +
                "\n\n* Added a simple (temporary) fallback logic for startup failure due to 3.9.0" +
                "\n\n If you are having troubles starting the app this update may help. On the first start the app will still fail but set a flag that it failed. On the second start the app tries to repair itself" +
                "\n\n* Fixed artifact and enchantment costs" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 171,
                Changes = "" +
                "\n\n* Ready for 3.9.0 (except new artifact default weighting, artifact costs, enchantment costs and new myth set)" +
                "\n\n* Weighting for default artifacts will be patched automatically as soon as I got that going" +
                "\n\n* There will be a separate patch for the remaining stuff" +
                "\n\n* Artifact optimizer: Min. efficiency is not a slider anymore! :O" +
                "\n\n* Improved reliability of the share functionality" +
                "\n\n* Reduced button rounding for a better UI experience" +
                "\n\n* Added some new features to automation service" +
                "\n\n* Added pictures for new raid cards" +
                "\n\n* Added button in settings to change display of BoS%" +
                "\n\n* Fixed an issue related to IAP" +
                "\n\n* Fixed french translations thanks to 6mon" +
                "\n\n* Fixed display of advanced start and passive skills" +
                "\n\n* Some small bug fixes" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 164,
                Changes = "" +
                "\n\n* Fixed issue with SP Optimizer" +
                "\n\n* Finally added undisputed wins as player property" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 163,
                Changes = "" +
                "\n\n* SP Optimizer: Fixed a bug where you can not set a pushing type other than online." +
                "\n\n The next versions will hopefully not purely be about bugfixes -.-" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 162,
                Changes = "" +
                "\n\n* Added possibility to manually download assets again. If you have any issues like the new equipment is not shown or the new skills are not shown try to use it (go to settings, scroll down and press button). " +
                "If that also does not work contact me again (either via discord or via mail - starting at the \"about me\" page)" +
                "\n\n* fixed a typo" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 161,
                Changes = "" +
                "HOTFIXES FOR 3.8" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 160,
                Changes = "" +
                "\n\n* fixed issue with equip advisor" +
                "\n\n* fixed issue with automatic clan member export" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 159,
                Changes = "" +
                "\n\n* Updated Enchantment cost list due to 3.7.0" +
                "\n\n* fixed a problem with the automatic clan member export. If you have a custom delimiter you need to set it again in the settings menu" +
                "\n\n* fixed a rare issue with the asset manager" +
                "\n\n* fixed a bug regarding the RpM module" +
                "\n\n* fixed pop up text for \"Export Raid as Json\"" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 158,
                Changes = "" +
                "\n\n* You can now change the path to your savefile in the settings menu" +
                "\n\n* fixed an issue where the progressbar in dashboard always shows 100%" +
                "\n\n* fixed an issue with the thai calendar causing several modules of the app not to work" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 157,
                Changes = "" +
                "\n\n* hopefully fixed a bug where the savefile could not be read" +
                "\n\n* (it was not my fault)" +
                "\n\n* (really - it was not!)" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 156,
                Changes = "" +
                "\n\n* added automatic clan export" +
                "\n\n* added automation service" +
                "\n\n* added context menu to clan messages so you can copy content to clipboard" +
                "\n\n* added max. amount of clan messages to export" +
                "\n\n* files are now saved inside a subfolder named TT2Master" +
                "\n\n* Artifact optimizer: added support for level rounding on absolute step amounts (the setting will be enabled if you set it ingame)" +
                "\n\n* We now have a new logo :)" +
                "\n\n* fixed an error with the asset manager and optional updates from GameHive" +
                "\n\n* if there is an error with the asset manager you now get only one notification as intended" +
                "\n\n* fixed an issue regarding the file picker when the savefile could not be found" +
                "\n\n* ATTENTION: Please check your Skill optimizer settings. I needed to rewrite them!" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 147,
                Changes = "" +
                "\n\n* fixed a bug where you see ads when you are supporter over time" +
                "\n\n* fixed a UI bug in settings where version numbers overwrite each other" +
                "\n\n* added more logging to asset manager just in case something goes wrong" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 146,
                Changes = "" +
                "\n\n* App should support x86 architecture again" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 145,
                Changes = "" +
                "\n\n* Fixed average MS missing on clan overview" +
                "\n\n* Fixed a bug in equip advisor where boss gold was prefered over chesterson gold" +
                "\n\n* Outsourced some assets to my server to provide even faster updates in future" +
                "\n\n* Soulrise made a tutorial for TT2Master usage. Make sure to check it out and subscribe to his channel to get more Tap Titans 2 content! You find a link to the " +
                "tutorial down below :)" +
                "",
                Hyperlink = @"https://www.youtube.com/watch?v=RyHW20IP8PY",
            },
            new ChangelogItem()
            {
                Version = 142,
                Changes = "" +
                "\n\n* Updated to 3.5.0" +
                "\n\n* Adjusted profile export to fit new pets" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 141,
                Changes = "" +
                "\n\n* Removed artifact weighting by category. Now every artifact is weighted for itself. Please check your custom builds!" +
                "\n\n* Added minimum efficiency setting to artifact optimizer" +
                "\n\n* Efficiency is now displayed right to artifact name" +
                "\n\n* Upgrade reason is now displayed. EFF = efficiency. AD = artifact damage" +
                "\n\n* Added maximum artifact amount to artifact optimizer" +
                "\n\n* Added advanced start to clan overview" +
                "\n\n* Added raid card export as json (available from export page)" +
                "",
                Hyperlink = @"https://www.reddit.com/r/TapTitans2/comments/dxpja7/tt2master_version_141_artifact_optimizer/",
            },
            new ChangelogItem()
            {
                Version = 138,
                Changes = "" +
                "\n\n* Updated info files" +
                "\n\n* Added new Cards" +
                "\n\n* Updated advanced start in dashboard (thanks to DreamXZE)" +
                "\n\n* Added another fallback value if current relics could not be read" +
                "\n\n* Added fallback method to send logfiles on error" +
                "\n\n* Added reload button to SP optimizer" +
                "\n\n* FIX: Date in clanmember export file name is now correct" +
                "\n\n* FIX: Custom gold source setting will no longer be deleted for default builds" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 137,
                Changes = "" +
                "\n\n* Reworked Artifact optimizer (in terms of efficiency calculation)" +
                "\n\n* Added Current Relics to Artifact & SP Export" +
                "\n\n* Added SaveDate to ClanMemberExport" +
                "",
                Hyperlink = @"https://www.reddit.com/r/TapTitans2/comments/docbn9/tt2master_version_137/",
            },
            new ChangelogItem()
            {
                Version = 136,
                Changes = "" +
                "\n\n* Updated to 3.4.0" +
                "\n\n* Hopefully fixed UI issue with collections where some values are not displayed after scrolling" +
                "\n\n* Removed _TAP build for artifact optimizer (forgot to do that earlier)" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 135,
                Changes = "" +
                "\n\n* BUGFIX: There was an issue with the Info file parsing. Thanks to Yupla, Justin and Robyn i was able to solve it fast." +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 134,
                Changes = "" +
                "\n\n* Coins of Ebizu where suggested when it made no sense. This has been fixed" +
                "\n\n* Updated Framework" +
                "\n\n* Hyperlink in changelog popup is now clickable" +
                "\n\n* Stability improvements" +
                "\n\n* Changed the way how emails are sent out of this app to be more reliable" +
                "\n\n* Added links from optimizers to in app guides" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 133,
                Changes = "" +
                "\n\n* Updated for 3.3.1" +
                "\n\n* Graphical raid card and deck export (go to Things->Export)" +
                "\n\n* Adressed very weird 'not enough relics' bug caused by CurrentRelics being 0 (savefile parsing issue)" +
                "",
                Hyperlink = @"https://www.reddit.com/r/TapTitans2/comments/dd8iax/tt2master_version_133_graphical_raid_card_export/",
            },
            new ChangelogItem()
            {
                Version = 132,
                Changes = "" +
                "\n\n* Updated for 3.3" +
                "\n\n* Graphical profile export preview (go to Things->Export)" +
                "\n\n* Stability improvements" +
                "\n\n* Improved Artifact optimizer logging" +
                "",
                Hyperlink = @"https://www.reddit.com/r/TapTitans2/comments/dbvhcv/tt2master_version_132_graphical_profile_export/",
            },
            new ChangelogItem()
            {
                Version = 131,
                Changes = "" +
                "\n\n* Updated for 3.2.4" +
                "\n\n* Fixed another tournament member detail navigation crash" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 130,
                Changes = "" +
                "\n\n* Fixed tournament member detail navigation crash" +
                "\n\n* Fixed Crash when opening the SP optimizer" +
                "",
                Hyperlink = @"",
            },
            new ChangelogItem()
            {
                Version = 129,
                Changes = "" +
                "\n\n* Updated everything for Version 3.2.2" +
                "\n\n* You can now see tournament member statistics and compare members with each other" +
                "\n\n* You can now input a negative overclock amount for the SP Optimizer" +
                "\n\n* Made some finetuning regarding the maths of the SP Optimizer" +
                "\n\n* Added an option to the SP Optimizer to ignore skills you can not afford. The optimizer then tries to find a skill which you can afford." +
                "",
                Hyperlink = @"https://www.reddit.com/r/TapTitans2/comments/cpvl6d/tt2master_version_129/",
            },
            new ChangelogItem()
            {
                Version = 128,
                Changes = "" +
                "\n\n* Added SP Optimizer" +
                "\n\n* Raid attack results now have their own category in the clan messages" +
                "\n\n* You can now compare clan member. Go to the clan member overview and press on a member for a while. a button will show for comparison. then you pick a player to compare with." +
                "\n\n* In the dashboard you got a button called \"Tournament\" and a lot of people got confused with it. I now added a little functionality to it. More to come!" +
                "\n\n* Updated info files" +
                "",
                Hyperlink = @"https://www.reddit.com/r/TapTitans2/comments/cl6mu9/tt2master_version_128/",
            },
            new ChangelogItem()
            {
                Version = 121,
                Changes = "" +
                "\n\n* 2 artifacts where not recognized which caused some trouble. This is now working again." +
                "\n\n* Because the two artifacts where not recognized, artifact ignoration somehow did not work quite good -> also fixed." +
                "\n\n* Fixed the issue that ignored artifacts where not shown on build edit" +
                "\n\n* Changed AR Skip to Heavenly Strike skip" +
                "",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 120,
                Changes = "" +
                "\n\n* Compatible with version 3.2 (need to rework skipping. Do not use AR calculation for now)" +
                "\n\n* Artifact optimizer AD% calculation improved" +
                "\n\n* Artifact optimizer considers enchanted Artifacts" +
                "\n\n* LTR-Calculation considers enchanted Artifacts" +
                "\n\n* ClanRole and LastSeen are now exportable from snapshot" +
                "\n\n* Fixed a very strange savefile-read bug" +
                "\n\n* Fixed a bug where you could not see builds from clan messages" +
                "\n\n* Added polish translation. Thank you VrozaX" +
                "\n\n* Fixed a bug where you could not send logfiles" +
                "",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 115,
                Changes = "* Your profile now shows the level of passive skills and LTR (you can still copy the value to clipboard in the dashboard by clicking on BoS%)" +
                "\n\n* As there are a lot of items in the menu I implemented groups" +
                "\n\n* Added Skill skip calculator" +
                "\n\n* Equipment Advisor now fully reloads when the reload button is pressed." +
                "\n\n* Fixed clan member sorting in overview" +
                "\n\n* Fixed another HS weighting bug" +
                "\n\n* Fixed snapshot export and navigation bug"
                ,
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 114,
                Changes = "* Fixed a whole lot of UI-issues"
                ,
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 113,
                Changes = "* Maybe fixed App-crash on Android 9" +
                "\n* Fixed: Prestige count showing lifetime prestige. It will take a day until everything is normal again." +
                "\n* Fixed: Clan member detail navigation (could not navigate when on your player)" +
                "\n* Fixed: Artifact information is now shown for every clan member as intended" +
                "\n* Fixed: Problems when clicking on build from messages"
                ,
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 112,
                Changes = "* Clan messages are now saved in the local database. you can set the maximum amount of stored messages in the app settings. In addition to that you can now search through the clan messages." +
                "\n* Language should now reload at most places without having to exit the app" +
                "\n* Adjusted DS and slash weight for HS-Build" +
                "\n* Added fallback value for clan member export if something is missing." +
                "\n* Self created snapshots will not be deleted anymore unless you have more snapshots than the max. amount in the settings" +
                "\n* The clan member detail got a redesign. You can now see all available properties within a tabbed page" +
                "\n* You can now navigate through the clan members with toolbar buttons" +
                "\n* Updated the guides - especially the artifact optimizer guide" +
                "\n* Fixed several navigation issues"
                ,
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 109,
                Changes = "* Fixed HS-Build" +
                "\n* Added 10 new properties to the clan export." +
                "\n* Clan and snapshot export are now highly customizable! Set which properties you want to export in which order."
                ,
                Hyperlink = "",
            },

            new ChangelogItem()
            {
                Version = 108,
                Changes = "* Improved Click suggestion. It is now the standard setting. Also the lifetime%-setting is removed as I no longer need it." +
                "\n* You can now choose which categories to export from the Clan messages" +
                "\n* When Exporting a SP build it is now the name of the skill which is taken and not the id of the skill" +
                "\n* Fixed: Clan member export did not get players total raid attack count" +
                "\n* You can now set the csv delimiter in the settings for your own" +
                "\n* Fixed: Duplicate entries in equipment drop list" +
                "\n* Maybe fixed a font color issue with the widget (white font on white background)" +
                "\n* Clan member detail now shows increase of ticket count (TC) and not Clan quest (CQ)" +
                "\n* Snapshots now save the total tickets a player collected and it will be used in exports" +
                "\n* Added sharing options to exports" +
                "\n* Added a reload button to the equip advisor" +
                "\n* Added the four new Artifacts" +
                "\n* Added total raid xp to the clan export"
                ,
                Hyperlink = "",
            },

            new ChangelogItem()
            {
                Version = 106,
                Changes = "* Artifact optimizer: Clicksuggestion now sums multiple artifacts which rank are next to each other" +
                "\n* Adjusted Player properties to store new raid properties" +
                "\n* Adjusted Clan member export to display raid properties" +
                "\n* Adjusted Snapshots to store raid properties" +
                "\n* Added back daily achievments (which is now a list instead of a single item) to the dashboard and the widget" +
                "\n* Added translation for the new equipment set" +
                "\n* Equipment Advisor now get the correct values again" +
                "\n* Equipment Advisor now recognizes event-parts again" +
                "\n* Translation fixes for Turkish and Chinese" +
                "\n* ClanMessages now have an own category for raid-related messages" +
                "\n* ClanMessages can now be exported" +
                "\n* The ClanOverview got a little polish"
                ,
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 105,
                Changes = "Ducktape-Update for v3.0" +
                "\n* Added new Artifact" +
                "\n* Updated some Values" +
                "\n* Removed daily achievments (just for now)",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 104,
                Changes = "* Removed Compendium Page" +
                "\n* Days-amount can now be set again in Clan member detail" +
                "\n* Fixed chinese translation" +
                "\n* Fixed tournament-recognition" +
                "\n* Added Profile page which you can access from the dashboard",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 103,
                Changes = "* Added prestige amount to snapshots. The daily prestige amount will now be shown in the dashboard\n* Improved Boss kill chart\n* Fixed a typo\n* Removed Boss kills, Boss alarm and display of tournament progress in widget causing unexpected behaviour",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 102,
                Changes = @"Menu:
- Added seperate button for settings
- Boss Timer settings are now in the general settings
- Changelog now opens after update

Dashboard:
- If you click on Artifact amount now get to the artifact overview
- Equip advisor now has an own place
- added percentual value of progress for current run

RpM:
- Mins passed in Notification
- RpM record should now stop after prestige

Exports:
- BossDamage is now in clan member export

Fixes:
- Fixed bug where multiple snapshots where saved for a day
- fixed language setting display bug
- fixed possible crash in boss kill export
- fixed possible crash in sp follower",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 101,
                Changes = "* Little improvements for the RpM feature",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 100,
                Changes = "* Voted Feature 1: Boss kill statistics now have CQ-Amount. CQ per boss calculation added but it is far from stable \n* Voted Feature 2: Relics/min calculation. For usage take a look at the youtube video in the TT2Master-Playlist.\n* Added Clan Ship pHoM SP Build\n* Added Shadow Clone SP Build \n* Fixed a bunch of bugs\n* Added a bunch of new languaguages - translated from computers :/",
                Hyperlink = @"https://youtu.be/IRFEbr9mt1M",
            },
            new ChangelogItem()
            {
                Version = 98,
                Changes = "* Boss kill statistics \n* Improved equip advisor. Thanks to Cauchy for his help. Without him this feature would not be possible\n* Removed Compendium Builds and added redirection to them\n* Improved Artifact efficiency formula (also props to Cauchy here)",
                Hyperlink = @"https://www.reddit.com/r/TapTitans2/comments/awwv14/tt2master_update_098/",
            },
            new ChangelogItem()
            {
                Version = 95,
                Changes = "* Fixed not in clan-error \n* little weightning improvement for equip advisor\n* fairy fix\n* Teto export \n* Clan export to clipboard",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 94,
                Changes = "* Added Equipment Advisor \n* Added Changelog\n*Dashboard chart now works as intended\n* Added minimum value for Lifetime% \n* We now have a discord server. Feel free to join :)",
                Hyperlink = "https://discord.gg/zWw3bgr",
            },
            new ChangelogItem()
            {
                Version = 86,
                Changes = "Equip Advisor goes into testing",
                Hyperlink = "https://www.reddit.com/r/TapTitans2/comments/ar1qcl/tt2master_equip_advisor_in_testing/",
            },
            new ChangelogItem()
            {
                Version = 85,
                Changes = "* Fixed reload button in Dashboard \n* You can now also set the gold source in the artifact optimizer settings \n* Fixed some startup issues \n* Extended logging",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 84,
                Changes = "* Added a button to manually reload the Dashboard \n* Equip drops do now have a single row for each entry \n* Dashboard: If you click on Artifacts amount you enter the Overview \n* Dashboard: If you click on Bos% you LTR will be copied to the clipboard",
                Hyperlink = "https://www.reddit.com/r/TapTitans2/comments/anijbq/tt2master_update_084/",
            },
            new ChangelogItem()
            {
                Version = 83,
                Changes = "*There is some kind of issue again. Some users have crashes. Hope this will help",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 82,
                Changes = "* Added Dashboard. You now start there instead of the artifact optimizer. \n* Auto-snapshotting for everyone \n* Stuff I forgot",
                Hyperlink = "https://www.reddit.com/r/TapTitans2/comments/aho762/tt2master_082_dashboard/",
            },
            new ChangelogItem()
            {
                Version = 81,
                Changes = "*Various bugfixes",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 79,
                Changes = "App could crash I don't know why, fixed this bug and don't know how. Do you have some issues, too? Together we will work this through.",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 78,
                Changes = "* Improved the optimizer \n* Auto-Snapshotting for supporter \n* Reworked Informations-Page to have little tutorials \n* Charts will now be better displayed \n* Changed Step amounts to be the same like ingame. \nCAUTION: Because i changed the step amounts please double check your settings in the Artifact optimizer",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 76,
                Changes = "*Bugfix in clan member detail. You could only watch one member and then had to no chance to enter another member. This is due to a bug in the framework i use. I had to disable the \"days\" setting. It is now seven days. As soon as the framework is fixed i will reimplement the option to pick the days that the statistics are based on",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 75,
                Changes = "*Added statistics module \n*Added JayroyExport \n*Increased Update-Time",
                Hyperlink = "https://www.reddit.com/r/TapTitans2/comments/aagqq2/tt2master_update_075_statistics_and_jayroxexport/",
            },
            new ChangelogItem()
            {
                Version = 74,
                Changes = "*Added BoS-Royalty for tournaments. Now you can set you royalty for both cases and do not have to change this so often \n*Fixed a bug in the export of SP builds",
                Hyperlink = "https://www.reddit.com/r/TapTitans2/comments/a9zmlc/tt2master_update_074/",
            },
            new ChangelogItem()
            {
                Version = 73,
                Changes = "*Artifact-Optimizer:\n - Settings moved to popup page (you have to set your stuff once again)\n - More step amounts\n - Added click suggestion\n - Current artifact build has moved to optimizer settings \n*Small clan member export (from memberlist) \n*Fixes",
                Hyperlink = "https://www.reddit.com/r/TapTitans2/comments/a9oifn/tt2master_update_073/",
            },
            new ChangelogItem()
            {
                Version = 72,
                Changes = "*Added Banlist. Navigate to a clan member and press on the trash can to ban him. If you start the app and there are banned player in your clan there will be a notification.",
                Hyperlink = "https://www.reddit.com/r/TapTitans2/comments/a68qyp/tt2master_072_banlist/",
            },
            new ChangelogItem()
            {
                Version = 71,
                Changes = "*Stealth Artifacts now handled correcly \n*Added small export which includes only the level",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 70,
                Changes = "*Coin of Ebizu is now available again. \n*Fixed some artifact names for english translation",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 69,
                Changes = "*Ready for 2.12.0 \n*Gave Flote of the Solist more weight in default Builds",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 68,
                Changes = "*Another bugfix: It happened that the weights for optimization where not correctly recreated.",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 67,
                Changes = "Critical Bugfix: On the last update I really fastened the build recreation. Turnes out I just deleted them -.- \nPS: If you are on version 0.6.6 you should really install this.",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 66,
                Changes = "- Added a little clan view with build import from clan messages\n - Increased initialization time",
                Hyperlink = "https://www.reddit.com/r/TapTitans2/comments/a2gi72/tt2master_066_with_little_clan_stuff/",
            },
            new ChangelogItem()
            {
                Version = 65,
                Changes = "*Added json export for Artifacts and Skills \n*Added some error handling to SP Optimizer",
                Hyperlink = "https://www.reddit.com/r/TapTitans2/comments/a1l3am/tt2master_update_065_export/",
            },
            new ChangelogItem()
            {
                Version = 64,
                Changes = "*Improved ID-catching for initial installation \n*Added portuguise (pt-BR). Thank you Dexless! \n*Added some SP Builds from compendium",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 62,
                Changes = "*Added support for German Language. You can set it in the Settings-Page (if you want your language - contact me and we will implement it together) \n*Supporter can now create custom categories for artifact optimization. If created, go to artifact overview, click on an artifact and set the category. Weighting can be done via builds \n*Number style-setting (2-char-support) \n*Privacy policy",
                Hyperlink = "https://www.reddit.com/r/TapTitans2/comments/9z67ns/tt2master_localization_and_stuff/",
            },
            new ChangelogItem()
            {
                Version = 61,
                Changes = "- Increased possibility to have a bug fixed",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 60,
                Changes = "- Ready for 2.11.0\n - Possibly fixed an issue where the save file could not be picked if TT2 was not stored on your primary storage\n - Maybe fixed some other stuff. Forgot about that. The other one was so messy it washed my brain :|",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 59,
                Changes = "- Fixed Bug in Widget for Android Oreo and above\n - When save file is not found you now get the option to pick the path yourself",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 58,
                Changes = "- Finally added 2.10 builds and replaced them with the old builds\n - Bugfix in SP Optimizer\n - BossTimer reset settings fix\n - Widget daily achievement now with spaces (because I forgot that last update :D)",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 57,
                Changes = "- BossTimer fix (value for notified boss were not saved correctly and you were spammed wit notifications)\n - Widget reload fix (listview in widget was not updating)\n - Widget cosmetics. Thanks KalNymeri",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 56,
                Changes = "- Fixed a bug where users from Thailand could not start the app",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 55,
                Changes = "- Added offline mode. When first starting the App after install or update\n - you have to be online to get resources. After that you do not have to be online anymore. If you still want to update each startup you can change this in settings. Without it\n - app startup time heavily increases. From Settings you can update manually if you like to.\n - Widget provides now more value when no tournament is running. Added Daily achievements and equip drops there\n - Maybe fixed a bug with Navigation",
                Hyperlink = "https://www.reddit.com/r/TapTitans2/comments/9lvvy0/tt2master_055_widget_upgrade_and_offline_mode/",
            },
            new ChangelogItem()
            {
                Version = 53,
                Changes = "- Fixed a bug in Optimizer for high MS player (thank you jarod :-*)\n - New BossTimer functionality: Store weekly CQ and see your progress in notification\n - Fixed a typo in Widget",
                Hyperlink = "https://www.reddit.com/r/TapTitans2/comments/9l03a7/tt2master_bosstimer_with_weekly_cq/",
            },
            new ChangelogItem()
            {
                Version = 50,
                Changes = "- Made some UI decisions\n - Fixed a bug with Android 9 not loading the widget",
                Hyperlink = "https://www.reddit.com/r/TapTitans2/comments/9jz5vo/tt2master_widget_update_now_with_price_type/",
            },
            new ChangelogItem()
            {
                Version = 49,
                Changes = "- Added Boss Notification\n - Stability Improvements\n - Increased minimum Android Version to API Level 21 (TT2 does not even run on lower devices)",
                Hyperlink = "https://www.reddit.com/r/TapTitans2/comments/9ib5kw/tt2master_boss_notification/",
            },
            new ChangelogItem()
            {
                Version = 45,
                Changes = "- Increased Widget stability. The reload button is now more reliable\n - You can now see the BonusType of upcoming Tournaments\n - You can now enter Artifact Details by tapping on a list item from Artifact Overview\n - Fixed some UI-specific bugs\n\n **Please reset default Builds in settings menu. There has been a minor change.**",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 44,
                Changes = "There was something wrong with the update before.",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 43,
                Changes = "- Fixed a bug where some artifacts where not ignored when manually set\n- Little finetuning in Gold Sources",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 42,
                Changes = "- Gold Source now changeable for everyone. Navigate to Builds -> Select a build -> Change Source -> Save\n- Coin of Ebizu now correctly replaced by Neko Sculpture",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 41,
                Changes = "Little Adjustments for 2.10",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 40,
                Changes = "- Tournament-Widget Improvements",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 39,
                Changes = "* Tournament-Widget Hotfix",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 38,
                Changes = "*Bugfix for Widget: For the tournament tomorrow I wanted to get this working. It kinda does now. You have a reload button on which you can load the data when you need them.",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 37,
                Changes = "*Fixed Bug where widget did not load when app is closed. Sorry for that",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 36,
                Changes = "* Added Tournament-Widget. Keep sure that you have started the app at least once before usage.",
                Hyperlink = "https://www.reddit.com/r/TapTitans2/comments/98keu6/tt2master_tournament_widget/",
            },
            new ChangelogItem()
            {
                Version = 34,
                Changes = "- Fixed a bug where In App Purchases were not recognized",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 33,
                Changes = "- Added SP Follower (first step).\n- Worked a bit on UI",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 26,
                Changes = "*Added Artifact Overview so you can see the percentage, level and category of all artifacts. From Main Menu click on the book to enter\n*Optimizer Settings are now saved more frequently",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 25,
                Changes = "*Added deliminator to suggested Amount so you do not have to count zeros. Sorry, wanted to implement this in the last release but forgot it °__°",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 24,
                Changes = "*If your Step amount was too high, artifacts were not shown. That is now changed. If the suggested level cannot be rounded to the step amount, a non rounded value is shown.",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 23,
                Changes = "*Heavy feature: Orange accent color!!!!!!!!!! #2019 #cloud #industry4.0 #web2.0\n*Fixed send mail to developer function.",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 22,
                Changes = "*Added Optimizer Logger. Activate from settings when you need it. From the info area click on send Mail to send the logs to me.\n*Fixed another issue causing the app to crash on startup, where the documents path could not be found.",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 21,
                Changes = "*Added Logfile sending when the app crashes. Some user that experiencing crashes are unable to send a textfile. This should help.\n*Found and fixed a bug causing user dialogs not to be displayed",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 20,
                Changes = "* Added Support for 2.9.2\n* Even more changes on UI\n* Added little descriptions for optimizer settings",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 18,
                Changes = "* Finetuning on Optimizer\n* Artifact Level (current percentage) now calculated even more correctly :'D\n* Did some changes on the UI. More to come. Hope you like it :)",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 17,
                Changes = "* Fixed a bug where some people could not start the app.\n* Added a logger for initialization. On start, a logfile is created and saved in your documents folder. If you have any issues on startup you can send me this logfile so I can resolve it.",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 14,
                Changes = "*Solved issues with saving optimizer settings.\n*Important: There seems to be an issue with calculating the correct BoS Upgrade amount. I changed a whole lot of values for the calculation. So you may have to adjust your Lifetime% value to get results from before.",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 13,
                Changes = "It is now secured that you have a current build. some people had issues setting this",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 12,
                Changes = "You can now purchase the supporter package correctly. With it you will have no more ads and gain accesss to creating your own builds as well as sharing them.",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 11,
                Changes = "Bugs fixed",
                Hyperlink = "",
            },
            new ChangelogItem()
            {
                Version = 10,
                Changes = "Initial Release",
                Hyperlink = "https://www.reddit.com/r/TapTitans2/comments/8x1izv/tt2master_artifact_optimizer_for_android/",
            },
        };
    }
}
