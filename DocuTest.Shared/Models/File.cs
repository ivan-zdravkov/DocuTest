namespace DocuTest.Shared.Models
{
    public class File
    {
        public Guid Id { get; set; }

        public Guid DocumentId { get; set; }

        public string Name { get; set; }

        public string Extension { get; set; }

        public byte[] Content { get; set; }

        public IEnumerable<Metadata> Metadata { get; set; } = Enumerable.Empty<Metadata>();

        public override bool Equals(object? other)
        {
            if (other == null)
                return false;

            if (other is not File)
                return false;

            File file = (File)other;

            return 
                this.Id == file.Id &&
                this.DocumentId == file.DocumentId &&
                this.Name == file.Name &&
                this.Extension == file.Extension &&
                this.Content == file.Content &&
                this.Metadata.SequenceEqual(file.Metadata);
        }

        public override int GetHashCode() => new
        {
            this.Id,
            this.DocumentId,
            this.Name,
            this.Extension,
            this.Content,
            this.Metadata
        }.GetHashCode();
    }
}
