using System;
using System.Collections.Generic;
using System.Text;

namespace TT2Master.Model.Clan
{
    public class TournamentResultMember
    {
        public string Id { get; set; }
        public int Rank { get; set; }
        public string Name { get; set; }
        public int Stage { get; set; }
        public string Flag { get; set; }
        public int UndisputedCount { get; set; }
        public bool HighlightPlayer { get; set; }
    }
}
