using Newtonsoft.Json;

namespace VerticaDevXmas2019.Domain
{
    public class ParticipationResponse: EntityObject
    {
        [JsonProperty("credentials")]
        public Credentials Credentials { get; set; }
    }
}