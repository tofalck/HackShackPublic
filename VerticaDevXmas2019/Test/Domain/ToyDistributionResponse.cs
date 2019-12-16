using Newtonsoft.Json;

namespace VerticaDevXmas2019.Domain
{
    public class ToyDistributionResponse: ValueObject
    {
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}