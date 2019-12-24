using System;
using System.Collections.Generic;
using System.Linq;

namespace VerticaDevXmas2019.Domain
{
    public class Santa
    {
        public IEnumerable<ToyDistribution> DistributePresentsToChildren(ToyDistributionProblem toyDistributionProblem)
        {
            var distributedToys = new List<Toy>();

            var childrenWithOneMatchInSantasBag = toyDistributionProblem.Children.Where(child => toyDistributionProblem.Toys.Count(toyInSantasBag => child.Wishes.Toys.Select(toyWish => toyWish.Name).Contains(toyInSantasBag.Name)) == 1).ToArray();

            ToyDistribution CreateDistribution(Child child, Func<IEnumerable<Toy>, Toy> toysFilter)
            {
                var toy = toysFilter(toyDistributionProblem.Toys.Except(distributedToys));
                distributedToys.Add(toy);
                return new ToyDistribution()
                {
                    ChildName = child.Name,
                    ToyName = toy.Name
                };
            }

            Func<Toy, bool> ToyWithSameName(Child child)
            {
                return toy => child.Wishes.Toys.Any(toyWish => toyWish.Name == toy.Name);
            }

            foreach (var child in childrenWithOneMatchInSantasBag)
            {
                yield return CreateDistribution(child, toys => toys.Single(ToyWithSameName(child)));
            }

            foreach (var child in toyDistributionProblem.Children.Except(childrenWithOneMatchInSantasBag))
            {
                //TODO: We have a bug somewhere in this since we sometimes run out of presents / no match 
                yield return CreateDistribution(child, toys => toys.First(ToyWithSameName(child)));
            }
        }
    }
}