using Newtonsoft.Json;

namespace VerticaDevXmas2019.Domain
{
    public class ToyDistribution: ValueObject
    {
        [JsonProperty("childName")]
        public string ChildName { get; set; }
        [JsonProperty("toyName")]
        public string ToyName { get; set; }
    }
}