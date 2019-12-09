using System.Collections.Generic;
using Newtonsoft.Json;

namespace VerticaDevXmas2019.Domain
{
    public class Point: ValueObject
    {
        [JsonProperty("lat")]
        public double Latitude { get; internal set; }
        [JsonProperty("lon")]
        public double Longitude { get; internal set; }

        private sealed class LatitudeLongitudeEqualityComparer : IEqualityComparer<Point>
        {
            public bool Equals(Point x, Point y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.Latitude.Equals(y.Latitude) && x.Longitude.Equals(y.Longitude);
            }

            public int GetHashCode(Point obj)
            {
                unchecked
                {
                    return (obj.Latitude.GetHashCode() * 397) ^ obj.Longitude.GetHashCode();
                }
            }
        }

        public static IEqualityComparer<Point> LatitudeLongitudeComparer { get; } = new LatitudeLongitudeEqualityComparer();
    }
}