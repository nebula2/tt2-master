using System;

namespace TT2MasterAdministrationApp.Shared
{
    public class JfProgressEventArgs
    {
        public int PercentDone => FinishedWorkItemCount * 100 / Math.Max(WorkloadCount, 1);
        public int FinishedWorkItemCount { get; set; }
        public int WorkloadCount { get; set; }
        public string Message { get; set; }
        public bool IsFinished { get; set; }
    }
}
