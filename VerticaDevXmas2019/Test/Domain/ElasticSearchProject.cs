using System.Collections.Generic;

namespace VerticaDevXmas2019.Domain
{
    public class ElasticSearchProject: EntityObject
    {
        public Dictionary<string, double> CanePosition { get; set; } //From ES - didn't bother to map is using ES lingo...
        public CanePosition SantasCanePosition => new CanePosition() //Shorthand to use . 
        {
            Latitude = CanePosition["lat"],
            Longitude = CanePosition["lon"],
        };
        public List<SantaMovement> SantaMovements { get; set; }
    }
}