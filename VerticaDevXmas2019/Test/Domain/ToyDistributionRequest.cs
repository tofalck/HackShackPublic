using System.Collections.Generic;
using Newtonsoft.Json;

namespace VerticaDevXmas2019.Domain
{
    public class ToyDistributionRequest: EntityObject
    {
        [JsonProperty("toyDistribution")]
        public IEnumerable<ToyDistribution> ToyDistribution { get; set; }
    }
}