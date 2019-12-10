namespace VerticaDevXmas2019.Domain
{
    public class SantaMovementRadius: ValueObject
    {
        public double Value { get; set; }
        public double ValueInMeters => Value.InMeters(Unit);
        public SantaMovementUnit Unit { get; set; }
    }
}