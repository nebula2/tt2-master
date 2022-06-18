namespace TT2Master.Shared.Models
{
    /// <summary>
    /// Describes a row of "RaidAreaInfo.csv"
    /// </summary>
    public class RaidAreaInfo
    {
        public string AreaID { get; set; }
        public string BorderColor { get; set; }
        public string OverlayColor { get; set; }
        public string FogBackMin { get; set; }
        public string FogBackMax { get; set; }
        public string FogFrontMin { get; set; }
        public string FogFrontMax { get; set; }
        public string BonusTypeA { get; set; }
        public double BonusAmountA { get; set; }
        public string BonusTypeB { get; set; }
        public double BonusAmountB { get; set; }
        public string BonusTypeC { get; set; }
        public double BonusAmountC { get; set; }
        public string BonusTypeD { get; set; }
        public double BonusAmountD { get; set; }
        public string BonusTypeE { get; set; }
        public double BonusAmountE { get; set; }
        public int IconCount { get; set; }
    }
}
