using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using VerticaDevXmas2019.Domain;
using VerticaDevXmas2019.Services;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace VerticaDevXmas2019
{
    [TestFixture]
    public class SantaTests
    {
        [Test]
        public async Task Hatch3_LocateReindeers_ShouldSucceed()
        {
            var backendService = new BackendService();

            var project = backendService.GetChristmasProject(await backendService.GetParticipationResponse());

            var santaRescueResponse = await backendService.GetPostResponse<SantaRescueResponse>("/api/santarescue", new SantaRescueRequest()
            {
                Id = project.Id,
                Position = project.InitialCanePosition.CalculateCurrentPosition(project.SantaMovements)
            });

            using (var documentClient = new DocumentClient(new Uri("https://xmas2019.documents.azure.com:443/"), santaRescueResponse.Token))
            {
                var reindeerRescueLocations = santaRescueResponse.SantaZones
                    .Select(zone => documentClient.CreateDocumentQuery<ReindeerQueryResponseObject>(
                        UriFactory.CreateDocumentCollectionUri("World", "Objects"),
                        new SqlQuerySpec()
                        {
                            QueryText = "SELECT o.id, o.name, o.location, o.countryCode, @radius as radius, ST_DISTANCE(o.location, {'type': 'Point', 'coordinates':[@lon, @lat]}) as dist FROM Objects o WHERE o.countryCode = @partitionKey AND o.name = @name AND ST_DISTANCE(o.location, {'type': 'Point', 'coordinates':[@lon, @lat]}) <= @radius",
                            Parameters = new SqlParameterCollection(new[]
                            {
                                new SqlParameter("@partitionKey", zone.CountryCode),
                                new SqlParameter("@name", zone.Reindeer),
                                new SqlParameter("@lat", zone.Center.Latitude),
                                new SqlParameter("@lon", zone.Center.Longitude),
                                new SqlParameter("@radius", zone.Radius.ValueInMeters),
                            })
                        }).AsEnumerable().SingleOrDefault())
                    .Where(reindeerQueryResponseObject => reindeerQueryResponseObject != null).Select(o => new ReindeerRescueLocation()
                    {
                        Name = o.Name,
                        //NB: GeoJSON use lon/lat instead of lat/lon...
                        Position = new Point() { Latitude = o.Location.Coordinates[1], Longitude = o.Location.Coordinates[0] }
                    })
                    .ToArray();

                reindeerRescueLocations.Distinct().Count().Should().Be(8);

                var postResponse = await backendService.GetPostResponse<ReindeerRescueResponse>("/api/reindeerrescue", new ReindeerRescueRequest()
                {
                    Id = project.Id,
                    Locations = reindeerRescueLocations
                });

                postResponse.Should().NotBeNull();
                postResponse.Message.Should().StartWith("Good job");
            }
        }

        [Test]
        public async Task Hatch2_FindSantasPosition_ShouldSucceed()
        {
            var backendService = new BackendService();

            for (var i = 0; i < 3; i++)
            {
                var start = DateTime.Now;
                try
                {
                    var participationResponse = await backendService.GetParticipationResponse();

                    var project = backendService.GetChristmasProject(participationResponse);

                    var sut = await backendService.GetPostResponse<SantaRescueResponse>("/api/santarescue", new SantaRescueRequest()
                    {
                        Id = participationResponse.Id,
                        Position = project.InitialCanePosition.CalculateCurrentPosition(project.SantaMovements)
                    });

                    //Asserts
                    sut.Should().NotBeNull();
                    sut.Token.Should().NotBeNullOrEmpty();
                    sut.SantaZones.Count().Should().Be(8);

                    foreach (var zone in sut.SantaZones)
                    {
                        zone.Reindeer.Should().BeOneOf(new[]
                            {
                                "Cupid",
                                "Comet",
                                "Vixen",
                                "Prancer",
                                "Blitzen",
                                "Dancer",
                                "Donner",
                                "Dasher" });
                        zone.CountryCode.Should().NotBeNullOrEmpty();
                        zone.CityName.Should().NotBeNullOrEmpty();
                        Math.Abs(zone.Center.Latitude).Should().BeGreaterThan(0);
                        Math.Abs(zone.Center.Longitude).Should().BeGreaterThan(0);
                        zone.Radius.Value.Should().BeGreaterThan(0);
                    }

                    Console.WriteLine($"Congrats - you found Santa: {sut.ToJson()}");

                    break;
                }
                catch (ApplicationException e)
                {
                    Console.WriteLine(e.Message);
                }
                Console.WriteLine($"Time spent: {(DateTime.Now - start).TotalSeconds}");
            }
        }

        [Test]
        public async Task Hatch2_GetChristmasProject_ShouldSucceed()
        {
            var backendService = new BackendService();

            var project = backendService.GetChristmasProject(await backendService.GetParticipationResponse());

            project.Id.Should().NotBeNullOrEmpty();
            project.CanePosition.Count.Should().Be(2);
            project.InitialCanePosition.Should().NotBeNull();
            Math.Abs(project.InitialCanePosition.Latitude).Should().BeGreaterThan(0);
            Math.Abs(project.InitialCanePosition.Longitude).Should().BeGreaterThan(0);
            project.SantaMovements.Count.Should().BeGreaterThan(0);

            var canePosition = project.InitialCanePosition.CalculateCurrentPosition(project.SantaMovements);

            canePosition.Latitude.Should().NotBe(project.InitialCanePosition.Latitude);
            canePosition.Longitude.Should().NotBe(project.InitialCanePosition.Longitude);
        }

        [Test]
        public void Hatch2_KnownMovementAndPos_ShouldSucceed()
        {
            var sut = new CanePosition()
            {
                Latitude = 71.639566053691,
                Longitude = -51.1902823595313
            }.CalculateCurrentPosition(new[]{
                new SantaMovement()
                {
                    Direction = SantaMovementDirection.Right,
                    Unit = SantaMovementUnit.Meter,
                    Value = 10000
                },
                new SantaMovement()
                {
                    Direction = SantaMovementDirection.Down,
                    Unit = SantaMovementUnit.Meter,
                    Value = 7500
                },
            });

            Math.Abs(sut.Latitude - 71.572192407382).Should().BeLessThan(0.0000000000001, sut.Latitude.ToJson());
            Math.Abs(sut.Longitude - -50.9050972077072).Should().BeLessThan(0.0000000000001, sut.Longitude.ToJson());
        }

        [Test]
        public async Task Hatch1_GetProject_ShouldSucceed()
        {
            //Arrange
            var sut = new BackendService();

            //Act
            var response = await sut.GetParticipationResponse();

            //Assert
            response.Should().NotBeNull();
            response.Id.Should().NotBeNullOrEmpty();
            response.Credentials.Should().NotBeNull();
            response.Credentials.UserName.Should().Be("Participant");
            response.Credentials.Password.Should().NotBeNullOrEmpty();
        }

        [Test(Description = "From mailing with Brian Holmgård Kristensen. " +
                            "Put coordinates in https://mobisoftinfotech.com/tools/plot-multiple-points-on-map/")]
        public void Brian_VerticaSquare_ShouldSucceed()
        {
            var endPosition = new CanePosition()
            {
                Latitude = 55.6760968,
                Longitude = 12.568337100000008,
            }.Dump("Start")
                .CalculateCurrentPosition(new[]{
                    new SantaMovement()
                    {
                        Direction = SantaMovementDirection.Down,
                        Unit = SantaMovementUnit.Meter,
                        Value = 10000
                    }
                }).Dump("Move down 10 km")
                .CalculateCurrentPosition(new[]
                {
                    new SantaMovement()
                    {
                        Direction = SantaMovementDirection.Right,
                        Unit = SantaMovementUnit.Meter,
                        Value = 10000
                    }
                }).Dump("Move right 10 km")
                .CalculateCurrentPosition(new[]
                {
                    new SantaMovement()
                    {
                        Direction = SantaMovementDirection.Up,
                        Unit = SantaMovementUnit.Meter,
                        Value = 10000
                    }
                }).Dump("Move up 10 km")
                .CalculateCurrentPosition(new[]
                {
                    new SantaMovement()
                    {
                        Direction = SantaMovementDirection.Left,
                        Unit = SantaMovementUnit.Meter,
                        Value = 10000
                    }
                }).Dump("Move left 10 km");

            endPosition.Latitude.Should().Be(55.67609680);
            endPosition.Longitude.Should().Be(12.567972299220262);
        }
    }
}
