using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using TT2Master.Helpers;
using TT2Master.Shared.Helper;
using TT2Master.Shared.Models;

namespace TT2Master.Model.Helpers
{
    /// <summary>
    /// Handles helper (hero) related stuff
    /// </summary>
    public static class HelpersFactory
    {
        #region Properties
        /// <summary>
        /// List of Helpers. To access the levels use <see cref="LoadHelperMasteries(JObject)"/>
        /// </summary>
        public static List<Helper> Helpers = new List<Helper>()
        {
            new Helper( 1, "H01"),
            new Helper( 2, "H02"),
            new Helper( 3, "H03"),
            new Helper( 4, "H04"),
            new Helper( 5, "H05"),
            new Helper( 6, "H06"),
            new Helper( 7, "H07"),
            new Helper( 8, "H08"),
            new Helper( 9, "H09"),
            new Helper(10, "H10"),
            new Helper(11, "H11"),
            new Helper(12, "H12"),
            new Helper(13, "H13"),
            new Helper(14, "H14"),
            new Helper(15, "H15"),
            new Helper(16, "H16"),
            new Helper(17, "H17"),
            new Helper(18, "H18"),
            new Helper(19, "H19"),
            new Helper(20, "H20"),
            new Helper(21, "H21"),
            new Helper(22, "H22"),
            new Helper(23, "H23"),
            new Helper(24, "H24"),
            new Helper(25, "H25"),
            new Helper(26, "H26"),
            new Helper(27, "H27"),
            new Helper(28, "H28"),
            new Helper(29, "H29"),
            new Helper(30, "H30"),
            new Helper(31, "H31"),
            new Helper(32, "H32"),
            new Helper(33, "H33"),
            new Helper(34, "H34"),
            new Helper(35, "H35"),
            new Helper(36, "H36"),
            new Helper(37, "H37"),
        };
        #endregion

        #region Public Methods
        /// <summary>
        /// Loads the levels given the helpermodel from savefile
        /// </summary>
        /// <param name="helperModel"></param>
        /// <returns></returns>
        public static bool LoadHelperMasteries(JObject helperModel)
        {
            try
            {
                // Get weapon levels and loop through
                var weapons = (JObject)helperModel.GetValue("allHelperWeaponLevels");

                foreach (var token in weapons)
                {
                    if (token.Key == "$type")
                    {
                        continue;
                    }

                    // Get index of item in list
                    int helperIndex = Helpers.FindIndex(x => x.JsonID == token.Key);

                    // Set level
                    Helpers[helperIndex].WeaponLevel = JfTypeConverter.ForceInt(token.Value);
                }

                // Get weapon levels and loop through
                var scrolls = (JObject)helperModel.GetValue("allHelperScrollLevels");

                foreach (var token in scrolls)
                {
                    if (token.Key == "$type")
                    {
                        continue;
                    }

                    // Get index of item in list
                    int helperIndex = Helpers.FindIndex(x => x.JsonID == token.Key);

                    // Set level
                    Helpers[helperIndex].ScrollLevel = JfTypeConverter.ForceInt(token.Value);
                }

                return true;

            }
            catch (Exception e)
            {
                OnProblemHaving?.Invoke("HelpersFactory", new CustErrorEventArgs(e));
                return false;
            }
        }

        /// <summary>
        /// Gets the total masteries level (sum of WeaponLevel and ScrollLevel)
        /// </summary>
        /// <returns></returns>
        public static int GetTotalMasteriesLevel()
        {
            int result = Helpers.Sum(x => x.WeaponLevel);
            result += Helpers.Sum(x => x.ScrollLevel);

            return result;
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
