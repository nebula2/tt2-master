using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TT2Master.Helpers;
using TT2Master.Shared.Assets;
using TT2Master.Shared.Helper;

namespace TT2Master.Model.Raid
{
    /// <summary>
    /// Clan raid result parser from raw text to data with crud
    /// </summary>
    public class ClanRaidResultParser
    {
        #region fields
        private int _raidId;
        private readonly DBRepository _dbRepo;

        public string RawData { get; set; }

        public List<ClanRaidResult> Results { get; private set; } = new List<ClanRaidResult>();
        #endregion

        #region private methods
        private bool ParseData()
        {
            bool isHeaderMapSuccessful = false;
            try
            {
                Results = AssetHandler.GetMappedEntitiesFromCsvString<ClanRaidResult, ClanRaidResultMap>(RawData);
                isHeaderMapSuccessful = true;
            }
            catch (Exception ex)
            {
                OnLogMePlease?.Invoke(this, new InformationEventArgs("Could not parse raid data with header names. " + ex.Message));
            }

            if (!isHeaderMapSuccessful)
            {
                try
                {
                    Results = AssetHandler.GetMappedEntitiesFromCsvString<ClanRaidResult, ClanRaidResultFallbackMap>(RawData);

                }
                catch (Exception ex)
                {
                    OnProblemHaving("ClanRaidResultParser.ParseData Fallback", new CustErrorEventArgs(ex));
                    return false;
                }
            }

            try
            {
                foreach (var item in Results)
                {
                    item.ParentId = _raidId;
                }

                OnLogMePlease?.Invoke(this, new InformationEventArgs($"Loaded {Results?.Count.ToString() ?? "null"} items"));

                return true;
            }
            catch (Exception ex)
            {
                OnProblemHaving("ClanRaidResultParser.ParseData", new CustErrorEventArgs(ex));
                return false;
            }
        }

        #endregion

        #region ctor
        public ClanRaidResultParser(DBRepository dbRepo, int clanRaidId, string rawData = null)
        {
            _dbRepo = dbRepo;
            _raidId = clanRaidId;
            RawData = rawData;
        }
        #endregion

        #region public functions
        /// <summary>
        /// reads in raw data and saves it in db related to given clan raid id
        /// </summary>
        /// <param name="clanRaidId">tt2master clan raid id</param>
        /// <param name="rawData">exported from tap titans to clipboard</param>
        /// <returns></returns>
        public async Task<bool> SaveRaidResultAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(RawData))
                {
                    return false;
                }

                // parse raw data
                if (!ParseData())
                {
                    return false;
                }

                // delete existing results
                await _dbRepo.DeleteClanRaidResultByParentId(_raidId);

                // save new results
                await _dbRepo.AddNewClanRaidResultListAsync(Results);

                return true;
            }
            catch (Exception ex)
            {
                OnProblemHaving("ClanRaidResultParser.SaveRaidResult", new CustErrorEventArgs(ex));

                return false;
            }
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
        public event ProgressCarrier OnLogMePlease;

        /// <summary>
        /// Delegate for occuring problems
        /// </summary>
        /// <param name="data"></param>
        public delegate void HoustonWeGotAProblem(object sender, CustErrorEventArgs e);

        /// <summary>
        /// Fires when this instance gets trouble
        /// </summary>
        public event HoustonWeGotAProblem OnProblemHaving;
        #endregion
    }
}
