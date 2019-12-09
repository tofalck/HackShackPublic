using Newtonsoft.Json;

namespace VerticaDevXmas2019.Domain
{
    public class SantaRescueRequest: EntityObject
    {
        [JsonProperty("position")]
        public CanePosition Position { get; internal set; }
    }
}