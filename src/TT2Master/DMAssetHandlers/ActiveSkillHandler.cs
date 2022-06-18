using System;
using System.Collections.Generic;
using TT2Master.Helpers;
using TT2Master.Loggers;
using TT2Master.Model.Assets;
using TT2Master.Shared;
using TT2Master.Shared.Assets.Maps;
using TT2Master.Shared.Models;

namespace TT2Master.Model.SP
{
    /// <summary>
    /// Handles skills which can be activated by the user
    /// </summary>
    public static class ActiveSkillHandler
    {
        #region Properties
        /// <summary>
        /// List of active skills
        /// </summary>
        public static List<ActiveSkill> ActiveSkills { get; set; } = new List<ActiveSkill>();
        #endregion

        #region Public Methods
        /// <summary>
        /// Loads active skills from Info file
        /// </summary>
        /// <returns></returns>
        public static bool LoadSkills()
        {
            try
            {
                OnLogMePlease?.Invoke("ActiveSkillHandler", new InformationEventArgs("SkillInfoHandler.LoadSkillsAsync"));
                ActiveSkills = AssetReader.GetInfoFile<ActiveSkill, ActiveSkillMap>(InfoFileEnum.ActiveSkillInfo);
                return true;
            }
            catch (Exception ex)
            {
                OnLogMePlease?.Invoke("ActiveSkillHandler", new InformationEventArgs($"ActiveSkillHandler: Exception at LoadSkills: {ex.Message}\n\n {ex.Data}\n\n"));
                OnProblemHaving?.Invoke("ActiveSkillHandler", new CustErrorEventArgs(ex));
                return false;
            }
        }
        #endregion

        #region events and delegates
        /// <summary>
        /// Delegate for occuring problems
        /// </summary>
        /// <param name="data"></param>
        public delegate void HoustonWeGotAProblem(object sender, CustErrorEventArgs e);

        /// <summary>
        /// Fires when this instance gets trouble
        /// </summary>
        public static event HoustonWeGotAProblem OnProblemHaving;

        /// <summary>
        /// Delegate for <see cref="OnProgressMade"/>
        /// </summary>
        /// <param name="message"></param>
        public delegate void LogCarrier(object sender, InformationEventArgs e);

        /// <summary>
        /// Raised when <see cref="Initialize"/> has something to log
        /// </summary>
        public static event LogCarrier OnLogMePlease;
        #endregion
    }
}