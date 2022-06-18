using System.Collections.Generic;
using System.IO;
using TT2Master.Shared.Models;
using TT2Master.Shared.Raids;
using Xunit;

namespace TT2Master.Shared.Tests
{
    public class RaidSeedParserTests
    {
        private static string GetRaidSeedString()
        {
            return File.ReadAllText("Resources//Seed//raid_seed_example.json");
        }

        [Fact]
        public void RaidSeedParser_ShouldReturnFalse()
        {
            var parser = new RaidSeedParser();
            Assert.False(parser.LoadSeedsFromJsonString(null));
            Assert.False(parser.LoadSeedsFromJsonString("Fusselbirne"));
            Assert.False(parser.LoadSeedsFromJsonString("[{\"Tier\": \"1\"}]"));
        }

        [Fact]
        public void RaidSeedParser_ShouldParseCorrectly()
        {
            var parser = new RaidSeedParser();
            Assert.True(parser.LoadSeedsFromJsonString(GetRaidSeedString()));
            Assert.NotNull(parser.Seeds);
        }
    }
}
