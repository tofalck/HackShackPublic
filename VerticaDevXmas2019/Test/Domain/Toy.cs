using System.Xml.Serialization;

namespace VerticaDevXmas2019.Domain
{
    public class Toy
    {
        [XmlAttribute(nameof(Name))]
        public string Name { get; set; }
    }
}