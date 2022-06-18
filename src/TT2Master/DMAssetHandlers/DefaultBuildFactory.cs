using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TT2Master.Helpers;
using TT2Master.Loggers;
using TT2Master.Model.Assets;
using TT2Master.Resources;
using TT2Master.Shared.Assets.Maps;
using TT2Master.Shared.Models;

namespace TT2Master.Model.Arti
{
    /// <summary>
    /// A Class to handle default Builds.
    /// </summary>
    public static class DefaultBuildFactory
    {
        private static List<ArtifactBuild> _artifactBuilds;

        private static void LoadItemsFromInfofile()
        {
            try
            {
                OnLogMePlease?.Invoke("DefaultBuildFactory", new InformationEventArgs("DefaultBuildFactory.LoadItemsFromInfofile"));

                // Load items from amazon csv infofile
                var items = AssetReader.GetInfoFile<DefaultArtifactBuildEntry, DefaultArtifactBuildEntryMap>("defaultBuilds");

                _artifactBuilds = new List<ArtifactBuild>();

                var builds = items.Select(x => x.BuildId).Distinct();

                foreach (var item in builds)
                {
                    _artifactBuilds.Add(new ArtifactBuild
                    {
                        Name = item,
                        Editable = false,
                        IsDeleted = false,
                        GoldSource = GoldType.pHoM,
                        ArtsIgnored = new List<ArtifactBuildIgno>(),
                        CategoryWeights = new List<ArtifactWeight>(),
                    });
                }

                foreach (var build in _artifactBuilds)
                {
                    foreach (var item in items.Where(x => x.BuildId == build.Name).ToList())
                    {
                        build.CategoryWeights.Add(new ArtifactWeight()
                        {
                            Build = item.BuildId,
                            ArtifactId = item.ArtifactId,
                            Weight = item.DefaultCategory,
                        });
                    }
                }

            }
            catch (Exception ex)
            {
                OnProblemHaving?.Invoke("DefaultBuildFactory", new CustErrorEventArgs(ex));
            }
        }

        /// <summary>
        /// Recreates all default builds by deleting the old and saving <see cref="_defaultBuilds"/>
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> RecreateAllDefaultBuildsAsync()
        {
            try
            {
                OnProgressMade?.Invoke("DefaultBuildFactory", new InformationEventArgs(AppResources.RetrievingStoredDefaultBuilds));
                //Check if StandardBuilds are stored in Database
                var builds = await App.DBRepo.GetDefaultArtifactBuildsAsync();

                OnLogMePlease?.Invoke("DefaultBuildFactory", new InformationEventArgs($"RecreateDefaultBuilds: got {builds.Count} builds"));

                // load default builds from info file
                LoadItemsFromInfofile();

                #region Delete obsolete builds
                foreach (var item in builds)
                {
                    if (!_artifactBuilds.Select(x => x.Name).ToList().Contains(item.Name))
                    {
                        OnProgressMade?.Invoke("DefaultBuildFactory", new InformationEventArgs(string.Format(AppResources.DeletingX, item.Name)));
                        OnLogMePlease?.Invoke("DefaultBuildFactory", new InformationEventArgs(string.Format(AppResources.DeletingX, item.Name)));

                        //Get Ignos and weights
                        var ignos = await App.DBRepo.GetAllArtifactBuildIgnoAsync(item.Name);
                        var weights = await App.DBRepo.GetAllArtifactWeightAsync(item.Name);

                        //delete them
                        int ignoCount = await App.DBRepo.DeleteArtifactBuildIgnoByBuildAsync(item.Name);
                        int weightCount = await App.DBRepo.DeleteArtifactWeightByBuild(item.Name);

                        //Delete Build
                        int buildCount = await App.DBRepo.DeleteArtifactBuildByName(item.Name);

                        item.IsDeleted = true;

                        OnLogMePlease?.Invoke("DefaultBuildFactory", new InformationEventArgs($"RecreateDefaultBuilds: Deleted {buildCount} builds containing {ignoCount} ignos and {weightCount} weights"));
                    }
                }
                #endregion

                #region Create a complete list
                foreach (var item in _artifactBuilds)
                {
                    // add new builds
                    if (!builds.Select(x => x.Name).ToList().Contains(item.Name))
                    {
                        builds.Add(item);
                    }
                    // update weights on existing default build
                    else
                    {
                        var savedDefault = builds.Where(x => x.Name == item.Name).FirstOrDefault();

                        // just for the case
                        if (savedDefault == null)
                        {
                            OnLogMePlease?.Invoke("DefaultBuildFactory", new InformationEventArgs($"could not find savedDefault for {item.Name}"));
                            continue;
                        }

                        savedDefault.ArtsIgnored = item.ArtsIgnored;
                        savedDefault.CategoryWeights = item.CategoryWeights;
                    }
                }
                #endregion

                #region Saving updated
                foreach (var item in builds)
                {
                    OnProgressMade?.Invoke("DefaultBuildFactory", new InformationEventArgs(string.Format(AppResources.DeletingX, item.Name)));
                    OnLogMePlease?.Invoke("DefaultBuildFactory", new InformationEventArgs(string.Format(AppResources.DeletingX, item.Name)));

                    //Get Ignos and weights
                    var ignosDel = await App.DBRepo.GetAllArtifactBuildIgnoAsync(item.Name);
                    var weightsDel = await App.DBRepo.GetAllArtifactWeightAsync(item.Name);

                    //delete them
                    int ignoCountDel = await App.DBRepo.DeleteArtifactBuildIgnoByBuildAsync(item.Name);
                    int weightCountDel = await App.DBRepo.DeleteArtifactWeightByBuild(item.Name);

                    //Delete Build
                    int buildCountDel = await App.DBRepo.DeleteArtifactBuildByName(item.Name);

                    OnLogMePlease?.Invoke("DefaultBuildFactory", new InformationEventArgs($"RecreateDefaultBuilds: Deleted {buildCountDel} builds containing {ignoCountDel} ignos and {weightCountDel} weights"));

                    OnProgressMade?.Invoke("DefaultBuildFactory", new InformationEventArgs(string.Format(AppResources.SavingX, item.Name)));

                    //save weights
                    int weightCountIns = await App.DBRepo.AddNewArtifactWeightsAsync(item.CategoryWeights);

                    //save build
                    int buildCountIns = await App.DBRepo.UpdateArtifactBuildAsync(item);

                    OnLogMePlease?.Invoke("DefaultBuildFactory", new InformationEventArgs($"RecreateDefaultBuilds: Inserted {buildCountIns} builds containing {weightCountIns} weights"));
                }
                #endregion

                return true;
            }
            catch (Exception e)
            {
                OnLogMePlease?.Invoke("DefaultBuildFactory", new InformationEventArgs(e.Message));
                OnProblemHaving?.Invoke("DefaultBuildFactory", new CustErrorEventArgs(e));
                return false;
            }
        }

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