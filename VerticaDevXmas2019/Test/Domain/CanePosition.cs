using System;
using System.Collections.Generic;

namespace VerticaDevXmas2019.Domain
{
    public class CanePosition: Point
    {
        private const double EarthRadius = 6378.137d; // radius of the earth in kilometers
        private const double OneMeterInDegree = (1 / (2 * Math.PI / 360 * EarthRadius) / 1000);  // 1 meter in degree

        private double GetMovementInMeters(SantaMovement santaMovement) => Math.Abs(santaMovement.Value.InMeters(santaMovement.Unit)) * ((santaMovement.Direction == SantaMovementDirection.Down || santaMovement.Direction == SantaMovementDirection.Left) ? -1 : 1);

        public CanePosition CalculateCurrentPosition(IEnumerable<SantaMovement> santaMovements)
        {
            var longitudeMovement = 0d;
            var latitudeMovement = 0d;
            foreach (var santaMovement in santaMovements)
            {
                var delta = (GetMovementInMeters(santaMovement));
                switch (santaMovement.Direction)
                {
                    case SantaMovementDirection.Up:
                    case SantaMovementDirection.Down:
                        latitudeMovement += delta;
                        break;
                    case SantaMovementDirection.Left:
                    case SantaMovementDirection.Right:
                        longitudeMovement += delta;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
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
    }
}