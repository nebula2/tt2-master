namespace TT2Master.Shared.Models
{
    /// <summary>
    /// Describes a helper/ hero
    /// </summary>
    public class Helper
    {
        /// <summary>
        /// Numeric identifier for this helper
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Identifier in Json
        /// </summary>
        public string JsonID { get; set; }

        /// <summary>
        /// Weapon Level
        /// </summary>
        public int WeaponLevel { get; set; }

        /// <summary>
        /// Scroll Level
        /// </summary>
        public int ScrollLevel { get; set; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="jsonId"></param>
        public Helper(int id, string jsonId)
        {
            ID = id;
            JsonID = jsonId;
            WeaponLevel = 0;
            ScrollLevel = 0;
        }
    }
}
