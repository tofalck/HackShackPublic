using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace VerticaDevXmas2019.Domain
{
    /// <summary>
    /// Elastic Search mapped project/document
    /// </summary>
    public class ChristmasProject: EntityObject
    {
        public Dictionary<string, double> CanePosition { get; set; } //From ES - didn't bother to map is using ES lingo...
        public CanePosition InitialCanePosition => new CanePosition() //Shorthand to use . 
        {
            Latitude = CanePosition["lat"],
            Longitude = CanePosition["lon"],
        };
        public List<SantaMovement> SantaMovements { get; set; }
    }
}