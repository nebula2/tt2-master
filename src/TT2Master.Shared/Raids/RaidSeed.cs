using Newtonsoft.Json;
using System.Collections.Generic;

namespace TT2Master.Shared.Raids
{
    public class RaidSeed
    {
        [JsonProperty("spawn_sequence")]
        public string[] SpawnSequence { get; set; }
        
        [JsonProperty("Tier")]
        public string Tier { get; set; }

        [JsonProperty("Level")]
        public string Level { get; set; }

        [JsonProperty("enemy_names")]
        public string[] EnemyNames { get; set; }

        public List<RaidSeedEnemy> Enemies { get; set; }

        [JsonProperty("area_buffs")]
        public List<RaidAreaBuff> AreaBuffs { get; set; }
    }
}
