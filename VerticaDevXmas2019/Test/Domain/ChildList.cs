using System.Xml.Serialization;

namespace VerticaDevXmas2019.Domain
{
    public class ChildList
    {
        [XmlElement(nameof(Child))]
        public Child[] Items { get; set; }
    }
}