using TT2Master.Model.Arti;
using TT2Master.Shared.Models;

namespace TT2Master
{
    /// <summary>
    /// Artifact for Export
    /// </summary>
    public class ExportArtifact
    {
        public string ID { get; set; }
        public int SortId { get; set; }
        public double Level { get; set; }
        public double RelicsSpent { get; set; }
        public string Name { get; set; }
        public double MaxLevel { get; set; }
        public string Effect { get; set; }
        public double EffectPerLevel { get; set; }
        public double GrowthMaximum { get; set; }
        public double GrowthRate { get; set; }
        public double GrowthExpo { get; set; }
        public double DamageBonus { get; set; }
        public double CostCoefficient { get; set; }
        public double CostExpo { get; set; }

        public ExportArtifact(Artifact art)
        {
            ID = art.ID;
            SortId = art.SortIndex;
            Level = art.Level;
            RelicsSpent = art.RelicsSpent;
            Name = art.Name;
            MaxLevel = art.MaxLevel;
            Effect = art.Effect;
            EffectPerLevel = art.EffectPerLevel;
            GrowthExpo = art.GrowthExpo;
            DamageBonus = art.DamageBonus;
            CostCoefficient = art.CostCoefficient;
            CostExpo = art.CostExpo;
        }
    }
}
