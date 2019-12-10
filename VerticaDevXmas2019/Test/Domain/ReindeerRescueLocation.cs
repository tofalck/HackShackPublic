using System.Collections.Generic;
using Newtonsoft.Json;

namespace VerticaDevXmas2019.Domain
{
    public class ReindeerRescueLocation : ValueObject
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("position")]
        public Point Position { get; set; }
        //public Microsoft.Azure.Documents.Spatial.Point Position { get; set; }

        private sealed class NamePositionEqualityComparer : IEqualityComparer<ReindeerRescueLocation>
        {
            public bool Equals(ReindeerRescueLocation x, ReindeerRescueLocation y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return string.Equals(x.Name, y.Name) && Equals(x.Position, y.Position);
            }

            public int GetHashCode(ReindeerRescueLocation obj)
            {
                unchecked
                {
                    return ((obj.Name != null ? obj.Name.GetHashCode() : 0) * 397) ^ (obj.Position != null ? obj.Position.GetHashCode() : 0);
                }
            }
        }

        public static IEqualityComparer<ReindeerRescueLocation> NamePositionComparer { get; } = new NamePositionEqualityComparer();
    }
}