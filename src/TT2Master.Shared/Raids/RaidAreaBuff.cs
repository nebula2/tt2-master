using Newtonsoft.Json;

namespace TT2Master.Shared.Raids
{
    public class RaidAreaBuff
    {
        [JsonProperty("BonusType")]
        public string BonusType { get; set; }

        [JsonProperty("BonusAmount")]
        public double BonusAmount { get; set; }
    }
}
