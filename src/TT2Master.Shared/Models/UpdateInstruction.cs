namespace TT2Master.Shared.Models
{
    /// <summary>
    /// Update instruction which should be executed by the client
    /// </summary>
    public class UpdateInstruction
    {
        /// <summary>
        /// Identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identifier of optional parent app update request
        /// </summary>
        public int ParentAppUpdateRequestId { get; set; }

        /// <summary>
        /// Parent app update request identifier
        /// </summary>
        public AppUpdateRequest ParentAppUpdateRequest { get; set; }

        /// <summary>
        /// User friendly description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Type of update which should be performed
        /// </summary>
        public UpdateType ItemType { get; set; }

        /// <summary>
        /// Optional parameters given in addition to the update type
        /// </summary>
        public string Parameters { get; set; }
    }
}
