using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace VerticaDevXmas2019.Domain
{
    [XmlRoot(nameof(ToyDistributionProblem))]
    public class ToyDistributionProblem
    {
        [XmlElement("Toys")] 
        public ToysList Toys { get; set; }

        [XmlElement(nameof(Children))]
        public ChildList Children { get; set; }

        public IEnumerable<ToyDistribution> DistributePresentsToChildren()
        {
            var distributedToys = new List<Toy>();

            var childrenWithOneMatchInSantasBag = Children.Items.Where(child => Toys.Items.Count(toyInSantasBag => child.Wishes.Toys.Items.Select(toyWish => toyWish.Name).Contains(toyInSantasBag.Name)) == 1).ToArray();

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
                yield return CreateDistribution(child, Toys.Items.Except(distributedToys).Single(toy => child.Wishes.Toys.Items.Any(toyWish => toyWish.Name == toy.Name)));
            }

            foreach (var child in Children.Items.Except(childrenWithOneMatchInSantasBag))
            {
                //TODO: We have a bug somewhere in this since we sometimes run out of presents / no match 
                yield return CreateDistribution(child, Toys.Items.Except(distributedToys).First(toy => child.Wishes.Toys.Items.Any(toyWish => toyWish.Name == toy.Name)));
            }
        }
    }
}