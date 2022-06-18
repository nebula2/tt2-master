using Prism.Mvvm;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TT2Master
{
    /// <summary>
    /// Describes a snapshot, which is like an image of the players current state
    /// </summary>
    [Table("SNAPSHOT")]
    public class Snapshot : BindableBase
    {
        /// <summary>
        /// Identifier
        /// </summary>
        [PrimaryKey]
        public int ID { get; set; }

        /// <summary>
        /// Clan name
        /// </summary>
        public string Clanname { get; set; } = "";

        /// <summary>
        /// Type of snapshot
        /// </summary>
        public SnapshotType Type { get; set; }

        /// <summary>
        /// Timestamp of creation
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Is this snapshot self made (true) or has it been created automatically (false)?
        /// </summary>
        public bool SelfMade { get; set; } = false;

        private List<MemberSnapshotItem> _memberSnapshotItems;
        /// <summary>
        /// Collection of Snapshot items for members
        /// </summary>
        [Ignore]
        public List<MemberSnapshotItem> MemberSnapshotItems
        {
            get => _memberSnapshotItems; set
            {
                SetProperty(ref _memberSnapshotItems, value);
                OnPropertyChanged(new PropertyChangedEventArgs(string.Empty));
            }
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public Snapshot()
        {
            MemberSnapshotItems = new List<MemberSnapshotItem>();
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="id">identifier</param>
        /// <param name="type">snapshot type</param>
        /// <param name="time">creation time</param>
        /// <param name="clan">clan name</param>
        public Snapshot(int id, SnapshotType type, DateTime time, string clan) : base()
        {
            ID = id;
            Type = type;
            Timestamp = time;
            Clanname = clan;

            MemberSnapshotItems = new List<MemberSnapshotItem>();
        }
    }
}