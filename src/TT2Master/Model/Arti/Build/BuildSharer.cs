using System;
using System.IO;
using System.Threading.Tasks;
using TT2Master.Model.FilePicker;
using TT2Master.Shared.Helper;
using TT2Master.Shared.Models;
using Xamarin.Forms;

namespace TT2Master.Model.Arti
{
    /// <summary>
    /// A class that provides functionality to share builds
    /// </summary>
    public static class BuildSharer
    {
        #region Public
        /// <summary>
        /// Imports a build from file async
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> ImportBuildAsync()
        {
            //pick file - has to be stored in downloads folder because this plugin is fucking shit
            //var file = await CrossFilePicker.Current.PickFile();
            var file = await Xamarin.Forms.DependencyService.Get<Interfaces.IFilePicker>().PickFile();

            //check if picked
            if (file == null)
            {
                OnProblemHaving?.Invoke(new Exception("no file selected"));
                return false;
            }

            if (!file.FileName.Contains(".build"))
            {
                OnProblemHaving?.Invoke(new Exception("type does not match"));
                return false;
            }

            //Process file
            return await ProcessBuildFileAsync(file);
        }

        /// <summary>
        /// Exports a build to download directory of the device
        /// </summary>
        /// <returns></returns>
        public static bool ExportBuild(ArtifactBuild build, string filename)
        {
            if (build.Name[0] == '_')
            {
                OnProblemHaving?.Invoke(new Exception("default builds cannot be exported"));
                return false;
            }

            try
            {
                #region Prepare string to save
                #region Basics
                string buildStr = build.Name;
                buildStr += $"\n{(int)build.GoldSource}";
                #endregion

                #region Weights
                buildStr += $"\nWeights";
                foreach (var weight in build.CategoryWeights)
                {
                    buildStr += $"\n{weight.ArtifactId};{weight.ArtifactBehind.Name};{weight.Weight}";
                }
                #endregion

                #region Ignos
                buildStr += "\nIgnos";
                foreach (var igno in build.ArtsIgnored)
                {
                    if (igno.IsIgnored)
                    {
                        buildStr += $"\n{igno.ArtifactID};{igno.Name}";
                    }
                }
                #endregion
                #endregion

                //delete file if already exists
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }

                //Write file
                File.WriteAllText(filename, buildStr);

                return true;
            }
            catch (Exception e)
            {
                OnProblemHaving?.Invoke(new Exception(e.Message));
                return false;
            }
        }
        #endregion

        #region Helper
        /// <summary>
        /// Checks file and imports it to database async
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private static async Task<bool> ProcessBuildFileAsync(FileData file)
        {
            string filePath = file.FilePath ?? Path.Combine(DependencyService.Get<IDirectory>().GetDownloadPath(), file.FileName);


            #region Error handling
            if (!File.Exists(filePath))
            {
                OnProblemHaving(new Exception("File does not exist. Is it stored in Downloads folder? I can only process it from there T.T"));
                return false;
            }

            string[] buildStr = File.ReadAllLines(filePath);

            if (buildStr.Length == 0)
            {
                OnProblemHaving(new Exception("file is empty"));
                return false;
            }

            //read file
            var build = new ArtifactBuild();

            //no default build import
            if (buildStr[0][0] == '_')
            {
                OnProblemHaving(new Exception("default builds cannot be shared"));
                return false;
            }
            #endregion

            #region Basics
            //read basic build info
            try
            {
                //set name
                build.Name = buildStr[0]; //set Name
                build.Editable = true;
                build.GoldSource = (GoldType)JfTypeConverter.ForceInt(buildStr[1]);
            }
            catch (Exception e)
            {
                OnProblemHaving(new Exception($"Problem with file: {e.Message}"));
                return false;
            }
            #endregion

            #region Detail
            //Read details
            // are we reading weights? if not we read ignos
            bool readingWeights = true;

            for (int i = 2; i < buildStr.Length; i++)
            {
                try
                {
                    #region Set what we are reading
                    if (buildStr[i] == "Weights")
                    {
                        readingWeights = true;
                        continue;
                    }
                    else if (buildStr[i] == "Ignos")
                    {
                        readingWeights = false;
                        continue;
                    }
                    #endregion

                    string[] entry = buildStr[i].Split(';');
                    #region Read weights
                    if (readingWeights)
                    {

                        build.CategoryWeights.Add(new ArtifactWeight()
                        {
                            Build = build.Name,
                            ArtifactId = entry[0],
                            Weight = JfTypeConverter.ForceDoubleUniversal(entry[2])
                        });
                    }
                    #endregion

                    #region Read Ignos
                    else
                    {
                        build.ArtsIgnored.Add(new ArtifactBuildIgno()
                        {
                            Build = build.Name,
                            IsIgnored = true,
                            ArtifactID = entry[0]
                        });
                    }
                    #endregion
                }
                catch (Exception e)
                {
                    OnProblemHaving(new Exception($"Problem with file: {e.Message}"));
                    return false;
                }
            }
            #endregion

            #region Check if Build exists
            var maybeBuild = await App.DBRepo.GetArtifactBuildByName(build.Name);
            if (maybeBuild != null)
            {
                if (maybeBuild.Name == build.Name)
                {
                    OnProblemHaving?.Invoke(new Exception($"Build {build.Name} already exists"));
                    return false;
                }
            }
            #endregion

            #region Save build
            //save build
            _ = await App.DBRepo.UpdateArtifactBuildAsync(build);

            //Save weights
            int weightsSaved = 0;
            foreach (var item in build.CategoryWeights)
            {
                weightsSaved += await App.DBRepo.UpdateArtifactWeightAsync(item);
            }

            //Save ignos
            int ignosSaved = 0;
            foreach (var igno in build.ArtsIgnored)
            {
                //Only store ignored stuff
                if (igno.IsIgnored)
                {
                    ignosSaved += await App.DBRepo.UpdateArtifactBuildIgnoAsync(igno);
                }
            }
            #endregion

            return true;
        }
        #endregion

        #region events and delegates
        /// <summary>
        /// Delegate for occuring problems
        /// </summary>
        /// <param name="data"></param>
        public delegate void HoustonWeGotAProblem(Exception e);

        /// <summary>
        /// Fires when this instance gets trouble
        /// </summary>
        public static event HoustonWeGotAProblem OnProblemHaving;
        #endregion
    }
}