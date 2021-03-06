﻿using Microsoft.Azure.Documents.Spatial;
using Newtonsoft.Json;

namespace VerticaDevXmas2019.Domain
{
    public class SantaZone: ValueObject
    {
        [JsonProperty("reindeer")]
        public string Reindeer { get; internal set; }

        [JsonProperty("countryCode")]
        public string CountryCode { get; internal set; }

        [JsonProperty("cityName")]
        public string CityName { get; internal set; }

        [JsonProperty("center")]
        public CanePosition Center { get; internal set; }

        public Microsoft.Azure.Documents.Spatial.Point GetCenter() => new Microsoft.Azure.Documents.Spatial.Point(Center.Longitude, Center.Latitude);

        [JsonProperty("radius")]
        public SantaMovementRadius Radius { get; internal set; }
    }
}