using Newtonsoft.Json;

namespace VerticaDevXmas2019.Domain
{
    public class SantaRescueRequest: EntityObject
    {
        [JsonProperty("id")]
        public string Id { get; internal set; }

        [JsonProperty("position")]
        public CanePosition Position { get; internal set; }
    }
}