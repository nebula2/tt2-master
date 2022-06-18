using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TT2Master.Helpers;

namespace TT2Master
{
    /// <summary>
    /// A class that provides functionality to share sp builds
    /// </summary>
    public static class SPBuildSharer
    {
        #region Public
        /// <summary>
        /// Exports a SP build to download directory of the device
        /// </summary>
        /// <returns></returns>
        public static bool ExportBuild(SPBuild build, string filename)
        {
            try
            {
                #region Prepare string to save
                #region Basics
                string buildStr = build.ID;
                buildStr += $"\n{build.OwnerName}";
                buildStr += $"\n{build.Name}";
                buildStr += $"\n{build.Editable.ToString().ToUpper()}";
                buildStr += $"\n{build.Description}";
                buildStr += $"\n{build.Version}";
                #endregion

                #region SkillTree
                //Build first line. (LEFT TO RIGHT)
                buildStr += $"\nSkill,";
                foreach (var ms in build.Milestones)
                {
                    buildStr += $"{ms.SPReq},";
                }

                //Build skill lines

                //get list of Skills (TOP TO BOT)
                var skills = new List<string>();

                foreach (var msi in build.Milestones[0].MilestoneItems)
                {
                    skills.Add(msi.SkillID);
                }

                //Write a line for each skill
                for (int row = 0; row < skills.Count; row++)
                {
                    string rowString = $"\n{SkillInfoHandler.Skills.Where(x => x.TalentID == skills[row]).FirstOrDefault().Name},";

                    for (int column = 0; column < build.Milestones.Count; column++)
                    {
                        rowString += $"{build.Milestones[column].MilestoneItems.Where(x => x.SkillID == skills[row]).FirstOrDefault().Amount},";
                    }

                    buildStr += rowString;
                }

                #endregion
                #endregion

                #region save on device
                //delete file if already exists
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }

                //Write file
                File.WriteAllText(filename, buildStr);
                #endregion

                return true;
            }
            catch (Exception e)
            {
                OnProblemHaving?.Invoke("SPBuildSharer", new CustErrorEventArgs(e));
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
        #endregion
    }
}