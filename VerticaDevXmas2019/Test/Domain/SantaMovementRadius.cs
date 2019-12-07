namespace VerticaDevXmas2019.Domain
{
    public class SantaMovementRadius: ValueObject
    {
        public double Value { get; set; }
        public SantaMovementUnit Unit { get; set; }
    }
}