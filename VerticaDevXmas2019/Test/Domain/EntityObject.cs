using Newtonsoft.Json;

namespace VerticaDevXmas2019.Domain
{
    public class EntityObject
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}