using System.Runtime.Serialization;

namespace VerticaDevXmas2019.Domain
{
    public enum SantaMovementUnit
    {
        [EnumMember(Value = "foot")]
        Foot,
        [EnumMember(Value = "meter")]
        Meter,
        [EnumMember(Value = "kilometer")]
        Kilometer,
    }
}