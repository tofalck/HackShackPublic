using Newtonsoft.Json;

namespace VerticaDevXmas2019.Domain
{
    public class ReindeerRescueLocation : ValueObject
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("position")]
        public Point Position { get; set; }
    }
}