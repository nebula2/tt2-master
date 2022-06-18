using TT2Master.Model.Arti;
using TT2Master.Shared.Models;

namespace TT2Master
{
    /// <summary>
    /// Artifact for Export
    /// </summary>
    public class ExportArtifactLvl
    {
        public string ID { get; set; }
        public double Level { get; set; }

        public ExportArtifactLvl(Artifact art)
        {
            ID = art.ID;
            Level = art.Level;
        }
    }
}