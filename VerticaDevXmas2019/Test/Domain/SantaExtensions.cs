using System;
using System.Globalization;
using Newtonsoft.Json;

namespace VerticaDevXmas2019.Domain
{
    public static class SantaExtensions
    {
        public static double InMeters(this double value, SantaMovementUnit unit) => (unit == SantaMovementUnit.Foot ? value * 0.304800610d : unit == SantaMovementUnit.Kilometer ? value * 1000 : value);

        public static string ToJson(this object obj, JsonSerializerSettings serializerSettings = null) =>
            serializerSettings == null ? JsonConvert.SerializeObject(obj) : JsonConvert.SerializeObject(obj, serializerSettings);

        public static T FromJson<T>(this string json) => JsonConvert.DeserializeObject<T>(json);

        public static CanePosition Dump(this CanePosition obj, string message = null)
        {
            string Format(double value)
            {
                return value.ToString("N8", new CultureInfo("en-US"));
            }
            if (string.IsNullOrEmpty(message) == false)
            {
                Console.WriteLine($"{message}");
            }
            Console.WriteLine($"{Format(obj.Latitude)},{Format(obj.Longitude)}{Environment.NewLine}");
            return obj;
        }
    }
}