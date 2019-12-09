using Newtonsoft.Json;

namespace VerticaDevXmas2019.Domain
{
    public class EntityObject
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        protected bool Equals(EntityObject other)
        {
            return string.Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EntityObject) obj);
        }

        public override int GetHashCode()
        {
            return (Id != null ? Id.GetHashCode() : 0);
        }
    }
}