using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace VerticaDevXmas2019.Domain
{
    [XmlRoot(nameof(ToyDistributionProblem))]
    public class ToyDistributionProblem
    {
        [XmlElement(nameof(Toy))]
        public Toy[] Toys { get; set; }

        [XmlElement(nameof(Child))]
        public Child[] Children { get; set; }
    }
}