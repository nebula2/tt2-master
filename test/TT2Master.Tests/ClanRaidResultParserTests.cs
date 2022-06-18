using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TT2Master.Model.Raid;
using Xunit;

namespace TT2Master.Tests
{
    public class ClanRaidResultParserTests
    {
        #region Helper
        private DBRepository GetDBRepository() => new DBRepository("tt2master.db3");

        private string GetCsvString(string s) => File.ReadAllText($"Resources\\raidexport{s}.txt");
        #endregion

        #region Unit Tests
        [Fact]
        public void ClanRaidResultParser_ShouldInstantiate()
        {
            var parser = new ClanRaidResultParser(GetDBRepository(), 1);
            Assert.NotNull(parser);
        }
        #endregion

        #region Integration Tests

        [Theory]
        [InlineData("3-22")]
        [InlineData("3-25")]
        [InlineData("3-25_headerShit")]
        [InlineData("540")]
        public async Task ClanRaidResultParser_ShouldLoadFromCsvString(string name)
        {
            var parser = new ClanRaidResultParser(GetDBRepository(), 1, GetCsvString(name));

            bool result = await parser.SaveRaidResultAsync();

            Assert.True(result);
            Assert.True(parser.Results.Count > 0);

            // check for correct ID
            var sortedResult = parser.Results.OrderBy(x => x.ID).ToList();
            Assert.Equal(sortedResult.Count - 1, sortedResult.Last().ID - sortedResult[0].ID);
        }
        #endregion
    }
}
