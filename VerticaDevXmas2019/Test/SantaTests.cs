using System;
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
        public async Task Hatch2_FindSantasPosition_ShouldSucceed()
        {
            var backendFacade = new BackendService();

            for (var i = 0; i < 3; i++)
            {
                var start = DateTime.Now;
                try
                {
                    var participationResponse = await backendFacade.GetParticipationResponse();

                    var project = backendFacade.GetProject(participationResponse);

                    var canePosition = project.SantasCanePosition.GetNewPosition(project.SantaMovements);

                    Console.WriteLine($"{i + 1}. Lat/Lon: {canePosition.ToJson()}");

                    var sut = await backendFacade.GetPostResponse<SantaRescueResponse>("/api/santarescue", new SantaRescueRequest()
                    {
                        Id = participationResponse.Id,
                        Position = canePosition
                    });

                    sut.Should().NotBeNull();

                    Console.WriteLine($"Congrats - you found Santa at: {sut.ToJson()}");

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
        public void Hatch2_KnownMovementAndPos_ShouldSucceed()
        {
            var sut = new CanePosition()
            {
                Latitude = 71.639566053691,
                Longitude = -51.1902823595313
            }.GetNewPosition(new[]{
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
        }

        [Test(Description = "From mailing with Brian Holmgård Kristensen. Put coordinates in https://mobisoftinfotech.com/tools/plot-multiple-points-on-map/")]
        public void Brian_VerticaSquare_ShouldSucceed()
        {
            var sut = new CanePosition()
                {
                    Latitude = 55.6760968,
                    Longitude = 12.568337100000008,
                }.Dump("Start")
                .GetNewPosition(new[]{
                    new SantaMovement()
                    {
                        Direction = SantaMovementDirection.Down,
                        Unit = SantaMovementUnit.Meter,
                        Value = 10000
                    }
                }).Dump("Move down 10 km")
                .GetNewPosition(new[]
                {
                    new SantaMovement()
                    {
                        Direction = SantaMovementDirection.Right,
                        Unit = SantaMovementUnit.Meter,
                        Value = 10000
                    }
                }).Dump("Move right 10 km")
                .GetNewPosition(new[]
                {
                    new SantaMovement()
                    {
                        Direction = SantaMovementDirection.Up,
                        Unit = SantaMovementUnit.Meter,
                        Value = 10000
                    }
                }).Dump("Move up 10 km")
                .GetNewPosition(new[]
                {
                    new SantaMovement()
                    {
                        Direction = SantaMovementDirection.Left,
                        Unit = SantaMovementUnit.Meter,
                        Value = 10000
                    }
                }).Dump("Move left 10 km");

            sut.Latitude.Should().Be(55.67609680);
            sut.Longitude.Should().Be(12.567972299220262);
        }
    }
}
