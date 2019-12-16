using System.Xml.Serialization;

namespace VerticaDevXmas2019.Domain
{
    public class ToysList
    {
        [XmlElement(nameof(Toy))]
        public Toy[] Items { get; set; }
    }
}