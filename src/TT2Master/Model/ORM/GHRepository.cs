using SQLite;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace TT2Master
{
    /// <summary>
    /// DB Repository for accessing Tap Titans SQLite-DB
    /// </summary>
    public class GHRepository
    {
        #region private member
        /// <summary>
        /// SQL-Connection
        /// </summary>
        private SQLiteAsyncConnection _conn;

        #endregion

        #region Properties
        /// <summary>
        /// his property provides a string which reports the last success or fail message from the database.
        /// </summary>
        public string StatusMessage { get; set; }

        #endregion

        #region ctor
        /// <summary>
        /// The constructor initializes the underlying SQLite connection.
        /// </summary>
        /// <param name="dbPath"></param>
        public GHRepository(string dbPath)
        {
            _conn = new SQLiteAsyncConnection(dbPath);
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Gets the session data from ga_session
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<GH_ga_session> GetSessionData()
        {
            try
            {
                OnProgressMade?.Invoke("Try accessing Sessiondata");

                var session = from s in _conn.Table<GH_ga_session>()
                              orderby s.Session_Id
                              select s;

                return await session.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
                OnProgressMade?.Invoke("Error accessing Sessiondata");
            }

            return new GH_ga_session();
        } 
        #endregion

        #region events delegates
        /// <summary>
        /// Delegate for <see cref="OnProgressMade"/>
        /// </summary>
        /// <param name="message"></param>
        public delegate void ProgressCarrier(string message);

        /// <summary>
        /// Raised when <see cref="Initialize"/> made progress
        /// </summary>
        public event ProgressCarrier OnProgressMade; 
        #endregion
    }
}
