using Newtonsoft.Json;

namespace VerticaDevXmas2019.Domain
{
    public class ReindeerQueryResponseObject : EntityObject
    {
        [JsonProperty("countryCode")]
        public string CountryCode { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("location")]
        //public ReindeerLocation Location { get; set; }
        public Microsoft.Azure.Documents.Spatial.Point Location { get; set; }
    }
}