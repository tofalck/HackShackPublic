using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace VerticaDevXmas2019.Domain
{
    public class CanePosition: ValueObject
    {
        private const double EarthRadius = 6378.137d; // radius of the earth in kilometers
        private const double OneMeterInDegree = (1 / (2 * Math.PI / 360 * EarthRadius) / 1000);  // 1 meter in degree

        [JsonProperty("lat")]
        public double Latitude { get; internal set; }
        [JsonProperty("lon")]
        public double Longitude { get; internal set; }

        public CanePosition CalculateCurrentPosition(IEnumerable<SantaMovement> santaMovements)
        {
            var longitudeMovement = 0d;
            var latitudeMovement = 0d;
            foreach (var santaMovement in santaMovements)
            {
                var delta = (GetMovementInMeters(santaMovement));
                if (santaMovement.Direction == SantaMovementDirection.Up || santaMovement.Direction == SantaMovementDirection.Down)
                {
                    latitudeMovement += delta;
                }
                else if (santaMovement.Direction == SantaMovementDirection.Left || santaMovement.Direction == SantaMovementDirection.Right)
                {
                    longitudeMovement += delta;
                }
                else throw new ArgumentOutOfRangeException();
            }

            longitudeMovement *= OneMeterInDegree;
            latitudeMovement *= OneMeterInDegree;

            var newLatitude = this.Latitude + latitudeMovement;
            var newLongitude = (this.Longitude) + (longitudeMovement / Math.Cos(this.Latitude * (Math.PI / 180)));

            return new CanePosition()
            {
                Latitude = newLatitude,
                Longitude = newLongitude
            };
        }

        private static double GetMovementInMeters(SantaMovement santaMovement)
        {
            var movementValue = (santaMovement.Unit == SantaMovementUnit.Foot ? santaMovement.Value * 0.304800610d : santaMovement.Unit == SantaMovementUnit.Kilometer ? santaMovement.Value * 1000 : santaMovement.Value);
            movementValue = Math.Abs(movementValue) * ((santaMovement.Direction == SantaMovementDirection.Down || santaMovement.Direction == SantaMovementDirection.Left) ? -1 : 1);
            return movementValue;
        }
    }
}