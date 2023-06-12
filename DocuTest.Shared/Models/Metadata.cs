namespace DocuTest.Shared.Models
{
    public class Metadata
    {
        public Guid FileId { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public override bool Equals(object? other)
        {
            if (other == null)
                return false;

            if (other is not Metadata)
                return false;

            Metadata metadata = (Metadata)other;

            return 
                this.FileId == metadata.FileId &&
                this.Key == metadata.Key &&
                this.Value == metadata.Value;
        }

        public override int GetHashCode() => new
        {
            this.FileId,
            this.Key,
            this.Value 
        }.GetHashCode();
    }
}
