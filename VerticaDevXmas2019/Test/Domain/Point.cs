using Newtonsoft.Json;

namespace VerticaDevXmas2019.Domain
{
    public class Point: ValueObject
    {
        [JsonProperty("lat")]
        public double Latitude { get; internal set; }
        [JsonProperty("lon")]
        public double Longitude { get; internal set; }
    }
}