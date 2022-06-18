namespace TT2Master.Shared.Models
{
    /// <summary>
    /// Describes a Pet
    /// </summary>
    public class Pet
    {
        /// <summary>
        /// Identifier
        /// </summary>
        public string PetId { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string PetName { get; set; }

        /// <summary>
        /// Bonus type this pet gives
        /// </summary>
        public string BonusType { get; set; }

        /// <summary>
        /// The type of this pet (legacy or exotic)
        /// </summary>
        public string PetType { get; set; }

        /// <summary>
        /// Level this pet has
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// True, if the pet is currently active
        /// </summary>
        public bool IsEquipped { get; set; }
    }
}
