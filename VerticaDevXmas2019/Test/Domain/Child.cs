using System.Xml.Serialization;

namespace VerticaDevXmas2019.Domain
{
    public class Child
    {

        [XmlAttribute(nameof(Name))]
        public string Name { get; set; }

        [XmlElement(nameof(WishList))]
        public WishList Wishes { get; set; }
    }
}