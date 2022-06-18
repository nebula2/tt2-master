using System;
using System.Collections.Generic;
using System.Text;

namespace TT2Master.Shared.Models
{
    /// <summary>
    /// Describes a raid set
    /// </summary>
    public class RaidCardSet
    {
        /// <summary>
        /// ID of deck in save file
        /// </summary>
        public string DeckId { get; set; }

        /// <summary>
        /// Maximum amount of card slots a deck can have
        /// </summary>
        public static int MaxSlotCount = 3;

        /// <summary>
        /// First card slot
        /// </summary>
        public RaidCard Slot0 { get; set; }

        /// <summary>
        /// Second card slot
        /// </summary>
        public RaidCard Slot1 { get; set; }

        /// <summary>
        /// Third card slot
        /// </summary>
        public RaidCard Slot2 { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        public RaidCardSet()
        {
            DeckId = "";
            Slot0 = new RaidCard();
            Slot1 = new RaidCard();
            Slot2 = new RaidCard();
        }

        /// <summary>
        /// Get amount of used card slots
        /// </summary>
        /// <returns></returns>
        public int GetUsedSlotCount()
        {
            int i = 0;

            if (Slot0 != null)
            {
                i++;
            }

            if (Slot1 != null)
            {
                i++;
            }

            if (Slot2 != null)
            {
                i++;
            }

            return i;
        }

        /// <summary>
        /// Gets the raid card for given slot id
        /// </summary>
        /// <param name="slotId">id of slot starting by 0.</param>
        /// <returns></returns>
        public RaidCard GetRaidCard(int slotId)
        {
            return slotId switch
            {
                0 => Slot0,
                1 => Slot1,
                2 => Slot2,
                _ => null,
            };
        }
    }
}
