using Newtonsoft.Json;

namespace VerticaDevXmas2019.Domain
{
    public class ReindeerQueryResponseObject : EntityObject
    {
        [JsonProperty("countryCode")]
        public string CountryCode { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("location")]
        public ReindeerLocation Location { get; set; }
        [JsonProperty("dist")]
        public double Distance { get; set; }
        [JsonProperty("radius")]
        public double Radius { get; set; }
    }
}