using Newtonsoft.Json;

namespace TT2Master.Shared.Raids
{
    public class RaidSeedEnemyPart
    {
        [JsonProperty("part_id")]
        public string PartId { get; set; }

        [JsonProperty("total_hp")]
        public double TotalHp { get; set; }
    }
}
