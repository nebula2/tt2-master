using Newtonsoft.Json;
using System.Collections.Generic;

namespace TT2Master.Shared.Raids
{
    public class RaidSeedEnemy
    {
        public string Name { get; set; }

        [JsonProperty("enemy_id")]
        public string EnemyId { get; set; }

        [JsonProperty("total_hp")]
        public double TotalHp { get; set; }

        [JsonProperty("parts")]
        public List<RaidSeedEnemyPart> Parts { get; set; }

        [JsonProperty("area_debuffs")]
        public List<RaidAreaBuff> AreaDebuffs { get; set; }
    }
}
