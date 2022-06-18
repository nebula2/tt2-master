using System.IO;
using System.Linq;
using TT2Master.Model.Clan;
using Xunit;

namespace TT2Master.Tests
{
    public class TournamentResultMemberTests
    {
        #region Helper
        private DBRepository GetDBRepository() => new DBRepository("tt2master.db3");

        private string GetDataString() => File.ReadAllText($"Resources\\tournamentResultString.txt");
        #endregion

        #region Unit Tests
        [Fact]
        public void TournamentResult_ShouldInstantiate()
        {
            var tr = new TournamentResult();
            Assert.NotNull(tr);
        }
        #endregion

        #region Integration Tests
        [Fact]
        public void TournamentResult_ShouldLoadFromJsonString()
        {
            var cm = new ClanMessage
            {
                Message = GetDataString(),
            };

            var t = new TournamentResult(cm);

            Assert.NotNull(t);
            Assert.NotNull(t.Member);
            Assert.Equal(9, t.Member.Count);

            var first = t.Member.Where(x => x.Rank == 1).Single();
            Assert.True(first.Id == "y6x9pxb");

            var high = t.Member.Where(x => x.HighlightPlayer).Single();
            Assert.True(high.Id == "7qy6r8");
        }
        #endregion
    }
}
