using TT2Master.Shared.Assets.Maps;
using Xunit;

namespace TT2Master.Shared.Tests
{
    public class ClanTraitTests
    {
        [Fact]
        public void ClanTraitMap_SkipExpressionWorks()
        {
            var func = ClanTraitMap.GetSkipExpression();

            string[] row = new string[] { "1" };
            Assert.False(func(row));

            row = new string[] { "ClanTrait" };
            Assert.False(func(row));

            row = new string[] { "d" };
            Assert.True(func(row));
        }
    }
}
