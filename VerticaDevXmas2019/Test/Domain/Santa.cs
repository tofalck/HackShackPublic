using System.Collections.Generic;
using System.Linq;

namespace VerticaDevXmas2019.Domain
{
    public class Santa
    {
        public IEnumerable<ToyDistribution> DistributePresentsToChildren(ToyDistributionProblem toyDistributionProblem)
        {
            var distributedToys = new List<Toy>();

            var childrenWithOneMatchInSantasBag = toyDistributionProblem.Children.Items.Where(child => toyDistributionProblem.Toys.Items.Count(toyInSantasBag => child.Wishes.Toys.Items.Select(toyWish => toyWish.Name).Contains(toyInSantasBag.Name)) == 1).ToArray();

            ToyDistribution CreateDistribution(Child child, Toy toy)
            {
                distributedToys.Add(toy);
                return new ToyDistribution()
                {
                    ChildName = child.Name,
                    ToyName = toy.Name
                };
            }

            foreach (var child in childrenWithOneMatchInSantasBag)
            {
                yield return CreateDistribution(child, toyDistributionProblem.Toys.Items.Except(distributedToys).Single(toy => child.Wishes.Toys.Items.Any(toyWish => toyWish.Name == toy.Name)));
            }

            foreach (var child in toyDistributionProblem.Children.Items.Except(childrenWithOneMatchInSantasBag))
            {
                //TODO: We have a bug somewhere in this since we sometimes run out of presents / no match 
                yield return CreateDistribution(child, toyDistributionProblem.Toys.Items.Except(distributedToys).First(toy => child.Wishes.Toys.Items.Any(toyWish => toyWish.Name == toy.Name)));
            }
        }
    }
}