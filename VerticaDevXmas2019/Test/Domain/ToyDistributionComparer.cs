using System.Collections.Generic;

namespace VerticaDevXmas2019.Domain
{
    public class ToyDistributionComparer : EqualityComparer<ToyDistribution>
    {
        public override bool Equals(ToyDistribution x, ToyDistribution y)
        {
            return x.ChildName == y.ChildName && x.ToyName == y.ToyName;
        }

        public override int GetHashCode(ToyDistribution obj)
        {
            return obj.ChildName.GetHashCode() ^ obj.ToyName.GetHashCode();
        }
    }
}