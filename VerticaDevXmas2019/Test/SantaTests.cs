using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using VerticaDevXmas2019.Domain;
using VerticaDevXmas2019.Services;

namespace VerticaDevXmas2019
{
    [TestFixture]
    public class SantaTests
    {
        [Test]
        public async Task Hatch4_GetToyDistributionList_ShouldSucceed()
        {
            var backendService = new BackendService();

            var (christmasProject, reindeerLocations) = backendService.GetReindeerLocations();

            var reindeerRescueResponse = await backendService.GetPostResponseAsync<ReindeerRescueResponse>("/api/reindeerrescue",
                new ReindeerRescueRequest()
                {
                    Id = christmasProject.Id,
                    Locations = reindeerLocations
                });

            var toyDistributionProblem = backendService.GetToyDistribution(reindeerRescueResponse);

            toyDistributionProblem.Should().NotBeNull();
            toyDistributionProblem.Toys.Count().Should().Be(15);
            toyDistributionProblem.Children.Length.Should().Be(15);
            toyDistributionProblem.Children.All(child => 
                string.IsNullOrEmpty(child.Name) == false && 
                child.Wishes.Toys.Length <= 3 && 
                child.Wishes.Toys.All(toy => 
                    string.IsNullOrEmpty(toy.Name) == false && 
                    toyDistributionProblem.Toys.SingleOrDefault(availableToy => availableToy.Name == toy.Name) != null));

            var santa = new Santa();
            var toyDistributions = santa.DistributePresentsToChildren(toyDistributionProblem);

            toyDistributions.Should().NotBeNull();
            toyDistributions.Count().Should().Be(15);
            toyDistributions.Distinct(new ToyDistributionComparer()).Count().Should().Be(15);
            var toyDistributionResponse = await backendService.GetPostResponseAsync<ToyDistributionResponse>("/api/toydistribution", new ToyDistributionRequest()
            {
                Id = christmasProject.Id,
                ToyDistribution = toyDistributions
            });
            toyDistributionResponse.Should().NotBeNull();
            toyDistributionResponse.Message.Should().NotBeNullOrEmpty();
        }

        [Test]
        public async Task Hatch3_LocateReindeers_ShouldSucceed()
        {
            var backendService = new BackendService();

            var (christmasProject, reindeerLocations) = backendService.GetReindeerLocations();

            christmasProject.Should().NotBeNull();
            reindeerLocations.Distinct().Count().Should().Be(8);

            var reindeerRescueResponse = await backendService.GetPostResponseAsync<ReindeerRescueResponse>("/api/reindeerrescue",
                new ReindeerRescueRequest()
                {
                    Id = christmasProject.Id,
                    Locations = reindeerLocations
                });

            reindeerRescueResponse.Should().NotBeNull();
            reindeerRescueResponse.Message.Should().StartWith("Good job");
            reindeerRescueResponse.ToyDistributionXmlUrl.Should().NotBeNullOrEmpty();
        }

        [Test]
        public async Task Hatch2_FindSantasPosition_ShouldSucceed()
        {
            var backendService = new BackendService();

            var start = DateTime.Now;
            try
            {
                var participationResponse = await backendService.GetParticipationResponseAsync();

                var project = await backendService.GetChristmasProjectAsync(participationResponse);

                var sut = await backendService.GetPostResponseAsync<SantaRescueResponse>("/api/santarescue", new SantaRescueRequest()
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
            }
            catch (ApplicationException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            Console.WriteLine($"Time spent: {(DateTime.Now - start).TotalSeconds}");
        }

        [Test]
        public async Task Hatch2_GetChristmasProject_ShouldSucceed()
        {
            var backendService = new BackendService();

            var project = await backendService.GetChristmasProjectAsync(await backendService.GetParticipationResponseAsync());

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
            var response = await sut.GetParticipationResponseAsync();

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
