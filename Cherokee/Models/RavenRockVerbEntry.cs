using Newtonsoft.Json;

namespace Cherokee.Models
{
    public sealed class RavenRockVerbEntry : RavenRockEntry
    {
        [JsonProperty("type")]
        public const string Type = "v";
        [JsonProperty("third_present_en")]
        public string English { get; set; }

        [JsonProperty("third_present_syllabary")]
        public string ThirdPresentSyllabary { get; set; }
        [JsonProperty("third_present_romanized")]
        public string ThirdPresentRomanized { get; set; }

        [JsonProperty("first_present_syllabary")]
        public string FirstPresentSyllabary { get; set; }
        [JsonProperty("first_present_romanized")]
        public string FirstPresentRomanized { get; set; }

        [JsonProperty("completive_past_syllabary")]
        public string CompletiveSyllabary { get; set; }
        [JsonProperty("completive_past_romanized")]
        public string CompletiveRomanized { get; set; }

        [JsonProperty("continuous_syllabary")]
        public string IncompletiveSyllabary { get; set; }
        [JsonProperty("continuous_romanized")]
        public string IncompletiveRomanized { get; set; }

        [JsonProperty("immediate_syllabary")]
        public string ImmediateSyllabary { get; set; }
        [JsonProperty("immediate_romanized")]
        public string ImmediateRomanized { get; set; }

        [JsonProperty("infinitive_syllabary")]
        public string InfinitiveSyllabary { get; set; }
        [JsonProperty("infinitive_romanized")]
        public string InfinitiveRomanized { get; set; }
    }
}
