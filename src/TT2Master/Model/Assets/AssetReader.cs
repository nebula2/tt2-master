using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TT2Master.Helpers;
using TT2Master.Shared;
using TT2Master.Shared.Assets;
using TT2Master.Shared.Assets.Maps;
using TT2Master.Shared.Models;

namespace TT2Master.Model.Assets
{
    public static class AssetReader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceName">identifier of resource without file ending</param>
        /// <returns></returns>
        public static List<T> GetInfoFile<T, U>(string resourceName) where T : class where U : ClassMap
        {
            try 
            {
                var target = AssetManager.GetVariousAssetFilename(resourceName);
                var result = AssetHandler.GetMappedEntitiesFromCsvFile<T, U>(target);

                return result;
            }
            catch (Exception e)
            {
                OnProblemHaving?.Invoke("AssetReader", new CustErrorEventArgs(e));
                return new List<T>();
            }
        }

        /// <summary>
        ///  Gets the Artifact info file async
        /// </summary>
        /// <param name="info"> which info do you want?</param>
        /// <returns></returns>
        public static List<T> GetInfoFile<T, U>(InfoFileEnum info) where T : class where U : ClassMap
        {
            try
            {
                var target = AssetManager.GetInfoFilename(info);
                var result = AssetHandler.GetMappedEntitiesFromCsvFile<T, U>(target);

                return result;
            }
            catch (Exception e)
            {
                OnProblemHaving?.Invoke("InfoFileHandler", new CustErrorEventArgs(e));
                return new List<T>();
            }
        }

        public static bool InitializeAssetNameHeaderMappings()
        {
            try
            {
                var target = AssetManager.GetVariousAssetFilename("assetNameMapping");

                var result = AssetHandler.GetMappedEntitiesFromCsvFile<AssetNameMapping, AssetNameMappingMap>(target);
                AssetMapNameProvider.AddAdditionalMappings(result);

                return true;
            }
            catch (Exception ex)
            {
                OnProblemHaving?.Invoke("AssetReader.InitializeAssetNameHeaderMappings", new CustErrorEventArgs(ex));
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
        #endregion
    }
}
