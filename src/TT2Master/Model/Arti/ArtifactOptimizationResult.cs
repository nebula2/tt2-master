using System.Collections.Generic;

namespace TT2Master.Model.Arti
{
    public class ArtifactOptimizationResult
    {
        public List<ArtifactToOptimize> OptimizedList { get; set; } = new List<ArtifactToOptimize>();

        public bool IsMessageNeeded { get; set; } = false;

        public string Header { get; set; }

        public string Content { get; set; }
    }
}
