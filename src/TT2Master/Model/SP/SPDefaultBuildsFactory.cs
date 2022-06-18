using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TT2Master.Helpers;
using TT2Master.Loggers;
using TT2Master.Resources;
using TT2Master.Shared.Helper;

namespace TT2Master
{
    /// <summary>
    /// A Class to handle default <see cref="SPBuild"/>s
    /// </summary>
    [Obsolete("-.-")]
    public static class SPDefaultBuildsFactory
    {
        /// <summary>
        /// True if the Default Builds List has been initialized
        /// </summary>
        private static bool _isListInitialized = false;

        /// <summary>
        /// Checks if all default builds exist async. If so, this returnes true
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> AllDefaultBuildsExistAsync()
        {
            try
            {
                if (!_isListInitialized)
                {
                    _isListInitialized = InitializeDefaultBuildList(); ;
                }

                OnProgressMade?.Invoke("SpDefaultBuildsFactory", new InformationEventArgs(AppResources.RetrievingStoredDefaultBuilds));
                var builds = await App.DBRepo.GetDefaultSPBuildsAsync();

                //if there are no stored builds, then there are no stored builds.
                if (builds.Count == 0)
                {
                    return false;
                }

                OnProgressMade?.Invoke("SpDefaultBuildsFactory", new InformationEventArgs(AppResources.CheckingDefaultBuilds));

                bool allBuildsStored = true; //determines wheather all default builds are there

                //Check each build
                foreach (var build in _defaultBuilds)
                {
                    OnProgressMade?.Invoke("SpDefaultBuildsFactory", new InformationEventArgs(string.Format(AppResources.LookingForX, build.ID)));

                    //check if build exists
                    allBuildsStored = builds.Where(x => x.ID == build.ID).Count() > 0;

                    //return quickly if even one build is not found
                    if (!allBuildsStored)
                    {
                        return false;
                    }
                }

                return allBuildsStored;
            }

            catch (Exception e)
            {
                OnProblemHaving?.Invoke("SPDefaultBuildsFactory", new CustErrorEventArgs(e));
                return false;
            }
        }

        /// <summary>
        /// Recreates all default builds by deleting the old and saving <see cref="_defaultBuilds"/>
        /// </summary>
        /// <returns></returns>
        [Obsolete("Why dafuq is this crap even here?")]
        public static async Task<bool> RecreateAllDefaultBuildsAsync()
        {
            try
            {
                if (!_isListInitialized)
                {
                    _isListInitialized = InitializeDefaultBuildList();
                }

                OnProgressMade?.Invoke("SpDefaultBuildsFactory", new InformationEventArgs(AppResources.RetrievingStoredDefaultBuilds));
                //Check if StandardBuilds are stored in Database
                var builds = await App.DBRepo.GetDefaultSPBuildsAsync();

                #region Delete old
                foreach (var build in builds)
                {
                    OnProgressMade?.Invoke("SpDefaultBuildsFactory", new InformationEventArgs(string.Format(AppResources.DeletingX, build.ID)));

                    //get milestones and delete them
                    var ms = await App.DBRepo.GetAllSPBuildMilestoneAsync(build.ID);

                    foreach (var bms in ms)
                    {
                        OnProgressMade?.Invoke("SpDefaultBuildsFactory", new InformationEventArgs(string.Format(AppResources.DeletingX, build.ID) + " - " + string.Format(AppResources.MilestoneX, bms.Milestone)));

                        //Get milestoneitems and delete them
                        var msi = await App.DBRepo.GetAllSPBuildMilestoneItemAsync(build.ID, bms.Milestone);

                        await App.DBRepo.DeleteSPBuildMilestoneItemByBuild(build.ID);

                        await App.DBRepo.DeleteSPBuildMilestoneByName(bms.Identifier);
                    }

                    //Delete Build
                    await App.DBRepo.DeleteSPBuildByName(build.ID);
                }
                #endregion

                #region Saving new
                foreach (var build in _defaultBuilds)
                {
                    OnProgressMade?.Invoke("SpDefaultBuildsFactory", new InformationEventArgs(string.Format(AppResources.BeginSavingX, build.ID)));

                    foreach (var ms in build.Milestones)
                    {
                        OnProgressMade?.Invoke("SpDefaultBuildsFactory", new InformationEventArgs(string.Format(AppResources.SavingX, build.ID) + " - " + string.Format(AppResources.MilestoneX, ms.Milestone)));

                        int mssaved = await App.DBRepo.UpdateSPBuildMilestoneAsync(ms);
                        //Logger.WriteToLogFile($"{build.ID} milestone saved? {mssaved}");

                        int msisaved = 0;
                        msisaved += await App.DBRepo.AddNewSPBuildMilestoneItemsAsync(ms.MilestoneItems);

                        //Logger.WriteToLogFile($"{build.ID} milestoneitems saved? {mssaved}");
                    }

                    //save build
                    OnProgressMade?.Invoke("SpDefaultBuildsFactory", new InformationEventArgs(string.Format(AppResources.SavingX, build.ID)));
                    await App.DBRepo.UpdateSPBuildAsync(build);
                }
                #endregion

                return true;
            }
            catch (Exception e)
            {
                OnProgressMade?.Invoke("SPDefaultBuildsFactory", new InformationEventArgs(e.Message));
                return false;
            }
        }

        /// <summary>
        /// Initializes the default Build List
        /// </summary>
        private static bool InitializeDefaultBuildList()
        {
            OnProgressMade?.Invoke("SpDefaultBuildsFactory", new InformationEventArgs(AppResources.InitSpBuildList));

            foreach (string build in SPBuildCSV.Builds)
            {
                //Logger.WriteToLogFile($"getting buildCSV {build}");

                string[] stringSeparators = new string[] { "\r\n" };
                string[] buildArr = build.Split(stringSeparators, StringSplitOptions.None);

                //set what you can with low logic
                var sPBuild = new SPBuild(buildArr[0])
                {
                    OwnerName = buildArr[1],
                    Name = buildArr[2],
                    Editable = buildArr[3] == "TRUE" ? true : false,
                    Description = buildArr[4],
                    Version = buildArr[5]
                };

                //Logger.WriteToLogFile($"recreating build {sPBuild.ID}");

                //sixth row contains milestones
                string[] ms = buildArr[6].Split(';');
                for (int i = 1; i < ms.Count(); i++)
                {
                    sPBuild.Milestones.Add(new SPBuildMilestone(sPBuild.ID, i, JfTypeConverter.ForceInt(ms[i])));
                }

                //for each milestone go down the list
                foreach (var milestone in sPBuild.Milestones)
                {
                    //table starts at 7. go down the whole list
                    for (int i = 7; i < buildArr.Count(); i++)
                    {
                        //create the row
                        string[] row = buildArr[i].Split(';');

                        //add a milestoneItem and fill it
                        milestone.MilestoneItems.Add(new SPBuildMilestoneItem()
                        {
                            Build = milestone.Build,
                            Milestone = milestone.Milestone,
                            SkillID = row[0],
                            Amount = JfTypeConverter.ForceInt(row[milestone.Milestone])
                        });
                    }
                }

                _defaultBuilds.Add(sPBuild);
            }

            return true;
        }

        /// <summary>
        /// List of Default Builds that should exist
        /// </summary>
        private readonly static List<SPBuild> _defaultBuilds = new List<SPBuild>();

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
        public delegate void ProgressCarrier(object sender, InformationEventArgs e);

        /// <summary>
        /// Raised when <see cref="Initialize"/> made progress
        /// </summary>
        public static event ProgressCarrier OnProgressMade;

        #endregion
    }
}