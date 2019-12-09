using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Nest;
using Newtonsoft.Json;
using VerticaDevXmas2019.Domain;

namespace VerticaDevXmas2019.Services
{
    public class BackendService
    {
        public async Task<ChristmasProject> GetChristmasProjectAsync(ParticipationResponse participationResponse)
        {
            var esClient = new ElasticClient(
                "xmas2019:ZXUtY2VudHJhbC0xLmF3cy5jbG91ZC5lcy5pbyRlZWJjNmYyNzcxM2Q0NTE5OTcwZDc1Yzg2MDUwZTM2MyQyNDFmMzQ3OWNkNzg0ZTUyOTRkODk5OTViMjg0MjAyYg==",
                new BasicAuthenticationCredentials(participationResponse.Credentials.UserName, participationResponse.Credentials.Password));

            var project = await esClient.GetAsync<ChristmasProject>(participationResponse.Id, descriptor => descriptor.Index("santa-trackings"));

            return project.Source;
        }

        public async Task<ParticipationResponse> GetParticipationResponseAsync()
        {
            return await GetPostResponseAsync<ParticipationResponse>("/api/participate", new
            {
                fullName = "Torben Falck",
                emailAddress = "tof@falcknet.com",
                subscribeToNewsletter = false
            });
        }

        public async Task<T> GetPostResponseAsync<T>(string resource, object payload)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("https://vertica-xmas2019.azurewebsites.net");
                var httpResponseMessage = await httpClient.PostAsync(resource, new StringContent(payload.ToJson(new JsonSerializerSettings() { Formatting = Formatting.None, }), Encoding.UTF8, "application/json"));
                var body = await httpResponseMessage.Content.ReadAsStringAsync();
                if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
                {
                    throw new ApplicationException(body);
                }
                var response = body.FromJson<T>();
                return response;
            }
        }
    }
}