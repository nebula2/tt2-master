using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TT2Master.Loggers;
using TT2Master.Model.Arti;
using TT2Master.Model.Information;
using TT2Master.Model.Raid;
using TT2Master.Model.SP;
using TT2Master.Shared.Helper;

namespace TT2Master
{
    /// <summary>
    /// Provides Access to MySQL Database
    /// </summary>
    public class DBRepository
    {
        #region Properties and Members
        /// <summary>
        /// SQL-Connection
        /// </summary>
        private readonly SQLiteAsyncConnection _conn;

        /// <summary>
        /// his property provides a string which reports the last success or fail message from the database.
        /// </summary>
        public string StatusMessage { get; set; }
        #endregion

        #region Ctor
        /// <summary>
        /// The constructor initializes the underlying SQLite connection.
        /// </summary>
        /// <param name="dbPath"></param>
        public DBRepository(string dbPath)
        {
            try
            {
                _conn = new SQLiteAsyncConnection(dbPath,
                    SQLiteOpenFlags.Create |
                    SQLiteOpenFlags.FullMutex |
                    SQLiteOpenFlags.ReadWrite);

                var playerResult = _conn.CreateTableAsync<Player>().Result;
                var weights = _conn.CreateTableAsync<ArtifactWeight>().Result;
                var artIgnoresult = _conn.CreateTableAsync<ArtifactBuildIgno>().Result;
                var artifactresult = _conn.CreateTableAsync<ArtifactBuild>().Result;
                var sPBuildsMSresult = _conn.CreateTableAsync<SPBuildMilestone>().Result;
                var sPBuildsMSIresult = _conn.CreateTableAsync<SPBuildMilestoneItem>().Result;
                var sPBuildsresult = _conn.CreateTableAsync<SPBuild>().Result;
                var bannedPlayers = _conn.CreateTableAsync<BannedPlayer>().Result;
                var artOptSettings = _conn.CreateTableAsync<ArtOptSettings>().Result;
                var snapshot = _conn.CreateTableAsync<Snapshot>().Result;
                var msnapshotItems = _conn.CreateTableAsync<MemberSnapshotItem>().Result;
                var equipAdvSettings = _conn.CreateTableAsync<EquipAdvSettings>().Result;
                var expProps = _conn.CreateTableAsync<CsvExportProperty>().Result;
                var clanMsg = _conn.CreateTableAsync<ClanMessage>().Result;
                var spOptConfig = _conn.CreateTableAsync<SPOptConfiguration>().Result;
                var spOptSkillSetting = _conn.CreateTableAsync<SPOptSkillSetting>().Result;
                var clanRaid = _conn.CreateTableAsync<ClanRaid>().Result;
                var clanRaidResult = _conn.CreateTableAsync<ClanRaidResult>().Result;
                var raidStrategy = _conn.CreateTableAsync<RaidStrategy>().Result;
                var raidEnemyStrategy = _conn.CreateTableAsync<ClanRaidEnemyStrategy>().Result;
                var raidTolerance = _conn.CreateTableAsync<RaidTolerance>().Result;
                var raidToleranceRelationship = _conn.CreateTableAsync<ClanRaidToleranceRelationship>().Result;
                var announcement = _conn.CreateTableAsync<DbAnnouncement>().Result;
            }
            catch (Exception ex)
            {
                Error(ex);
            }
        }
        #endregion

        #region private methods
        private void Error(Exception ex)
        {
            StatusMessage = $"Failed to retrieve data. {ex.Message}\n{ex.Data}";

            try
            {
                //Logger.WriteToLogFile(StatusMessage);
            }
            // this could throw if native goes on error
            catch { }
        }
        #endregion

        #region public methods
        public async Task<bool> Close()
        {
            await _conn.CloseAsync();

            return true;
        }

        #region Player
        /// <summary>
        /// This method inserts a new <see cref="Player"/> record into the database.
        /// </summary>
        /// <param name="name"></param>
        public async Task AddNewPlayerAsync(Player player)
        {
            try
            {
                //basic validation to ensure a name was entered
                if (player == null)
                {
                    throw new Exception("Valid player required");
                }

                int result = await _conn.InsertAsync(player);

                StatusMessage = string.Format("{0} record(s) added [Name: {1})", result, player.PlayerId);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", player.PlayerId, ex.Message);
            }

        }

        /// <summary>
        /// his method returns all of the <see cref="Player"/> records from the database.
        /// </summary>
        /// <returns></returns>
        public async Task<List<Player>> GetAllPlayerAsync()
        {
            try
            {
                return await _conn.Table<Player>().ToListAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }

            return new List<Player>();
        }

        /// <summary>
        /// his method updates a <see cref="Player"/> record in the database.
        /// </summary>
        /// <returns></returns>
        /// <param name="player">The Player that needs to be updated</param>
        /// <param name="overrideAdId">pass true, when adid should be overriden</param>
        public async Task<int> UpdatePlayerAsync(Player player, bool overrideAdId = false)
        {
            if (player == null)
            {
                return 0;
            }

            try
            {
                var currentPlayer = await GetPlayerById(player.PlayerId);

                if (currentPlayer != null)
                {
                    //transfer AdId
                    if (!overrideAdId)
                    {
                        player.AdId = currentPlayer.AdId;
                    }

                    currentPlayer = player;
                    return await _conn.UpdateAsync(currentPlayer);
                }
                else
                {
                    return await _conn.InsertAsync(player);
                }
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to update data. {0}", ex.Message);
            }

            return 0;
        }

        /// <summary>
        /// Gets a <see cref="Player"/> by Name
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Player> GetPlayerById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return new Player();
            }

            try
            {
                var player = from p in _conn.Table<Player>()
                             where p.PlayerId == id
                             select p;

                return await player.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }

            return new Player();
        }

        /// <summary>
        /// Gets my <see cref="Player"/>
        /// </summary>
        /// <returns></returns>
        public async Task<Player> GetMyPlayer()
        {
            try
            {
                var player = from p in _conn.Table<Player>()
                             where p.IsMe == true
                             select p;

                return await player.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }

            return new Player();
        }
        #endregion

        #region ArtifactBuild
        /// <summary>
        /// This method inserts a new <see cref="ArtifactBuild"/> record into the database.
        /// </summary>
        /// <param name="name"></param>
        public async Task<int> AddNewArtifactBuildAsync(ArtifactBuild build)
        {
            try
            {
                if (build == null)
                {
                    throw new Exception("Valid build required");
                }

                if (string.IsNullOrWhiteSpace(build.Name))
                {
                    throw new Exception("Name cannot be null");
                }

                int result = await _conn.InsertAsync(build);

                StatusMessage = string.Format("{0} record(s) added [Name: {1})", result, build.Name);

                return result;
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }

        }

        /// <summary>
        /// This method returns all of the <see cref="ArtifactBuild"/> records from the database.
        /// </summary>
        /// <returns></returns>
        public async Task<List<ArtifactBuild>> GetAllArtifactBuildAsync()
        {
            try
            {
                return await _conn.Table<ArtifactBuild>().ToListAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return new List<ArtifactBuild>();
        }

        /// <summary>
        /// This method updates a <see cref="ArtifactBuild"/> record in the database.
        /// </summary>
        /// <returns></returns>
        /// <param name="player">The ArtifactBuild that needs to be updated</param>
        public async Task<int> UpdateArtifactBuildAsync(ArtifactBuild build)
        {
            if (build == null)
            {
                return 0;
            }

            try
            {
                var currentBuild = await GetArtifactBuildByName(build.Name);

                if (currentBuild != null)
                {
                    currentBuild = build;
                    return await _conn.UpdateAsync(currentBuild);
                }
                else
                {
                    return await _conn.InsertAsync(build);
                }
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return 0;
        }

        /// <summary>
        /// Gets a <see cref="ArtifactBuild"/> by Name
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ArtifactBuild> GetArtifactBuildByName(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return new ArtifactBuild();
            }

            try
            {
                var build = from p in _conn.Table<ArtifactBuild>()
                            where p.Name == id
                            select p;

                return await build.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return new ArtifactBuild();
        }

        /// <summary>
        /// This method returnes all default Artifact builds
        /// </summary>
        /// <returns></returns>
        public async Task<List<ArtifactBuild>> GetDefaultArtifactBuildsAsync()
        {
            try
            {
                return await _conn.Table<ArtifactBuild>().Where(x => x.Editable == false).ToListAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return new List<ArtifactBuild>();
        }

        /// <summary>
        /// Gets an array of all build names
        /// </summary>
        /// <returns></returns>
        public async Task<string[]> GetArtifactBuildNamesArrayAsync()
        {
            try
            {
                var build = from p in _conn.Table<ArtifactBuild>()
                            orderby p.Name
                            select p;

                var res = await build.ToListAsync();


                return (from p in res select p.Name).ToArray();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return new string[0];
        }

        /// <summary>
        /// Deletes a Build by name async
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteArtifactBuildByName(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                StatusMessage = "No id to delete";
                return 0;
            }

            try
            {
                var deleteArtifactBuild = from build in _conn.Table<ArtifactBuild>()
                                          where build.Name == id
                                          select build;

                var dbBuild = await deleteArtifactBuild.FirstOrDefaultAsync();

                if (dbBuild == null)
                {
                    StatusMessage = $"Could not find Build {id}";
                    return 0;
                }

                return await _conn.DeleteAsync(dbBuild);
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }
        }
        #endregion

        #region ArtifactBuildWeight
        /// <summary>
        /// Deletes a Build weight by name async
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteArtifactWeightByName(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                StatusMessage = "No id to delete";
                return 0;
            }

            try
            {
                var deleteArtifactWeight = from weight in _conn.Table<ArtifactWeight>()
                                           where weight.BuildAndArtifact == id
                                           select weight;

                var dbBuild = await deleteArtifactWeight.FirstOrDefaultAsync();

                if (dbBuild == null)
                {
                    StatusMessage = $"Could not find Weight {id}";
                    return 0;
                }

                return await _conn.DeleteAsync(dbBuild);
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }
        }

        /// <summary>
        /// Deletes a Build weight by build async
        /// </summary>
        /// <param name="build"></param>
        /// <returns></returns>
        public async Task<int> DeleteArtifactWeightByBuild(string build)
        {
            if (string.IsNullOrWhiteSpace(build))
            {
                StatusMessage = "No id to delete";
                return 0;
            }

            try
            {
                var deleteArtifactWeight = from weight in _conn.Table<ArtifactWeight>()
                                           where weight.Build == build
                                           select weight;

                if (deleteArtifactWeight == null)
                {
                    StatusMessage = $"Could not find Weight {build}";
                    return 0;
                }

                return await deleteArtifactWeight.DeleteAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }
        }

        /// <summary>
        /// This method inserts a new <see cref="ArtifactWeight"/> record into the database.
        /// </summary>
        /// <param name="name"></param>
        public async Task<int> AddNewArtifactWeightAsync(ArtifactWeight item)
        {
            try
            {
                if (item == null)
                {
                    throw new Exception("Valid weight required");
                }

                if (string.IsNullOrWhiteSpace(item.Build) || string.IsNullOrWhiteSpace(item.ArtifactId))
                {
                    throw new Exception("Build and Category cannot be null");
                }

                int result = await _conn.InsertAsync(item);

                StatusMessage = string.Format("{0} record(s) added [Name: {1})", result, item.Build);

                return result;
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", item.Build, ex.Message);
                return 0;
            }

        }

        /// <summary>
        /// This method inserts a list of <see cref="ArtifactWeight"/> record into the database.
        /// </summary>
        /// <param name="name"></param>
        public async Task<int> AddNewArtifactWeightsAsync(List<ArtifactWeight> items)
        {
            try
            {
                if (items == null)
                {
                    throw new Exception("Valid weight required");
                }

                int result = await _conn.InsertAllAsync(items);

                StatusMessage = string.Format("{0} record(s) added [Name: {1})", result, items.Count);

                return result;
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", items.Count, ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// This method returns all of the <see cref="ArtifactWeight"/> records from the database
        /// for a specific build name
        /// </summary>
        /// <returns></returns>
        public async Task<List<ArtifactWeight>> GetAllArtifactWeightAsync(string buildname)
        {
            try
            {
                return await _conn.Table<ArtifactWeight>().Where(x => x.Build == buildname).ToListAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return new List<ArtifactWeight>();
        }

        /// <summary>
        /// This method updates a <see cref="ArtifactWeight"/> record in the database.
        /// </summary>
        /// <returns></returns>
        /// <param name="player">The ArtifactBuild that needs to be updated</param>
        public async Task<int> UpdateArtifactWeightAsync(ArtifactWeight buildweight)
        {
            if (buildweight == null)
            {
                return 0;
            }

            try
            {
                var currentBuild = await GetArtifactWeightByName(buildweight.Build, buildweight.ArtifactId);

                if (currentBuild != null)
                {
                    currentBuild = buildweight;
                    return await _conn.UpdateAsync(currentBuild);
                }
                else
                {
                    return await _conn.InsertAsync(buildweight);
                }
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return 0;
        }

        /// <summary>
        /// This method updates a list of <see cref="ArtifactWeight"/> record in the database.
        /// </summary>
        /// <returns></returns>
        public async Task<int> UpdateArtifactWeightAsync(List<ArtifactWeight> buildweights)
        {
            if (buildweights == null)
            {
                return 0;
            }

            try
            {
                return await _conn.InsertAllAsync(buildweights);
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return 0;
        }

        /// <summary>
        /// Gets a <see cref="ArtifactWeight"/> by Name
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ArtifactWeight> GetArtifactWeightByName(string build, string artifact)
        {
            if (string.IsNullOrWhiteSpace(build) || string.IsNullOrWhiteSpace(artifact))
            {
                return new ArtifactWeight();
            }

            try
            {
                var buildweight = from p in _conn.Table<ArtifactWeight>()
                                  where p.Build == build && p.ArtifactId == artifact
                                  select p;

                return await buildweight.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return new ArtifactWeight();
        }
        #endregion

        #region ArtifactBuildIgno
        /// <summary>
        /// Deletes a <see cref="ArtifactBuildIgno"/> by name async
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteArtifactBuildIgnoByNameAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                StatusMessage = "No id to delete";
                return 0;
            }

            try
            {
                var del = from s in _conn.Table<ArtifactBuildIgno>()
                          where s.BuildArt == id
                          select s;

                var dbBuild = await del.FirstOrDefaultAsync();

                if (dbBuild == null)
                {
                    StatusMessage = $"Could not find {id}";
                    return 0;
                }

                return await _conn.DeleteAsync(dbBuild);
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }
        }

        /// <summary>
        /// Deletes a <see cref="ArtifactBuildIgno"/> by build async
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteArtifactBuildIgnoByBuildAsync(string build)
        {
            if (string.IsNullOrWhiteSpace(build))
            {
                StatusMessage = "No id to delete";
                return 0;
            }

            try
            {
                var del = from s in _conn.Table<ArtifactBuildIgno>()
                          where s.Build == build
                          select s;

                if (del == null)
                {
                    StatusMessage = $"Could not find {build}";
                    return 0;
                }

                return await del.DeleteAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }
        }

        /// <summary>
        /// This method inserts a new <see cref="ArtifactBuildIgno"/> record into the database.
        /// </summary>
        /// <param name="name"></param>
        public async Task<int> AddNewArtifactBuildIgnoAsync(ArtifactBuildIgno a)
        {
            try
            {
                if (a == null)
                {
                    throw new Exception("null passed");
                }

                if (string.IsNullOrWhiteSpace(a.Build) || string.IsNullOrWhiteSpace(a.ArtifactID))
                {
                    throw new Exception("Build and ArtifactID cannot be null");
                }

                int result = await _conn.InsertAsync(a);

                StatusMessage = string.Format("{0} record(s) added [Name: {1})", result, a.BuildArt);

                return result;
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", a.BuildArt, ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// This method returns all of the <see cref="ArtifactBuildIgno"/> records from the database
        /// for a specific build name
        /// </summary>
        /// <returns></returns>
        public async Task<List<ArtifactBuildIgno>> GetAllArtifactBuildIgnoAsync(string buildname)
        {
            try
            {
                var tmp = await _conn.Table<ArtifactBuildIgno>().Where(x => x.Build == buildname).ToListAsync();

                //Set db arts ignored
                foreach (var item in tmp)
                {
                    item.IsIgnored = true;
                    //item.Name = ArtifactDict.ArtifactNames[item.ArtifactID];
                }

                //fill remaining arts as not ignored
                foreach (var art in ArtifactHandler.Artifacts)
                {
                    if (tmp.Where(x => x.ArtifactID == art.ID).Count() == 0)
                    {
                        var igno = new ArtifactBuildIgno(buildname, art.ID, false)
                        {
                            //Name = ArtifactDict.ArtifactNames[art.ID]
                        };

                        tmp.Add(igno);
                    }
                }

                return tmp;
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return new List<ArtifactBuildIgno>();
        }

        /// <summary>
        /// This method updates a <see cref="ArtifactBuildIgno"/> record in the database.
        /// </summary>
        /// <returns></returns>
        public async Task<int> UpdateArtifactBuildIgnoAsync(ArtifactBuildIgno s)
        {
            if (s == null)
            {
                return 0;
            }

            if (s.IsIgnored == false)
            {
                return 0;
            }

            {
                try
                {
                    var i = await GetArtifactBuildIgnoByNameAsync(s.Build, s.ArtifactID);

                    if (i != null)
                    {
                        i = s;
                        return await _conn.UpdateAsync(i);
                    }
                    else
                    {
                        return await _conn.InsertAsync(s);
                    }
                }
                catch (Exception ex)
                {
                    Error(ex);
                }
            }

            return 0;
        }

        /// <summary>
        /// Gets a <see cref="ArtifactBuildIgno"/> by Name
        /// </summary>
        /// <returns></returns>
        public async Task<ArtifactBuildIgno> GetArtifactBuildIgnoByNameAsync(string build, string artId)
        {
            if (string.IsNullOrWhiteSpace(build) || string.IsNullOrWhiteSpace(artId))
            {
                return new ArtifactBuildIgno();
            }

            try
            {
                var v = from p in _conn.Table<ArtifactBuildIgno>()
                        where p.Build == build && p.ArtifactID == artId
                        select p;

                return await v.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return new ArtifactBuildIgno();
        }
        #endregion

        #region SPBuild
        /// <summary>
        /// This method inserts a new <see cref="SPBuild"/> record into the database.
        /// </summary>
        /// <param name="name"></param>
        public async Task<int> AddNewSPBuildAsync(SPBuild build)
        {
            try
            {
                if (build == null)
                {
                    throw new Exception("Valid build required");
                }

                if (string.IsNullOrWhiteSpace(build.ID))
                {
                    throw new Exception("Name cannot be null");
                }

                int result = await _conn.InsertAsync(build);

                StatusMessage = string.Format("{0} record(s) added [Name: {1})", result, build.ID);

                return result;
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }

        }

        /// <summary>
        /// This method returns all of the <see cref="SPBuild"/> records from the database.
        /// </summary>
        /// <returns></returns>
        public async Task<List<SPBuild>> GetAllSPBuildAsync()
        {
            try
            {
                return await _conn.Table<SPBuild>().ToListAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return new List<SPBuild>();
        }

        /// <summary>
        /// Gets an array of all build names
        /// </summary>
        /// <returns></returns>
        public async Task<string[]> GetSPBuildNamesArrayAsync()
        {
            try
            {
                var build = from p in _conn.Table<SPBuild>()
                            orderby p.ID
                            select p;

                var res = await build.ToListAsync();


                return (from p in res select p.ID).ToArray() ?? new string[] { "" };
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return new string[] { "" };
        }

        /// <summary>
        /// This method updates a <see cref="SPBuild"/> record in the database.
        /// </summary>
        /// <returns></returns>
        public async Task<int> UpdateSPBuildAsync(SPBuild build)
        {
            if (build == null)
            {
                return 0;
            }

            try
            {
                var currentBuild = await GetSPBuildByName(build.ID);

                if (currentBuild != null)
                {
                    currentBuild = build;
                    return await _conn.UpdateAsync(currentBuild);
                }
                else
                {
                    return await _conn.InsertAsync(build);
                }
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return 0;
        }

        /// <summary>
        /// Gets a <see cref="SPBuild"/> by Name
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<SPBuild> GetSPBuildByName(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return new SPBuild();
            }

            try
            {
                var build = from p in _conn.Table<SPBuild>()
                            where p.ID == id
                            select p;

                return await build.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return new SPBuild();
        }

        /// <summary>
        /// This method returnes all default <see cref="SPBuild"/>
        /// </summary>
        /// <returns></returns>
        public async Task<List<SPBuild>> GetDefaultSPBuildsAsync()
        {
            try
            {
                return await _conn.Table<SPBuild>().Where(x => x.Editable == false).ToListAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return new List<SPBuild>();
        }

        /// <summary>
        /// Deletes a Build by name async
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteSPBuildByName(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                StatusMessage = "No id to delete";
                return 0;
            }

            try
            {
                var deleteBuild = from build in _conn.Table<SPBuild>()
                                  where build.ID == id
                                  select build;

                var dbBuild = await deleteBuild.FirstOrDefaultAsync();

                if (dbBuild == null)
                {
                    StatusMessage = $"Could not find Build {id}";
                    return 0;
                }

                return await _conn.DeleteAsync(dbBuild);
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }
        }
        #endregion

        #region SPBuildMileStone

        /// <summary>
        /// Deletes a <see cref="SPBuildMilestone"/> by name async
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteSPBuildMilestoneByName(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                StatusMessage = "No id to delete";
                return 0;
            }

            try
            {
                var delete = from weight in _conn.Table<SPBuildMilestone>()
                             where weight.Identifier == id
                             select weight;

                var dbBuild = await delete.FirstOrDefaultAsync();

                if (dbBuild == null)
                {
                    StatusMessage = $"Could not find Weight {id}";
                    return 0;
                }

                return await _conn.DeleteAsync(dbBuild);
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }
        }

        /// <summary>
        /// Deletes a <see cref="SPBuildMilestone"/> by build async
        /// </summary>
        /// <param name="build"></param>
        /// <returns></returns>
        public async Task<int> DeleteSPBuildMilestoneByBuild(string build)
        {
            if (string.IsNullOrWhiteSpace(build))
            {
                StatusMessage = "No id to delete";
                return 0;
            }

            try
            {
                var delete = from weight in _conn.Table<SPBuildMilestone>()
                             where weight.Build == build
                             select weight;

                if (delete == null)
                {
                    StatusMessage = $"Could not find Weight {build}";
                    return 0;
                }

                return await delete.DeleteAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }
        }

        /// <summary>
        /// This method inserts a new <see cref="SPBuildMilestone"/> record into the database.
        /// </summary>
        public async Task<int> AddNewSPBuildMilestoneAsync(SPBuildMilestone item)
        {
            try
            {
                if (item == null)
                {
                    throw new Exception("Valid item required");
                }

                if (string.IsNullOrWhiteSpace(item.Build))
                {
                    throw new Exception("Build cannot be null");
                }

                int result = await _conn.InsertAsync(item);

                StatusMessage = string.Format("{0} record(s) added [Name: {1})", result, item.Build);

                return result;
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", item.Build, ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// This method inserts a list of <see cref="SPBuildMilestone"/> record into the database.
        /// </summary>
        public async Task<int> AddNewSPBuildMilestonesAsync(List<SPBuildMilestone> items)
        {
            try
            {
                if (items == null)
                {
                    throw new Exception("Valid item required");
                }

                int result = await _conn.InsertAllAsync(items);

                StatusMessage = string.Format("{0} record(s) added [Name: {1})", result, items.Count);

                return result;
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", items.Count, ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// This method returns all of the <see cref="SPBuildMilestone"/> records from the database
        /// for a specific build name
        /// </summary>
        /// <returns></returns>
        public async Task<List<SPBuildMilestone>> GetAllSPBuildMilestoneAsync(string buildname)
        {
            try
            {
                return await _conn.Table<SPBuildMilestone>().Where(x => x.Build == buildname).ToListAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return new List<SPBuildMilestone>();
        }

        /// <summary>
        /// This method updates a <see cref="SPBuildMilestone"/> record in the database.
        /// </summary>
        /// <returns></returns>
        public async Task<int> UpdateSPBuildMilestoneAsync(SPBuildMilestone item)
        {
            if (item == null)
            {
                return 0;
            }

            try
            {
                var currentBuild = await GetSPBuildMilestoneByName(item.Build, item.Milestone);

                if (currentBuild != null)
                {
                    currentBuild = item;
                    return await _conn.UpdateAsync(currentBuild);
                }
                else
                {
                    return await _conn.InsertAsync(item);
                }
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return 0;
        }

        /// <summary>
        /// This method updates a list of <see cref="SPBuildMilestone"/> record in the database.
        /// </summary>
        /// <returns></returns>
        public async Task<int> UpdateSPBuildMilestoneAsync(List<SPBuildMilestone> items)
        {
            if (items == null)
            {
                return 0;
            }

            try
            {
                return await _conn.InsertAllAsync(items);
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return 0;
        }

        /// <summary>
        /// Gets a <see cref="SPBuildMilestone"/> by Identifier
        /// </summary>
        /// <returns></returns>
        public async Task<SPBuildMilestone> GetSPBuildMilestoneByName(string build, int milestone)
        {
            if (string.IsNullOrWhiteSpace(build))
            {
                return new SPBuildMilestone();
            }

            try
            {
                var buildweight = from p in _conn.Table<SPBuildMilestone>()
                                  where p.Build == build && p.Milestone == milestone
                                  select p;

                return await buildweight.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return new SPBuildMilestone();
        }
        #endregion

        #region SPBuildMileStoneItem
        /// <summary>
        /// Deletes a <see cref="SPBuildMilestoneItem"/> by name async
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteSPBuildMilestoneItemByName(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                StatusMessage = "No id to delete";
                return 0;
            }

            try
            {
                var deleteArtifactWeight = from weight in _conn.Table<SPBuildMilestoneItem>()
                                           where weight.Identifier == id
                                           select weight;

                var dbBuild = await deleteArtifactWeight.FirstOrDefaultAsync();

                if (dbBuild == null)
                {
                    StatusMessage = $"Could not find Weight {id}";
                    return 0;
                }

                return await _conn.DeleteAsync(dbBuild);
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }
        }

        /// <summary>
        /// Deletes a <see cref="SPBuildMilestoneItem"/> by build async
        /// </summary>
        /// <param name="build"></param>
        /// <returns></returns>
        public async Task<int> DeleteSPBuildMilestoneItemByBuild(string build)
        {
            if (string.IsNullOrWhiteSpace(build))
            {
                StatusMessage = "No id to delete";
                return 0;
            }

            try
            {
                var deleteArtifactWeight = from weight in _conn.Table<SPBuildMilestoneItem>()
                                           where weight.Build == build
                                           select weight;

                if (deleteArtifactWeight == null)
                {
                    StatusMessage = $"Could not find Weight {build}";
                    return 0;
                }

                return await deleteArtifactWeight.DeleteAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }
        }

        /// <summary>
        /// This method inserts a new <see cref="SPBuildMilestoneItem"/> record into the database.
        /// </summary>
        /// <param name="name"></param>
        public async Task<int> AddNewSPBuildMilestoneItemAsync(SPBuildMilestoneItem item)
        {
            try
            {
                if (item == null)
                {
                    throw new Exception("Valid weight required");
                }

                if (string.IsNullOrWhiteSpace(item.Build) || string.IsNullOrWhiteSpace(item.SkillID))
                {
                    throw new Exception("Build and skillId cannot be null");
                }

                int result = await _conn.InsertAsync(item);

                StatusMessage = string.Format("{0} record(s) added [Name: {1})", result, item.Build);

                return result;
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", item.Build, ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// This method inserts a list of <see cref="SPBuildMilestoneItem"/> record into the database.
        /// </summary>
        /// <param name="name"></param>
        public async Task<int> AddNewSPBuildMilestoneItemsAsync(List<SPBuildMilestoneItem> items)
        {
            try
            {
                if (items == null)
                {
                    throw new Exception("Valid weight required");
                }

                int result = await _conn.InsertAllAsync(items);

                StatusMessage = string.Format("{0} record(s) added [Name: {1})", result, items.Count);

                return result;
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", items.Count, ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// This method returns all of the <see cref="SPBuildMilestoneItem"/> records from the database
        /// for a specific build name
        /// </summary>
        /// <returns></returns>
        public async Task<List<SPBuildMilestoneItem>> GetAllSPBuildMilestoneItemAsync(string buildname, int milestone)
        {
            try
            {
                return await _conn.Table<SPBuildMilestoneItem>().Where(x => x.Build == buildname && x.Milestone == milestone).ToListAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return new List<SPBuildMilestoneItem>();
        }

        /// <summary>
        /// This method updates a <see cref="SPBuildMilestoneItem"/> record in the database.
        /// </summary>
        /// <returns></returns>
        public async Task<int> UpdateSPBuildMilestoneItemAsync(SPBuildMilestoneItem item)
        {
            if (item == null)
            {
                return 0;
            }

            try
            {
                var currentBuild = await GetSPBuildMilestoneItemByName(item.Build, item.Milestone, item.SkillID);

                if (currentBuild != null)
                {
                    currentBuild = item;
                    return await _conn.UpdateAsync(currentBuild);
                }
                else
                {
                    return await _conn.InsertAsync(item);
                }
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return 0;
        }

        /// <summary>
        /// This method updates a list of <see cref="SPBuildMilestoneItem"/> record in the database.
        /// </summary>
        /// <returns></returns>
        public async Task<int> UpdateSPBuildMilestoneItemAsync(List<SPBuildMilestoneItem> items)
        {
            if (items == null)
            {
                return 0;
            }

            try
            {
                return await _conn.InsertAllAsync(items);
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return 0;
        }

        /// <summary>
        /// Gets a <see cref="SPBuildMilestoneItem"/> by Identifier
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<SPBuildMilestoneItem> GetSPBuildMilestoneItemByName(string build, int milestone, string skillId)
        {
            if (string.IsNullOrWhiteSpace(build) || string.IsNullOrWhiteSpace(skillId))
            {
                return new SPBuildMilestoneItem();
            }

            try
            {
                var buildweight = from p in _conn.Table<SPBuildMilestoneItem>()
                                  where p.Build == build && p.SkillID == skillId && p.Milestone == milestone
                                  select p;

                return await buildweight.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return new SPBuildMilestoneItem();
        }
        #endregion

        #region Banned Player
        /// <summary>
        /// This method inserts a new <see cref="BannedPlayer"/> record into the database.
        /// </summary>
        /// <param name="name"></param>
        public async Task<int> AddBannedPlayerAsync(BannedPlayer item)
        {
            try
            {
                if (item == null)
                {
                    throw new Exception("Valid build required");
                }

                if (string.IsNullOrWhiteSpace(item.ID))
                {
                    throw new Exception("Name cannot be null");
                }

                int result = await _conn.InsertAsync(item);

                StatusMessage = string.Format("{0} record(s) added [Name: {1})", result, item.ID);

                return result;
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }

        }

        /// <summary>
        /// This method returns all of the <see cref="BannedPlayer"/> records from the database.
        /// </summary>
        /// <returns></returns>
        public async Task<List<BannedPlayer>> GetAllBannedPlayersAsync()
        {
            try
            {
                return await _conn.Table<BannedPlayer>().ToListAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return new List<BannedPlayer>();
        }

        /// <summary>
        /// Gets an array of all BannedPlayer IDs
        /// </summary>
        /// <returns></returns>
        public async Task<string[]> GetBannedPlayersArrayAsync()
        {
            try
            {
                var build = from p in _conn.Table<BannedPlayer>()
                            orderby p.ID
                            select p;

                var res = await build.ToListAsync();


                return (from p in res select p.ID).ToArray();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return new string[0];
        }

        /// <summary>
        /// This method updates a <see cref="BannedPlayer"/> record in the database.
        /// </summary>
        /// <returns></returns>
        public async Task<int> UpdateBannedPlayerdAsync(BannedPlayer item)
        {
            if (item == null)
            {
                return 0;
            }

            try
            {
                var currentBuild = await GetBannedPlayerByID(item.ID);

                if (currentBuild != null)
                {
                    currentBuild = item;
                    return await _conn.UpdateAsync(currentBuild);
                }
                else
                {
                    return await _conn.InsertAsync(item);
                }
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return 0;
        }

        /// <summary>
        /// Gets a <see cref="BannedPlayer"/> by Name
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<BannedPlayer> GetBannedPlayerByID(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return new BannedPlayer();
            }

            try
            {
                var build = from p in _conn.Table<BannedPlayer>()
                            where p.ID == id
                            select p;

                return await build.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return new BannedPlayer();
        }

        /// <summary>
        /// Deletes a BannedPlayer by ID async
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteBannedPlayerByID(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                StatusMessage = "No id to delete";
                return 0;
            }

            try
            {
                var deleteBuild = from build in _conn.Table<BannedPlayer>()
                                  where build.ID == id
                                  select build;

                var dbBuild = await deleteBuild.FirstOrDefaultAsync();

                if (dbBuild == null)
                {
                    StatusMessage = $"Could not find Build {id}";
                    return 0;
                }

                return await _conn.DeleteAsync(dbBuild);
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }
        }
        #endregion

        #region ArtOptSettings
        /// <summary>
        /// This method inserts a new <see cref="ArtOptSettings"/> record into the database.
        /// </summary>
        /// <param name="item"></param>
        public async Task<int> AddArtOptSettingsAsync(ArtOptSettings item)
        {
            try
            {
                if (item == null)
                {
                    throw new Exception("Valid item required");
                }

                int result = await _conn.InsertAsync(item);

                StatusMessage = string.Format("{0} record(s) added [Name: {1})", result, item.ID);

                return result;
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }

        }

        /// <summary>
        /// This method returns all of the <see cref="ArtOptSettings"/> records from the database.
        /// </summary>
        /// <returns></returns>
        public async Task<List<ArtOptSettings>> GetAllArtOptSettingsAsync()
        {
            try
            {
                return await _conn.Table<ArtOptSettings>().ToListAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return new List<ArtOptSettings>();
        }

        /// <summary>
        /// Gets an array of all ArtOptSettings IDs
        /// </summary>
        /// <returns></returns>
        public async Task<string[]> GetArtOptSettingsArrayAsync()
        {
            try
            {
                var build = from p in _conn.Table<ArtOptSettings>()
                            orderby p.ID
                            select p;

                var res = await build.ToListAsync();


                return (from p in res select p.ID).ToArray();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return new string[0];
        }

        /// <summary>
        /// This method updates a <see cref="ArtOptSettings"/> record in the database.
        /// </summary>
        /// <returns></returns>
        public async Task<int> UpdateArtOptSettingsAsync(ArtOptSettings item)
        {
            if (item == null)
            {
                return 0;
            }

            try
            {
                var currentItem = await GetArtOptSettingsByID(item.ID);

                if (currentItem != null)
                {
                    currentItem = item;
                    return await _conn.UpdateAsync(currentItem);
                }
                else
                {
                    return await _conn.InsertAsync(item);
                }
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return 0;
        }

        /// <summary>
        /// Gets a <see cref="ArtOptSettings"/> by Name
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ArtOptSettings> GetArtOptSettingsByID(string id)
        {
            try
            {
                var build = from p in _conn.Table<ArtOptSettings>()
                            where p.ID == id
                            select p;

                return await build.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return null;
        }

        /// <summary>
        /// Deletes a ArtOptSettings by ID async
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteArtOptSettingsID(string id)
        {
            try
            {
                var deleteBuild = from build in _conn.Table<ArtOptSettings>()
                                  where build.ID == id
                                  select build;

                var dbBuild = await deleteBuild.FirstOrDefaultAsync();

                if (dbBuild == null)
                {
                    StatusMessage = $"Could not find item {id}";
                    return 0;
                }

                return await _conn.DeleteAsync(dbBuild);
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }
        }
        #endregion

        #region Snapshots
        /// <summary>
        /// Deletes snapshots that are not fitting the currentClan
        /// </summary>
        /// <param name="currentClan"></param>
        /// <returns></returns>
        public async Task<int> DeleteOldClanData(string currentClan)
        {
            int rowsAffected = 0;
            try
            {
                var deleteBuild = from build in _conn.Table<Snapshot>()
                                  where build.Clanname != currentClan
                                  select build;

                var dbList = await deleteBuild.ToListAsync();

                if (dbList == null)
                {
                    StatusMessage = $"Could not find item {currentClan}";
                    return 0;
                }

                foreach (var item in dbList)
                {
                    rowsAffected += await DeleteMemberSnapshotItemBySId(item.ID);
                    rowsAffected += await _conn.DeleteAsync(item);
                }

            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// Deletes every item in the given list
        /// child items are included
        /// </summary>
        /// <param name="snapshots"></param>
        /// <returns></returns>
        public async Task<int> DeleteSnapshots(List<Snapshot> snapshots)
        {
            int rowsAffected = 0;
            try
            {
                foreach (var item in snapshots)
                {
                    rowsAffected += await DeleteMemberSnapshotItemBySId(item.ID);
                    rowsAffected += await _conn.DeleteAsync(item);
                }
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// This method gets the next <see cref="Snapshot"/>-ID that is free
        /// </summary>
        /// <param name="name"></param>
        public async Task<int> GetNextSnapshotIdAsync()
        {
            try
            {
                var item = await _conn.Table<Snapshot>().OrderByDescending(p => p.ID).FirstOrDefaultAsync();

                return item == null ? 0 : item.ID + 1;
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }

        }

        /// <summary>
        /// This method inserts a new <see cref="Snapshot"/> record into the database.
        /// </summary>
        /// <param name="name"></param>
        public async Task<int> AddNewSnapshotAsync(Snapshot item)
        {
            try
            {
                if (item == null)
                {
                    throw new Exception("Valid item required");
                }

                int result = await _conn.InsertAsync(item);

                StatusMessage = string.Format("{0} record(s) added [Name: {1})", result, item.ID);

                return result;
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }

        }

        /// <summary>
        /// This method returns all of the <see cref="Snapshot"/> records from the database.
        /// </summary>
        /// <returns></returns>
        public async Task<List<Snapshot>> GetAllSnapshotAsync()
        {
            try
            {
                return await _conn.Table<Snapshot>().OrderByDescending(x => x.ID).ToListAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return new List<Snapshot>();
        }

        /// <summary>
        /// This method returns all of the <see cref="Snapshot"/> records from the database for today.
        /// </summary>
        /// <returns></returns>
        public async Task<List<Snapshot>> GetTodaysSnapshotAsync()
        {
            try
            {
                var unfilteredItems = await _conn.Table<Snapshot>().ToListAsync();

                return unfilteredItems?.Where(p => p.Timestamp.Date == DateTime.Now.Date).ToList();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return new List<Snapshot>();
        }

        /// <summary>
        /// This method returns the last snapshot that was not taken today
        /// </summary>
        /// <returns></returns>
        public async Task<Snapshot> GetLastNotTodaySnapshotAsync()
        {
            try
            {
                var unfilteredItems = await _conn.Table<Snapshot>().ToListAsync();

                var sorted = unfilteredItems.OrderByDescending(x => x.ID).ToList();

                return sorted.Where(x => x.Timestamp.Date < DateTime.Now.Date).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return new Snapshot();
        }

        /// <summary>
        /// This method updates a <see cref="Snapshot"/> record in the database.
        /// </summary>
        /// <returns></returns>
        public async Task<int> UpdateSnapshotAsync(Snapshot item)
        {
            if (item == null)
            {
                return 0;
            }

            try
            {
                var currentBuild = await GetSnapshotByID(item.ID);

                if (currentBuild != null)
                {
                    currentBuild = item;
                    return await _conn.UpdateAsync(currentBuild);
                }
                else
                {
                    return await _conn.InsertAsync(item);
                }
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return 0;
        }

        /// <summary>
        /// Gets a <see cref="Snapshot"/> by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Snapshot> GetSnapshotByID(int id)
        {

            try
            {
                var build = from p in _conn.Table<Snapshot>()
                            where p.ID == id
                            select p;

                return await build.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return new Snapshot();
        }

        /// <summary>
        /// Deletes a <see cref="Snapshot"/> by id async
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteSnapshotByID(int id)
        {
            try
            {
                var deleteBuild = from build in _conn.Table<Snapshot>()
                                  where build.ID == id
                                  select build;

                var dbBuild = await deleteBuild.FirstOrDefaultAsync();

                if (dbBuild == null)
                {
                    StatusMessage = $"Could not find item {id}";
                    return 0;
                }

                return await _conn.DeleteAsync(dbBuild);
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }
        }
        #endregion

        #region MemberSnapshotItems

        public async Task<int> UpdateMemberSnapshotItemExternalIdAsync(string identifier, Guid externalReference)
        {
            try
            {
                StatusMessage = "";
                var current = await GetMemberSnapshotItemById(identifier);

                if (current == null)
                {
                    return 0;
                }

                current.ExternalId = externalReference;

                return await _conn.UpdateAsync(current);
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }
        }

        public async Task<List<MemberSnapshotItem>> GetSnapshotsToTransferAsync()
        {
            try
            {
                StatusMessage = "";
                var items = from m in _conn.Table<MemberSnapshotItem>()
                            where m.ExternalId == null
                            select m;

                return await items.ToListAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }
        }

        /// <summary>
        /// Deletes <see cref="MemberSnapshotItem"/> by Snapshot-ID async
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteMemberSnapshotItemBySId(int id)
        {
            try
            {
                var items = from weight in _conn.Table<MemberSnapshotItem>()
                                           where weight.SnapshotId == id
                                           select weight;

                if (items == null)
                {
                    StatusMessage = $"Could not find id {id}";
                    return 0;
                }

                if(await items.CountAsync() == 0)
                {
                    return 0;
                }

                int delCount = 0;

                foreach (var item in await items.ToListAsync())
                {
                    delCount += await _conn.DeleteAsync(item);
                }

                return delCount;
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }
        }

        /// <summary>
        /// Deletes a <see cref="MemberSnapshotItem"/> by id async
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteMemberSnapshotItemById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                StatusMessage = "No id to delete";
                return 0;
            }

            try
            {
                var deleteArtifactWeight = from weight in _conn.Table<MemberSnapshotItem>()
                                           where weight.Id == id
                                           select weight;

                if (deleteArtifactWeight == null)
                {
                    StatusMessage = $"Could not find id {id}";
                    return 0;
                }

                return await deleteArtifactWeight.DeleteAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }
        }

        /// <summary>
        /// This method inserts a new <see cref="MemberSnapshotItem"/> record into the database.
        /// </summary>
        /// <param name="name"></param>
        public async Task<int> AddNewMemberSnapshotItemAsync(MemberSnapshotItem item)
        {
            try
            {
                if (item == null)
                {
                    throw new Exception("Valid item required");
                }

                if (string.IsNullOrWhiteSpace(item.Id))
                {
                    throw new Exception("Id cannot be null");
                }

                int result = await _conn.InsertAsync(item);

                StatusMessage = string.Format("{0} record(s) added [Name: {1})", result, item.Id);

                return result;
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", item.Id, ex.Message);
                return 0;
            }

        }

        /// <summary>
        /// This method inserts a list of <see cref="MemberSnapshotItem"/> records into the database.
        /// </summary>
        /// <param name="name"></param>
        public async Task<int> AddNewMemberSnapshotItemListAsync(List<MemberSnapshotItem> items)
        {
            try
            {
                if (items == null)
                {
                    throw new Exception("Valid item required");
                }

                int result = await _conn.InsertAllAsync(items);

                StatusMessage = string.Format("{0} record(s) added [Name: {1})", result, items.Count);

                return result;
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", items.Count, ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// This method returns all of the <see cref="MemberSnapshotItem"/> records from the database
        /// for a specific snapshot
        /// </summary>
        /// <returns></returns>
        public async Task<List<MemberSnapshotItem>> GetAllMemberSnapshotItemAsync(int snapshotId)
        {
            try
            {
                return await _conn.Table<MemberSnapshotItem>().Where(x => x.SnapshotId == snapshotId).ToListAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return new List<MemberSnapshotItem>();
        }

        /// <summary>
        /// This method updates a <see cref="MemberSnapshotItem"/> record in the database.
        /// </summary>
        /// <returns></returns>
        /// <param name="player">The ArtifactBuild that needs to be updated</param>
        public async Task<int> UpdateMemberSnapshotItemAsync(MemberSnapshotItem item)
        {
            if (item == null)
            {
                return 0;
            }

            try
            {
                var currentBuild = await GetMemberSnapshotItemById(item.Id);

                if (currentBuild != null)
                {
                    currentBuild = item;
                    return await _conn.UpdateAsync(currentBuild);
                }
                else
                {
                    return await _conn.InsertAsync(item);
                }
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return 0;
        }

        /// <summary>
        /// Gets a <see cref="MemberSnapshotItem"/> by Name
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<MemberSnapshotItem> GetMemberSnapshotItemById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return new MemberSnapshotItem();
            }

            try
            {
                var buildweight = from p in _conn.Table<MemberSnapshotItem>()
                                  where p.Id == id
                                  select p;

                return await buildweight.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return new MemberSnapshotItem();
        }
        #endregion

        #region EquipAdvSettings
        /// <summary>
        /// This method inserts a new <see cref="EquipAdvSettings"/> record into the database.
        /// </summary>
        /// <param name="item"></param>
        public async Task<int> AddEquipAdvSettingsAsync(EquipAdvSettings item)
        {
            try
            {
                if (item == null)
                {
                    throw new Exception("Valid item required");
                }

                int result = await _conn.InsertAsync(item);

                StatusMessage = string.Format("{0} record(s) added [Name: {1})", result, item.ID);

                return result;
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }

        }

        /// <summary>
        /// This method returns all of the <see cref="EquipAdvSettings"/> records from the database.
        /// </summary>
        /// <returns></returns>
        public async Task<List<EquipAdvSettings>> GetAllEquipAdvSettingsAsync()
        {
            try
            {
                return await _conn.Table<EquipAdvSettings>().ToListAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return new List<EquipAdvSettings>();
        }

        /// <summary>
        /// Gets an array of all <see cref="EquipAdvSettings"/> IDs
        /// </summary>
        /// <returns></returns>
        public async Task<string[]> GetEquipAdvSettingsArrayAsync()
        {
            try
            {
                var build = from p in _conn.Table<EquipAdvSettings>()
                            orderby p.ID
                            select p;

                var res = await build.ToListAsync();


                return (from p in res select p.ID).ToArray();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return new string[0];
        }

        /// <summary>
        /// This method updates a <see cref="EquipAdvSettings"/> record in the database.
        /// </summary>
        /// <returns></returns>
        public async Task<int> UpdateEquipAdvSettingsAsync(EquipAdvSettings item)
        {
            if (item == null)
            {
                return 0;
            }

            try
            {
                var currentItem = await GetEquipAdvSettingsByID(item.ID);

                if (currentItem != null)
                {
                    currentItem = item;
                    return await _conn.UpdateAsync(currentItem);
                }
                else
                {
                    return await _conn.InsertAsync(item);
                }
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return 0;
        }

        /// <summary>
        /// Gets a <see cref="EquipAdvSettings"/> by Name
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<EquipAdvSettings> GetEquipAdvSettingsByID(string id)
        {
            try
            {
                var build = from p in _conn.Table<EquipAdvSettings>()
                            where p.ID == id
                            select p;

                return await build.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return null;
        }

        /// <summary>
        /// Deletes a <see cref="EquipAdvSettings"/> by ID async
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteEquipAdvSettingsID(string id)
        {
            try
            {
                var deleteBuild = from build in _conn.Table<EquipAdvSettings>()
                                  where build.ID == id
                                  select build;

                var dbBuild = await deleteBuild.FirstOrDefaultAsync();

                if (dbBuild == null)
                {
                    StatusMessage = $"Could not find item {id}";
                    return 0;
                }

                return await _conn.DeleteAsync(dbBuild);
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }
        }
        #endregion

        #region ExportProperties
        /// <summary>
        /// Deletes a Build weight by identifier async
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteCsvExportPropertyByName(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                StatusMessage = "No id to delete";
                return 0;
            }

            try
            {
                var deleteArtifactWeight = from weight in _conn.Table<CsvExportProperty>()
                                           where weight.Identifier == id
                                           select weight;

                var dbBuild = await deleteArtifactWeight.FirstOrDefaultAsync();

                if (dbBuild == null)
                {
                    StatusMessage = $"Could not find Weight {id}";
                    return 0;
                }

                return await _conn.DeleteAsync(dbBuild);
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }
        }

        /// <summary>
        /// Deletes csvexport by build async
        /// </summary>
        /// <param name="build"></param>
        /// <returns></returns>
        public async Task<int> DeleteCsvExportPropertiesByBuild(string build)
        {
            if (string.IsNullOrWhiteSpace(build))
            {
                StatusMessage = "No id to delete";
                return 0;
            }

            try
            {
                var deleteArtifactWeight = from weight in _conn.Table<CsvExportProperty>()
                                           where weight.ExportReference == build
                                           select weight;

                if (deleteArtifactWeight == null)
                {
                    StatusMessage = $"Could not find Weight {build}";
                    return 0;
                }

                return await deleteArtifactWeight.DeleteAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }
        }

        /// <summary>
        /// This method inserts a new <see cref="CsvExportProperty"/> record into the database.
        /// </summary>
        /// <param name="name"></param>
        public async Task<int> AddNewCsvExportPropertyAsync(CsvExportProperty item)
        {
            try
            {
                if (item == null)
                {
                    throw new Exception("Valid weight required");
                }

                if (string.IsNullOrWhiteSpace(item.Identifier))
                {
                    throw new Exception("Identifier cannot be null");
                }

                int result = await _conn.InsertAsync(item);

                StatusMessage = string.Format("{0} record(s) added [Name: {1})", result, item.Identifier);

                return result;
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", item.Identifier, ex.Message);
                return 0;
            }

        }

        /// <summary>
        /// This method inserts a list of <see cref="ArtifactBuildWeight"/> record into the database.
        /// </summary>
        /// <param name="name"></param>
        public async Task<int> AddNewCsvExportPropertiesAsync(List<CsvExportProperty> items)
        {
            try
            {
                if (items == null)
                {
                    throw new Exception("Valid weight required");
                }

                int result = await _conn.InsertAllAsync(items);

                StatusMessage = string.Format("{0} record(s) added [Name: {1})", result, items.Count);

                return result;
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", items.Count, ex.Message);
                return 0;
            }

        }

        /// <summary>
        /// This method returns all of the <see cref="CsvExportProperty"/> records from the database
        /// for a specific build name
        /// </summary>
        /// <returns></returns>
        public async Task<List<CsvExportProperty>> GetAllCsvExportPropertiesAsync(string buildname)
        {
            try
            {
                return await _conn.Table<CsvExportProperty>().Where(x => x.ExportReference == buildname).ToListAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return new List<CsvExportProperty>();
        }

        /// <summary>
        /// This method updates a <see cref="CsvExportProperty"/> record in the database.
        /// </summary>
        /// <returns></returns>
        /// <param name="player">The ArtifactBuild that needs to be updated</param>
        public async Task<int> UpdateCsvExportPropertyAsync(CsvExportProperty buildweight)
        {
            if (buildweight == null)
            {
                return 0;
            }

            try
            {
                var currentBuild = await GetCsvExportPropertyByName(buildweight.Identifier);

                if (currentBuild != null)
                {
                    currentBuild = buildweight;
                    return await _conn.UpdateAsync(currentBuild);
                }
                else
                {
                    return await _conn.InsertAsync(buildweight);
                }
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return 0;
        }

        /// <summary>
        /// This method updates a list of <see cref="CsvExportProperty"/> record in the database.
        /// </summary>
        /// <returns></returns>
        public async Task<int> UpdateCsvExportPropertiesAsync(List<CsvExportProperty> buildweights)
        {
            if (buildweights == null)
            {
                return 0;
            }

            try
            {
                return await _conn.InsertAllAsync(buildweights);
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return 0;
        }

        /// <summary>
        /// Gets a <see cref="CsvExportProperty"/> by Name
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<CsvExportProperty> GetCsvExportPropertyByName(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                return new CsvExportProperty();
            }

            try
            {
                var buildweight = from p in _conn.Table<CsvExportProperty>()
                                  where p.Identifier == identifier
                                  select p;

                return await buildweight.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return new CsvExportProperty();
        }
        #endregion

        #region MemberSnapshotItems
        /// <summary>
        /// Deletes <see cref="ClanMessage"/> by Message-ID async
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteClanMessageById(int id)
        {
            try
            {
                var deleteArtifactWeight = from weight in _conn.Table<ClanMessage>()
                                           where weight.MessageID == id
                                           select weight;

                if (deleteArtifactWeight == null)
                {
                    StatusMessage = $"Could not find id {id}";
                    return 0;
                }

                return await deleteArtifactWeight.DeleteAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }
        }

        /// <summary>
        /// This method inserts a new <see cref="ClanMessage"/> record into the database.
        /// </summary>
        /// <param name="name"></param>
        public async Task<int> AddNewClanMessageAsync(ClanMessage item)
        {
            try
            {
                if (item == null)
                {
                    throw new Exception("Valid item required");
                }

                int result = await _conn.InsertAsync(item);

                StatusMessage = string.Format("{0} record(s) added [Name: {1})", result, item.MessageID);

                return result;
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", item.MessageID, ex.Message);
                return 0;
            }

        }

        /// <summary>
        /// This method inserts a list of <see cref="ClanMessage"/> records into the database.
        /// </summary>
        /// <param name="name"></param>
        public async Task<int> AddNewClanMessageListAsync(List<ClanMessage> items)
        {
            try
            {
                if (items == null)
                {
                    throw new Exception("Valid item required");
                }

                int result = await _conn.InsertAllAsync(items);

                StatusMessage = string.Format("{0} record(s) added [Name: {1})", result, items.Count);

                return result;
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", items.Count, ex.Message);
                return 0;
            }

        }

        /// <summary>
        /// This method returns all of the <see cref="ClanMessage"/> records from the database
        /// </summary>
        /// <returns></returns>
        public async Task<List<ClanMessage>> GetAllClanMessageAsync()
        {
            try
            {
                return await _conn.Table<ClanMessage>().ToListAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return new List<ClanMessage>();
        }

        /// <summary>
        /// Gets a <see cref="ClanMessage"/> by Name
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ClanMessage> GetClanMessageById(int id)
        {
            try
            {
                var buildweight = from p in _conn.Table<ClanMessage>()
                                  where p.MessageID == id
                                  select p;

                return await buildweight.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return new ClanMessage();
        }

        /// <summary>
        /// This method updates a <see cref="ClanMessage"/> record in the database.
        /// </summary>
        /// <returns></returns>
        /// <param name="player">The ArtifactBuild that needs to be updated</param>
        public async Task<int> UpdateClanMessageAsync(ClanMessage item)
        {
            if (item == null)
            {
                return 0;
            }

            try
            {
                var currentBuild = await GetClanMessageById(item.MessageID);

                if (currentBuild != null)
                {
                    currentBuild = item;
                    return await _conn.UpdateAsync(currentBuild);
                }
                else
                {
                    return await _conn.InsertAsync(item);
                }
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return 0;
        }

        /// <summary>
        /// Deletes clanmessages that are not fitting the currentClan
        /// </summary>
        /// <param name="currentClan"></param>
        /// <returns></returns>
        public async Task<int> DeleteOldClanMsgData(string currentClan)
        {
            int rowsAffected = 0;
            try
            {
                var deleteBuild = from build in _conn.Table<ClanMessage>()
                                  where build.ClanCode != currentClan
                                  select build;

                return await deleteBuild.DeleteAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// Gets number of saved <see cref="ClanMessage"/> by Name
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> GetLastKnownClanMsgID()
        {
            try
            {
                var list = await _conn.Table<ClanMessage>().ToListAsync();

                return list == null 
                    ? 0 
                    : list.Count == 0 
                        ? 0 
                        : JfTypeConverter.ForceInt(list.Max(x => x.MessageID));
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return -1;
        }


        /// <summary>
        /// Deletes every item in the given list
        /// child items are included
        /// </summary>
        /// <param name="clanmessages"></param>
        /// <returns></returns>
        public async Task<int> DeleteClanMessages(List<ClanMessage> clanmessages)
        {
            int rowsAffected = 0;
            try
            {
                foreach (var item in clanmessages)
                {
                    rowsAffected += await DeleteMemberSnapshotItemBySId(item.MessageID);
                    rowsAffected += await _conn.DeleteAsync(item);
                }
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return rowsAffected;
        }
        #endregion

        #region SPOptConfiguration
        /// <summary>
        /// This method inserts a new <see cref="SPOptConfiguration"/> record into the database.
        /// </summary>
        /// <param name="item"></param>
        public async Task<int> AddSPOptConfigurationAsync(SPOptConfiguration item)
        {
            try
            {
                if (item == null)
                {
                    throw new Exception("Valid item required");
                }

                int result = await _conn.InsertAsync(item);

                StatusMessage = string.Format("{0} record(s) added [Name: {1})", result, item.Name);

                return result;
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }

        }

        /// <summary>
        /// This method returns all of the <see cref="SPOptConfiguration"/> records from the database.
        /// </summary>
        /// <returns></returns>
        public async Task<List<SPOptConfiguration>> GetAllSPOptConfigurationsAsync()
        {
            try
            {
                return await _conn.Table<SPOptConfiguration>().ToListAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return new List<SPOptConfiguration>();
        }

        /// <summary>
        /// This method returns the amount of saved <see cref="SPOptConfiguration"/> records from the database.
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetSPOptConfigurationsAmountAsync()
        {
            try
            {
                var v = await _conn.Table<SPOptConfiguration>().ToListAsync();

                return v.Count;
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return 0;
        }

        /// <summary>
        /// Gets a <see cref="SPOptConfiguration"/> by Name
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<SPOptConfiguration> GetSPOptConfigurationByID(string id)
        {
            try
            {
                var build = from p in _conn.Table<SPOptConfiguration>()
                            where p.Name == id
                            select p;

                return await build.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return null;
        }

        /// <summary>
        /// Gets if <see cref="SPOptConfiguration"/> is saved by Name
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> GetSPOptConfigurationSavedByID(string id)
        {
            try
            {
                var build = from p in _conn.Table<SPOptConfiguration>()
                            where p.Name == id
                            select p;

                return await build.FirstOrDefaultAsync() != null;
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return false;
        }

        /// <summary>
        /// This method updates a <see cref="SPOptConfiguration"/> record in the database.
        /// </summary>
        /// <returns></returns>
        public async Task<int> UpdateSPOptConfigurationAsync(SPOptConfiguration item)
        {
            if (item == null)
            {
                return 0;
            }

            try
            {
                var currentItem = await GetSPOptConfigurationByID(item.Name);

                if (currentItem != null)
                {
                    currentItem = item;
                    return await _conn.UpdateAsync(currentItem);
                }
                else
                {
                    return await _conn.InsertAsync(item);
                }
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return 0;
        }

        /// <summary>
        /// Deletes a <see cref="SPOptConfiguration"/> by ID async
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteSPOptConfigurationByID(string id)
        {
            try
            {
                var deleteBuild = from build in _conn.Table<SPOptConfiguration>()
                                  where build.Name == id
                                  select build;

                var dbBuild = await deleteBuild.FirstOrDefaultAsync();

                if (dbBuild == null)
                {
                    StatusMessage = $"Could not find item {id}";
                    return 0;
                }

                return await _conn.DeleteAsync(dbBuild);
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }
        }

        /// <summary>
        /// Deletes every item in the given list
        /// child items are included
        /// </summary>
        /// <param name="RpMRecord"></param>
        /// <returns></returns>
        public async Task<int> DeleteSPOptConfigurations(List<SPOptConfiguration> items)
        {
            int rowsAffected = 0;
            try
            {
                foreach (var item in items)
                {
                    rowsAffected += await DeleteSPOptSkillSettingBySId(item.Name);
                    rowsAffected += await _conn.DeleteAsync(item);
                }
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return rowsAffected;
        }
        #endregion

        #region SPOptSkillSetting
        /// <summary>
        /// This method inserts a list of <see cref="SPOptSkillSetting"/> records into the database.
        /// </summary>
        /// <param name="name"></param>
        public async Task<int> AddNewSPOptSkillSettingListAsync(List<SPOptSkillSetting> items)
        {
            try
            {
                if (items == null)
                {
                    throw new Exception("Valid item required");
                }

                int result = await _conn.InsertAllAsync(items);

                StatusMessage = string.Format("{0} record(s) added [Name: {1})", result, items.Count);

                return result;
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0} items. Error: {1}", items.Count, ex.Message);
                return 0;
            }

        }

        /// <summary>
        /// Deletes <see cref="SPOptSkillSetting"/> by Parent-ID async
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteSPOptSkillSettingBySId(string id)
        {
            try
            {
                var deleteArtifactWeight = from weight in _conn.Table<SPOptSkillSetting>()
                                           where weight.Configuration == id
                                           select weight;

                if (deleteArtifactWeight == null)
                {
                    StatusMessage = $"Could not find id {id}";
                    return 0;
                }

                return await deleteArtifactWeight.DeleteAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }
        }

        /// <summary>
        /// This method inserts a new <see cref="SPOptSkillSetting"/> record into the database.
        /// </summary>
        /// <param name="item"></param>
        public async Task<int> AddSPOptSkillSettingAsync(SPOptSkillSetting item)
        {
            try
            {
                if (item == null)
                {
                    throw new Exception("Valid item required");
                }

                int result = await _conn.InsertAsync(item);

                StatusMessage = string.Format("{0} record(s) added [Name: {1})", result, item.Identifier);

                return result;
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }

        }

        /// <summary>
        /// This method returns all of the <see cref="SPOptSkillSetting"/> records from the database.
        /// </summary>
        /// <returns></returns>
        public async Task<List<SPOptSkillSetting>> GetAllSPOptSkillSettingAsync()
        {
            try
            {
                return await _conn.Table<SPOptSkillSetting>().ToListAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return new List<SPOptSkillSetting>();
        }

        /// <summary>
        /// Gets a <see cref="SPOptSkillSetting"/> by Name
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<SPOptSkillSetting> GetSPOptSkillSettingByID(string id)
        {
            try
            {
                var build = from p in _conn.Table<SPOptSkillSetting>()
                            where p.Identifier == id
                            select p;

                return await build.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return null;
        }

        /// <summary>
        /// This method updates a <see cref="SPOptSkillSetting"/> record in the database.
        /// </summary>
        /// <returns></returns>
        public async Task<int> UpdateSPOptSkillSettingAsync(SPOptSkillSetting item)
        {
            if (item == null)
            {
                return 0;
            }

            try
            {
                var currentItem = await GetSPOptSkillSettingByID(item.Identifier);

                if (currentItem != null)
                {
                    currentItem = item;
                    return await _conn.UpdateAsync(currentItem);
                }
                else
                {
                    return await _conn.InsertAsync(item);
                }
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return 0;
        }

        /// <summary>
        /// Deletes a <see cref="SPOptSkillSetting"/> by ID async
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteSPOptSkillSettingID(string id)
        {
            try
            {
                var deleteBuild = from build in _conn.Table<SPOptSkillSetting>()
                                  where build.Identifier == id
                                  select build;

                var dbBuild = await deleteBuild.FirstOrDefaultAsync();

                if (dbBuild == null)
                {
                    StatusMessage = $"Could not find item {id}";
                    return 0;
                }

                return await _conn.DeleteAsync(dbBuild);
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }
        }

        /// <summary>
        /// This method returns all of the <see cref="SPOptSkillSetting"/> records from the database
        /// for a specific parent
        /// </summary>
        /// <returns></returns>
        public async Task<List<SPOptSkillSetting>> GetAllSPOptSkillSettingByParentIdAsync(string id)
        {
            try
            {
                return await _conn.Table<SPOptSkillSetting>().Where(x => x.Configuration == id).ToListAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return new List<SPOptSkillSetting>();
        }
        #endregion

        #region ClanRaid
        /// <summary>
        /// This method inserts a new <see cref="ClanRaid"/> record into the database.
        /// </summary>
        /// <param name="item"></param>
        public async Task<int> AddClanRaidAsync(ClanRaid item)
        {
            try
            {
                if (item == null)
                {
                    throw new Exception("Valid item required");
                }

                int result = await _conn.InsertAsync(item);

                StatusMessage = string.Format("{0} record(s) added [Name: {1})", result, item.ID);

                return result;
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }

        }

        /// <summary>
        /// This method returns all of the <see cref="ClanRaid"/> records from the database.
        /// </summary>
        /// <returns></returns>
        public async Task<List<ClanRaid>> GetAllClanRaidsAsync()
        {
            try
            {
                return await _conn.Table<ClanRaid>().ToListAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return new List<ClanRaid>();
        }

        /// <summary>
        /// This method returns the amount of saved <see cref="ClanRaid"/> records from the database.
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetClanRaidAmountAsync()
        {
            try
            {
                var v = await _conn.Table<ClanRaid>().ToListAsync();

                return v?.Count ?? 0;
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return 0;
        }

        /// <summary>
        /// Gets a <see cref="ClanRaid"/> by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ClanRaid> GetClanRaidByID(int id)
        {
            try
            {
                var items = from p in _conn.Table<ClanRaid>()
                            where p.ID == id
                            select p;

                return await items.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return null;
        }

        /// <summary>
        /// Gets if <see cref="ClanRaid"/> is existing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> IsClanRaidExisting(int id)
        {
            try
            {
                var build = from p in _conn.Table<ClanRaid>()
                            where p.ID == id
                            select p;

                return await build.FirstOrDefaultAsync() != null;
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return false;
        }

        /// <summary>
        /// This method updates a <see cref="ClanRaid"/> record in the database.
        /// </summary>
        /// <returns></returns>
        public async Task<int> UpdateClanRaidAsync(ClanRaid item)
        {
            if (item == null)
            {
                return 0;
            }

            try
            {
                var currentItem = await GetClanRaidByID(item.ID);

                if (currentItem != null)
                {
                    currentItem = item;
                    return await _conn.UpdateAsync(currentItem);
                }
                else
                {
                    return await _conn.InsertAsync(item);
                }
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return 0;
        }

        /// <summary>
        /// Deletes a <see cref="SPOptConfiguration"/> by ID async
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteClanRaidByID(int id)
        {
            try
            {
                var deleteBuild = from build in _conn.Table<ClanRaid>()
                                  where build.ID == id
                                  select build;

                var dbBuild = await deleteBuild.FirstOrDefaultAsync();

                int rowsAffected = 0;

                rowsAffected += await DeleteClanRaidResultByParentId(id);
                rowsAffected += await DeleteClanRaidEnemyStrategiesByClanRaidId(id);
                rowsAffected += await DeleteClanRaidToleranceRelationshipByClanRaidId(id);

                if (dbBuild == null)
                {
                    StatusMessage = $"Could not find item {id}";
                    return rowsAffected;
                }

                rowsAffected += await _conn.DeleteAsync(dbBuild);

                return rowsAffected;
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }
        }

        /// <summary>
        /// Deletes every item in the given list
        /// child items are included
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public async Task<int> DeleteClanRaids(List<ClanRaid> items)
        {
            int rowsAffected = 0;
            try
            {
                foreach (var item in items)
                {
                    rowsAffected += await DeleteClanRaidByID(item.ID);
                }
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return rowsAffected;
        }
        #endregion

        #region ClanRaidResult
        /// <summary>
        /// This method inserts a list of <see cref="ClanRaidResult"/> records into the database.
        /// </summary>
        /// <param name="name"></param>
        public async Task<int> AddNewClanRaidResultListAsync(List<ClanRaidResult> items)
        {
            try
            {
                if (items == null)
                {
                    throw new Exception("Valid item required");
                }

                int result = await _conn.InsertAllAsync(items);

                StatusMessage = string.Format("{0} record(s) added [Name: {1})", result, items.Count);

                return result;
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", items.Count, ex.Message);
                return 0;
            }

        }

        /// <summary>
        /// Deletes <see cref="ClanRaidResult"/> by Parent-ID async
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteClanRaidResultByParentId(int id)
        {
            try
            {
                var deleteArtifactWeight = from weight in _conn.Table<ClanRaidResult>()
                                           where weight.ParentId == id
                                           select weight;

                if (deleteArtifactWeight == null)
                {
                    StatusMessage = $"Could not find id {id}";
                    return 0;
                }

                return await deleteArtifactWeight.DeleteAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }
        }

        /// <summary>
        /// This method inserts a new <see cref="ClanRaidResult"/> record into the database.
        /// </summary>
        /// <param name="item"></param>
        public async Task<int> AddClanRaidResultAsync(ClanRaidResult item)
        {
            try
            {
                if (item == null)
                {
                    throw new Exception("Valid item required");
                }

                int result = await _conn.InsertAsync(item);

                StatusMessage = string.Format("{0} record(s) added [Name: {1})", result, item.ID);

                return result;
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }

        }

        /// <summary>
        /// This method returns all of the <see cref="ClanRaidResult"/> records from the database.
        /// </summary>
        /// <returns></returns>
        public async Task<List<ClanRaidResult>> GetAllClanRaidResultAsync()
        {
            try
            {
                return await _conn.Table<ClanRaidResult>().ToListAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return new List<ClanRaidResult>();
        }

        /// <summary>
        /// Gets all <see cref="ClanRaidResult"/> by parent id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<ClanRaidResult>> GetClanRaidResultsByParentID(int id)
        {
            try
            {
                var build = from p in _conn.Table<ClanRaidResult>()
                            where p.ParentId == id
                            select p;

                return await build.ToListAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return null;
        }

        /// <summary>
        /// Gets a <see cref="ClanRaidResult"/> by parent id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ClanRaidResult> GetClanRaidResultByID(int id)
        {
            try
            {
                var build = from p in _conn.Table<ClanRaidResult>()
                            where p.ID == id
                            select p;

                return await build.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return null;
        }

        /// <summary>
        /// This method updates a <see cref="ClanRaidResult"/> record in the database.
        /// </summary>
        /// <returns></returns>
        public async Task<int> UpdateClanRaidResultAsync(ClanRaidResult item)
        {
            if (item == null)
            {
                return 0;
            }

            try
            {
                var currentItem = await GetClanRaidResultByID(item.ID);

                if (currentItem != null)
                {
                    currentItem = item;
                    return await _conn.UpdateAsync(currentItem);
                }
                else
                {
                    return await _conn.InsertAsync(item);
                }
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return 0;
        }

        /// <summary>
        /// Deletes a <see cref="ClanRaidResult"/> by ID async
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteClanRaidResultByID(int id)
        {
            try
            {
                var deleteBuild = from build in _conn.Table<ClanRaidResult>()
                                  where build.ID == id
                                  select build;

                var dbBuild = await deleteBuild.FirstOrDefaultAsync();

                if (dbBuild == null)
                {
                    StatusMessage = $"Could not find item {id}";
                    return 0;
                }

                return await _conn.DeleteAsync(dbBuild);
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }
        }
        #endregion

        #region RaidStrategy
        /// <summary>
        /// This method inserts a new <see cref="RaidStrategy"/> record into the database.
        /// </summary>
        /// <param name="item"></param>
        public async Task<int> AddRaidStrategyAsync(RaidStrategy item)
        {
            try
            {
                if (item == null)
                {
                    throw new Exception("Valid item required");
                }

                int result = await _conn.InsertAsync(item);

                StatusMessage = string.Format("{0} record(s) added [Name: {1})", result, item.Name);

                return result;
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }

        }

        /// <summary>
        /// This method returns all of the <see cref="RaidStrategy"/> records from the database.
        /// </summary>
        /// <returns></returns>
        public async Task<List<RaidStrategy>> GetAllRaidStrategiesAsync()
        {
            try
            {
                return await _conn.Table<RaidStrategy>().ToListAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return new List<RaidStrategy>();
        }

        /// <summary>
        /// Gets all raid strategies for given enemy id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<RaidStrategy>> GetRaidStrategiesByEnemyID(string id)
        {
            try
            {
                var build = from p in _conn.Table<RaidStrategy>()
                            where p.EnemyId == id
                            select p;

                return await build.ToListAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return null;
        }

        /// <summary>
        /// This method returns the amount of saved <see cref="RaidStrategy"/> records from the database.
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetRaidStrategyAmountAsync()
        {
            try
            {
                var v = await _conn.Table<RaidStrategy>().ToListAsync();

                return v?.Count ?? 0;
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return 0;
        }

        /// <summary>
        /// Gets a <see cref="RaidStrategy"/> by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<RaidStrategy> GetRaidStrategyByID(string name)
        {
            try
            {
                var items = from p in _conn.Table<RaidStrategy>()
                            where p.Name == name
                            select p;

                return await items.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return null;
        }

        /// <summary>
        /// Gets if <see cref="RaidStrategy"/> is existing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> IsRaidStrategyExisting(string name)
        {
            try
            {
                var build = from p in _conn.Table<RaidStrategy>()
                            where p.Name == name
                            select p;

                return await build.FirstOrDefaultAsync() != null;
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return false;
        }

        /// <summary>
        /// This method updates a <see cref="RaidStrategy"/> record in the database.
        /// </summary>
        /// <returns></returns>
        public async Task<int> UpdateRaidStrategyAsync(RaidStrategy item)
        {
            if (item == null)
            {
                return 0;
            }

            try
            {
                var currentItem = await GetRaidStrategyByID(item.Name);

                if (currentItem != null)
                {
                    currentItem = item;
                    return await _conn.UpdateAsync(currentItem);
                }
                else
                {
                    return await _conn.InsertAsync(item);
                }
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return 0;
        }

        /// <summary>
        /// Deletes a <see cref="RaidStrategy"/> by ID async
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteRaidStrategyByID(string name)
        {
            try
            {
                var deleteBuild = from build in _conn.Table<RaidStrategy>()
                                  where build.Name == name
                                  select build;

                var dbBuild = await deleteBuild.FirstOrDefaultAsync();

                if (dbBuild == null)
                {
                    StatusMessage = $"Could not find item {name}";
                    return 0;
                }

                _ = await DeleteClanRaidEnemyStrategyByStrategyID(name);

                return await _conn.DeleteAsync(dbBuild);
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }
        }


        #endregion

        #region ClanRaidEnemyStrategy
        /// <summary>
        /// This method inserts a list of <see cref="ClanRaidEnemyStrategy"/> records into the database.
        /// </summary>
        /// <param name="name"></param>
        public async Task<int> AddNewClanRaidEnemyStrategyListAsync(List<ClanRaidEnemyStrategy> items)
        {
            try
            {
                if (items == null)
                {
                    throw new Exception("Valid item required");
                }

                int result = await _conn.InsertAllAsync(items);

                StatusMessage = string.Format("{0} record(s) added [Name: {1})", result, items.Count);

                return result;
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", items.Count, ex.Message);
                return 0;
            }

        }

        /// <summary>
        /// Deletes <see cref="ClanRaidEnemyStrategy"/> by clan raid-ID async
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteClanRaidEnemyStrategiesByClanRaidId(int id)
        {
            try
            {
                var deleteArtifactWeight = from weight in _conn.Table<ClanRaidEnemyStrategy>()
                                           where weight.ClanRaidId == id
                                           select weight;

                if (deleteArtifactWeight == null)
                {
                    StatusMessage = $"Could not find id {id}";
                    return 0;
                }

                return await deleteArtifactWeight.DeleteAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }
        }

        /// <summary>
        /// Deletes <see cref="ClanRaidEnemyStrategy"/> by clan strategy-ID async
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteClanRaidEnemyStrategyByStrategyID(string id)
        {
            try
            {
                var deleteArtifactWeight = from weight in _conn.Table<ClanRaidEnemyStrategy>()
                                           where weight.RaidStrategyId == id
                                           select weight;

                if (deleteArtifactWeight == null)
                {
                    StatusMessage = $"Could not find id {id}";
                    return 0;
                }

                return await deleteArtifactWeight.DeleteAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }
        }

        /// <summary>
        /// This method inserts a new <see cref="ClanRaidEnemyStrategy"/> record into the database.
        /// </summary>
        /// <param name="item"></param>
        public async Task<int> AddClanRaidEnemyStrategyAsync(ClanRaidEnemyStrategy item)
        {
            try
            {
                if (item == null)
                {
                    throw new Exception("Valid item required");
                }

                int result = await _conn.InsertAsync(item);

                StatusMessage = string.Format("{0} record(s) added [Name: {1})", result, item.ID);

                return result;
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }

        }

        /// <summary>
        /// This method returns all of the <see cref="ClanRaidEnemyStrategy"/> records from the database.
        /// </summary>
        /// <returns></returns>
        public async Task<List<ClanRaidEnemyStrategy>> GetAllClanRaidEnemyStrategiesAsync()
        {
            try
            {
                return await _conn.Table<ClanRaidEnemyStrategy>().ToListAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return new List<ClanRaidEnemyStrategy>();
        }

        /// <summary>
        /// Gets all <see cref="ClanRaidEnemyStrategy"/> by raid id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<ClanRaidEnemyStrategy>> GetClanRaidEnemyStrategiesByClanRaidID(int id)
        {
            try
            {
                var build = from p in _conn.Table<ClanRaidEnemyStrategy>()
                            where p.ClanRaidId == id
                            select p;

                return await build.ToListAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return null;
        }
        
        /// <summary>
        /// Gets all <see cref="ClanRaidEnemyStrategy"/> by strategy id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<ClanRaidEnemyStrategy>> GetClanRaidEnemyStrategiesByStrategyID(string id)
        {
            try
            {
                var build = from p in _conn.Table<ClanRaidEnemyStrategy>()
                            where p.RaidStrategyId == id
                            select p;

                return await build.ToListAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return null;
        }

        /// <summary>
        /// Gets a <see cref="ClanRaidEnemyStrategy"/> by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ClanRaidEnemyStrategy> GetClanRaidEnemyStrategyByID(int id)
        {
            try
            {
                var build = from p in _conn.Table<ClanRaidEnemyStrategy>()
                            where p.ID == id
                            select p;

                return await build.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return null;
        }

        /// <summary>
        /// This method updates a <see cref="ClanRaidEnemyStrategy"/> record in the database.
        /// </summary>
        /// <returns></returns>
        public async Task<int> UpdateClanRaidEnemyStrategyAsync(ClanRaidEnemyStrategy item)
        {
            if (item == null)
            {
                return 0;
            }

            try
            {
                var currentItem = await GetClanRaidEnemyStrategyByID(item.ID);

                if (currentItem != null)
                {
                    currentItem = item;
                    return await _conn.UpdateAsync(currentItem);
                }
                else
                {
                    return await _conn.InsertAsync(item);
                }
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return 0;
        }

        /// <summary>
        /// Deletes a <see cref="ClanRaidEnemyStrategy"/> by ID async
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteClanRaidEnemyStrategyByID(int id)
        {
            try
            {
                var deleteBuild = from build in _conn.Table<ClanRaidEnemyStrategy>()
                                  where build.ID == id
                                  select build;

                var dbBuild = await deleteBuild.FirstOrDefaultAsync();

                if (dbBuild == null)
                {
                    StatusMessage = $"Could not find item {id}";
                    return 0;
                }

                return await _conn.DeleteAsync(dbBuild);
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }
        }
        #endregion

        #region RaidTolerance
        /// <summary>
        /// This method inserts a new <see cref="RaidTolerance"/> record into the database.
        /// </summary>
        /// <param name="item"></param>
        public async Task<int> AddRaidToleranceAsync(RaidTolerance item)
        {
            try
            {
                if (item == null)
                {
                    throw new Exception("Valid item required");
                }

                int result = await _conn.InsertAsync(item);

                StatusMessage = string.Format("{0} record(s) added [Name: {1})", result, item.Name);

                return result;
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }

        }

        /// <summary>
        /// This method returns all of the <see cref="RaidTolerance"/> records from the database.
        /// </summary>
        /// <returns></returns>
        public async Task<List<RaidTolerance>> GetAllRaidTolerancesAsync()
        {
            try
            {
                return await _conn.Table<RaidTolerance>().ToListAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return new List<RaidTolerance>();
        }

        /// <summary>
        /// This method returns the amount of saved <see cref="RaidTolerance"/> records from the database.
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetRaidToleranceAmountAsync()
        {
            try
            {
                var v = await _conn.Table<RaidTolerance>().ToListAsync();

                return v?.Count ?? 0;
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return 0;
        }

        /// <summary>
        /// Gets a <see cref="RaidTolerance"/> by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<RaidTolerance> GetRaidToleranceByID(string name)
        {
            try
            {
                var items = from p in _conn.Table<RaidTolerance>()
                            where p.Name == name
                            select p;

                return await items.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return null;
        }

        /// <summary>
        /// Gets if <see cref="RaidTolerance"/> is existing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> IsRaidToleranceExisting(string name)
        {
            try
            {
                var build = from p in _conn.Table<RaidTolerance>()
                            where p.Name == name
                            select p;

                return await build.FirstOrDefaultAsync() != null;
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return false;
        }

        /// <summary>
        /// This method updates a <see cref="RaidTolerance"/> record in the database.
        /// </summary>
        /// <returns></returns>
        public async Task<int> UpdateRaidToleranceAsync(RaidTolerance item)
        {
            if (item == null)
            {
                return 0;
            }

            try
            {
                var currentItem = await GetRaidToleranceByID(item.Name);

                if (currentItem != null)
                {
                    currentItem = item;
                    return await _conn.UpdateAsync(currentItem);
                }
                else
                {
                    return await _conn.InsertAsync(item);
                }
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return 0;
        }

        /// <summary>
        /// Deletes a <see cref="RaidTolerance"/> by ID async
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteRaidToleranceByID(string name)
        {
            try
            {
                var deleteBuild = from build in _conn.Table<RaidTolerance>()
                                  where build.Name == name
                                  select build;

                var dbBuild = await deleteBuild.FirstOrDefaultAsync();

                if (dbBuild == null)
                {
                    StatusMessage = $"Could not find item {name}";
                    return 0;
                }

                _ = await DeleteClanRaidToleranceRelationshipByToleranceID(name);

                return await _conn.DeleteAsync(dbBuild);
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }
        }
        #endregion

        #region ClanRaidToleranceRelationship
        /// <summary>
        /// This method inserts a list of <see cref="ClanRaidToleranceRelationship"/> records into the database.
        /// </summary>
        /// <param name="name"></param>
        public async Task<int> AddNewClanRaidToleranceRelationshipListAsync(List<ClanRaidToleranceRelationship> items)
        {
            try
            {
                if (items == null)
                {
                    throw new Exception("Valid item required");
                }

                int result = await _conn.InsertAllAsync(items);

                StatusMessage = string.Format("{0} record(s) added [Name: {1})", result, items.Count);

                return result;
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", items.Count, ex.Message);
                return 0;
            }

        }

        /// <summary>
        /// Deletes <see cref="ClanRaidToleranceRelationship"/> by clan raid-ID async
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteClanRaidToleranceRelationshipByClanRaidId(int id)
        {
            try
            {
                var deleteArtifactWeight = from weight in _conn.Table<ClanRaidToleranceRelationship>()
                                           where weight.ClanRaidId == id
                                           select weight;

                if (deleteArtifactWeight == null)
                {
                    StatusMessage = $"Could not find id {id}";
                    return 0;
                }

                return await deleteArtifactWeight.DeleteAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }
        }

        /// <summary>
        /// Deletes <see cref="ClanRaidToleranceRelationship"/> by tolerance id async
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteClanRaidToleranceRelationshipByToleranceID(string id)
        {
            try
            {
                var deleteArtifactWeight = from weight in _conn.Table<ClanRaidToleranceRelationship>()
                                           where weight.RaidToleranceId == id
                                           select weight;

                if (deleteArtifactWeight == null)
                {
                    StatusMessage = $"Could not find id {id}";
                    return 0;
                }

                return await deleteArtifactWeight.DeleteAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }
        }

        /// <summary>
        /// This method inserts a new <see cref="ClanRaidToleranceRelationship"/> record into the database.
        /// </summary>
        /// <param name="item"></param>
        public async Task<int> AddClanRaidToleranceRelationshipAsync(ClanRaidToleranceRelationship item)
        {
            try
            {
                if (item == null)
                {
                    throw new Exception("Valid item required");
                }

                int result = await _conn.InsertAsync(item);

                StatusMessage = string.Format("{0} record(s) added [Name: {1})", result, item.ID);

                return result;
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }

        }

        /// <summary>
        /// This method returns all of the <see cref="ClanRaidToleranceRelationship"/> records from the database.
        /// </summary>
        /// <returns></returns>
        public async Task<List<ClanRaidToleranceRelationship>> GetAllClanRaidToleranceRelationshipsAsync()
        {
            try
            {
                return await _conn.Table<ClanRaidToleranceRelationship>().ToListAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return new List<ClanRaidToleranceRelationship>();
        }

        /// <summary>
        /// Gets all <see cref="ClanRaidToleranceRelationship"/> by tolerance id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<ClanRaidToleranceRelationship>> GetClanRaidToleranceRelationshipByToleranceID(string id)
        {
            try
            {
                var build = from p in _conn.Table<ClanRaidToleranceRelationship>()
                            where p.RaidToleranceId == id
                            select p;

                return await build.ToListAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return null;
        }

        /// <summary>
        /// Gets all <see cref="ClanRaidToleranceRelationship"/> by raid id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<ClanRaidToleranceRelationship>> GetClanRaidToleranceRelationshipByClanRaidID(int id)
        {
            try
            {
                var build = from p in _conn.Table<ClanRaidToleranceRelationship>()
                            where p.ClanRaidId == id
                            select p;

                return await build.ToListAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return null;
        }

        /// <summary>
        /// Gets a <see cref="ClanRaidToleranceRelationship"/> by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ClanRaidToleranceRelationship> GetClanRaidToleranceRelationshipByID(int id)
        {
            try
            {
                var build = from p in _conn.Table<ClanRaidToleranceRelationship>()
                            where p.ID == id
                            select p;

                return await build.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return null;
        }

        /// <summary>
        /// This method updates a <see cref="ClanRaidToleranceRelationship"/> record in the database.
        /// </summary>
        /// <returns></returns>
        public async Task<int> UpdateClanRaidToleranceRelationshipAsync(ClanRaidToleranceRelationship item)
        {
            if (item == null)
            {
                return 0;
            }

            try
            {
                var currentItem = await GetClanRaidToleranceRelationshipByID(item.ID);

                if (currentItem != null)
                {
                    currentItem = item;
                    return await _conn.UpdateAsync(currentItem);
                }
                else
                {
                    return await _conn.InsertAsync(item);
                }
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return 0;
        }

        /// <summary>
        /// Deletes a <see cref="ClanRaidToleranceRelationship"/> by ID async
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteClanRaidToleranceRelationshipByID(int id)
        {
            try
            {
                var deleteBuild = from build in _conn.Table<ClanRaidToleranceRelationship>()
                                  where build.ID == id
                                  select build;

                var dbBuild = await deleteBuild.FirstOrDefaultAsync();

                if (dbBuild == null)
                {
                    StatusMessage = $"Could not find item {id}";
                    return 0;
                }

                return await _conn.DeleteAsync(dbBuild);
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }
        }
        #endregion

        #region Announcement
        /// <summary>
        /// This method inserts a list of <see cref="DbAnnouncement"/> records into the database.
        /// </summary>
        /// <param name="name"></param>
        public async Task<int> AddNewDbAnnouncementListAsync(List<DbAnnouncement> items)
        {
            try
            {
                if (items == null)
                {
                    throw new Exception("Valid item required");
                }

                int result = await _conn.InsertAllAsync(items);

                StatusMessage = string.Format("{0} record(s) added [Name: {1})", result, items.Count);

                return result;
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", items.Count, ex.Message);
                return 0;
            }

        }

        /// <summary>
        /// Deletes <see cref="DbAnnouncement"/> by clan iD async
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteDbAnnouncementById(int id)
        {
            try
            {
                var items = from fart in _conn.Table<DbAnnouncement>()
                                           where fart.ID == id
                                           select fart;

                if (items == null)
                {
                    StatusMessage = $"Could not find id {id}";
                    return 0;
                }

                return await items.DeleteAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }
        }

        /// <summary>
        /// This method inserts a new <see cref="DbAnnouncement"/> record into the database.
        /// </summary>
        /// <param name="item"></param>
        public async Task<int> AddDbAnnouncementAsync(DbAnnouncement item)
        {
            try
            {
                if (item == null)
                {
                    throw new Exception("Valid item required");
                }

                int result = await _conn.InsertAsync(item);

                StatusMessage = string.Format("{0} record(s) added [Name: {1})", result, item.ID);

                return result;
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }

        }

        /// <summary>
        /// This method returns all of the <see cref="DbAnnouncement"/> records from the database.
        /// </summary>
        /// <returns></returns>
        public async Task<List<DbAnnouncement>> GetAllDbAnnouncementAsync()
        {
            try
            {
                return await _conn.Table<DbAnnouncement>().ToListAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return new List<DbAnnouncement>();
        }

        /// <summary>
        /// Gets all <see cref="DbAnnouncement"/> unseen announcements
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<DbAnnouncement>> GetUnseenDbAnnouncementsAsync()
        {
            try
            {
                var build = from p in _conn.Table<DbAnnouncement>()
                            where !p.IsSeen
                            select p;

                return await build.ToListAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return null;
        }

        /// <summary>
        /// Gets a <see cref="DbAnnouncement"/> by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<DbAnnouncement> GetDbAnnouncementByID(int id)
        {
            try
            {
                var build = from p in _conn.Table<DbAnnouncement>()
                            where p.ID == id
                            select p;

                return await build.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return null;
        }

        /// <summary>
        /// This method upserts a <see cref="DbAnnouncement"/> record in the database.
        /// </summary>
        /// <returns></returns>
        public async Task<int> UpsertDbAnnouncementAsync(DbAnnouncement item)
        {
            if (item == null)
            {
                return 0;
            }

            try
            {
                var currentItem = await GetDbAnnouncementByID(item.ID);

                if (currentItem != null)
                {
                    currentItem = item;
                    return await _conn.UpdateAsync(currentItem);
                }
                else
                {
                    return await _conn.InsertAsync(item);
                }
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return 0;
        }

        /// <summary>
        /// Deletes a <see cref="DbAnnouncement"/> by ID async
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteDbAnnouncementByIDAsync(int id)
        {
            try
            {
                var deleteBuild = from build in _conn.Table<DbAnnouncement>()
                                  where build.ID == id
                                  select build;

                var dbBuild = await deleteBuild.FirstOrDefaultAsync();

                if (dbBuild == null)
                {
                    StatusMessage = $"Could not find item {id}";
                    return 0;
                }

                return await _conn.DeleteAsync(dbBuild);
            }
            catch (Exception ex)
            {
                Error(ex);
                return 0;
            }
        }

        /// <summary>
        /// Gets highest id of saved <see cref="DbAnnouncement"/>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> GetLatestDbAnnouncementID()
        {
            try
            {
                var list = await _conn.Table<DbAnnouncement>().ToListAsync();

                return list == null
                    ? 0
                    : list.Count == 0
                        ? 0
                        : JfTypeConverter.ForceInt(list.Max(x => x.ID));
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return -1;
        }

        /// <summary>
        /// Sets IsSeen true <see cref="DbAnnouncement"/> by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> UpdateAnnouncementAsSeenByIdAsync(int id)
        {
            try
            {
                var build = from p in _conn.Table<DbAnnouncement>()
                            where p.ID == id
                            select p;

                var item = await build.FirstOrDefaultAsync();

                if(item != null)
                {
                    item.IsSeen = true;
                }

                return await UpsertDbAnnouncementAsync(item);
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return 0;
        }
        #endregion
        #endregion
    }
}