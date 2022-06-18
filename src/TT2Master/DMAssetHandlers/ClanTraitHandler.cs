using System;
using System.Collections.Generic;
using System.Linq;
using TT2Master.Helpers;
using TT2Master.Model.Assets;
using TT2Master.Shared;
using TT2Master.Shared.Assets.Maps;
using TT2Master.Shared.Models;

namespace TT2Master.Model.Clan
{
    /// <summary>
    /// Logic for ClanTrait
    /// </summary>
    public static class ClanTraitHandler
    {
        public static List<ClanTrait> ClanTraits { get; set; } = new List<ClanTrait>();

        #region Public Functions
        public static bool LoadItemsFromInfoFile()
        {
            try
            {
                OnLogMePlease?.Invoke("ClanTraitHandler", new InformationEventArgs("ClanTraitHandler.LoadItemsFromInfofile"));

                ClanTraits = AssetReader.GetInfoFile<ClanTrait, ClanTraitMap>(InfoFileEnum.RaidClanInfo);

                return true;
            }
            catch (Exception ex)
            {
                OnLogMePlease?.Invoke("ClanTraitHandler", new InformationEventArgs($"ClanTraitHandler: Exception at LoadItemsFromInfoFile: {ex.Message}\n\n {ex.Data}\n\n"));
                OnProblemHaving?.Invoke("ClanTraitHandler", new CustErrorEventArgs(ex));
                return false;
            }
        }
        /// <summary>
        /// Returns the clantrait that matches the current clan exp
        /// </summary>
        /// <returns></returns>
        public static ClanTrait GetClanTrait()
        {
            if (ClanTraits == null)
            {
                LoadItemsFromInfoFile();
            }

            if (ClanTraits.Count == 0)
            {
                LoadItemsFromInfoFile();
            }

            return ClanTraits.Where(x => x.ClanExp < (App.Save.ThisClan?.ClanRaidExp ?? 0)).OrderByDescending(x => x.ClanLevel).FirstOrDefault();
        }
        #endregion

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