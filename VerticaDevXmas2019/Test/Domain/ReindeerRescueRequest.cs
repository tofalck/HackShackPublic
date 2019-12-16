using System.Collections.Generic;
using Newtonsoft.Json;

namespace VerticaDevXmas2019.Domain
{
    public class ReindeerRescueResponse : ValueObject
    {
        [JsonProperty("toyDistributionXmlUrl")]
        public string ToyDistributionXmlUrl { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }

    public class ReindeerRescueRequest : EntityObject
    {
        [JsonProperty("locations")]
        public IEnumerable<ReindeerRescueLocation> Locations { get; set; }
    }
}