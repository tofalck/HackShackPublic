using Newtonsoft.Json;

namespace VerticaDevXmas2019.Domain
{
    public class ReindeerLocation: ValueObject
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("coordinates")]
        public double[] Coordinates { get; set; }
    }
}