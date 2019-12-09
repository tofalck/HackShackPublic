namespace VerticaDevXmas2019.Domain
{
    public class SantaMovementRadius: ValueObject
    {
        public double Value { get; set; }
        public double ValueInMeters => (Unit == SantaMovementUnit.Foot ? Value * 0.304800610d : Unit == SantaMovementUnit.Kilometer ? Value * 1000 : Value);
        public SantaMovementUnit Unit { get; set; }
    }
}