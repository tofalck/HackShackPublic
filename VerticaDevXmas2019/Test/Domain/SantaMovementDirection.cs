using System.Runtime.Serialization;

namespace VerticaDevXmas2019.Domain
{
    public enum SantaMovementDirection
    {
        [EnumMember(Value = "up")]
        Up,
        [EnumMember(Value = "right")]
        Right,
        [EnumMember(Value = "left")]
        Left,
        [EnumMember(Value = "down")]
        Down
    }
}