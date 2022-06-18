namespace TT2Master.Shared.Models
{
    /// <summary>
    /// Describes a row of "RaidEnemyInfo.csv"
    /// </summary>
    public class RaidEnemyInfo
    {
        public string EnemyId { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public string EnchantColor { get; set; }
        public double BodyHeadMult { get; set; }
        public double BodyTorsoMult { get; set; }
        public double BodyArmsMult { get; set; }
        public double BodyLegsMult { get; set; }
        public double ArmorHeadMult { get; set; }
        public double ArmorTorsoMult { get; set; }
        public double ArmorArmsMult { get; set; }
        public double ArmorLegsMult { get; set; }
        public double TotalInArmour { get; set; }
        public double TotalInBody { get; set; }
        public double BodyPhysicalMult { get; set; }
        public double BodyElementalMult { get; set; }
        public double BodyArcaneMult { get; set; }
        public double ArmorPhysicalMult { get; set; }
        public double ArmorElementalMult { get; set; }
        public double ArmorArcaneMult { get; set; }
        public double HPWeightBody { get; set; }
        public double HPWeightArmor { get; set; }
        public double HPWeightHead { get; set; }
        public double HPWeightChest { get; set; }
        public double HPWeightArm { get; set; }
        public double HPWeightLeg { get; set; }
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
    }
}
