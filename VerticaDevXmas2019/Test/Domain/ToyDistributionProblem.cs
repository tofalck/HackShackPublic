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
    }
}