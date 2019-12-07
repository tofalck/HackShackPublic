using System.Collections.Generic;
using Newtonsoft.Json;

namespace VerticaDevXmas2019.Domain
{
    public class SantaRescueResponse: ValueObject
    {
        [JsonProperty("zones")]
        public IEnumerable<SantaZone> SantaZones { get; internal set; }

        [JsonProperty("token")]
        public string Token { get; internal set; }
    }
}