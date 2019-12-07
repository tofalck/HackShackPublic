using Newtonsoft.Json;

namespace VerticaDevXmas2019.Domain
{
    public class ElasticSearchCredentials
    {
        [JsonProperty("username")]
        public string UserName { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}