using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Elasticsearch.Net;
using FluentAssertions;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Nest;
using Newtonsoft.Json;
using VerticaDevXmas2019.Domain;
using Microsoft.Azure.Documents.Spatial;
using Point = VerticaDevXmas2019.Domain.Point;


namespace VerticaDevXmas2019.Services
{
    public class BackendService
    {
        public async Task GetReindeerLocations(Action<ChristmasProject, IEnumerable<ReindeerRescueLocation>> handleFoundReindeers)
        {
            var project = await GetChristmasProjectAsync(await GetParticipationResponseAsync());

            var santaRescueResponse = await GetPostResponseAsync<SantaRescueResponse>("/api/santarescue",
                new SantaRescueRequest()
                {
                    Id = project.Id,
                    Position = project.InitialCanePosition.CalculateCurrentPosition(project.SantaMovements)
                });

            using (var documentClient = new DocumentClient(
                new Uri("https://xmas2019.documents.azure.com:443/"),
                santaRescueResponse.Token))
            {
                handleFoundReindeers(project, (from zone in santaRescueResponse.SantaZones
                    let foundReindeer = (from reindeerQueryResponseObject in documentClient.CreateDocumentQuery<ReindeerQueryResponseObject>(
                            UriFactory.CreateDocumentCollectionUri("World", "Objects"),
                            new FeedOptions() {PartitionKey = new PartitionKey(zone.CountryCode)})
                        where (reindeerQueryResponseObject.Name == zone.Reindeer &&
                               zone.GetCenter().Distance(reindeerQueryResponseObject.Location) <= zone.Radius.ValueInMeters)
                        select reindeerQueryResponseObject).AsEnumerable().Single(o => o != null)
                    select new ReindeerRescueLocation()
                    {
                        Name = foundReindeer.Name,
                        //NB: GeoJSON use lon/lat instead of lat/lon...
                        //TODO: We have our own stand in for Azure's point to serialize properly - could/should be fixed somehow
                        Position = new Point() {Latitude = foundReindeer.Location.Position.Latitude, Longitude = foundReindeer.Location.Position.Longitude}
                    }).ToArray());
            }
        }

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

        public async Task<SantaRescueResponse> Get()
        {
            var project = await GetChristmasProjectAsync(await GetParticipationResponseAsync());

            return await GetPostResponseAsync<SantaRescueResponse>("/api/santarescue",
                new SantaRescueRequest()
                {
                    Id = project.Id,
                    Position = project.InitialCanePosition.CalculateCurrentPosition(project.SantaMovements)
                });
        }
    }
}