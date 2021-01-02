using Newtonsoft.Json;

namespace Cherokee.Models
{
    public sealed class RavenRockNounEntry : RavenRockEntry
    {
        [JsonProperty("type")]
        public const string Type = "n";
        [JsonProperty("singular_en")]
        public string English { get; set; }
        [JsonProperty("singular_syllabary")]
        public string Syllabary { get; set; }
        [JsonProperty("singular_romanized")]
        public string Romanized { get; set; }
    }
}
