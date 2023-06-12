namespace DocuTest.Shared.Models
{
    public class Document
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid DocumentTypeId { get; set; }

        public Guid UserId { get; set; }

        public IEnumerable<File> Files { get; set; }

        public override bool Equals(object? other)
        {
            if (other == null)
                return false;

            if (other is not Document)
                return false;

            Document document = (Document)other;

            return
                this.Id == document.Id &&
                this.Name == document.Name &&
                this.DocumentTypeId == document.DocumentTypeId &&
                this.UserId == document.UserId &&
                this.Files.SequenceEqual(document.Files);
        }

        public override int GetHashCode() => new
        {
            this.Id,
            this.Name,
            this.DocumentTypeId,
            this.UserId,
            this.Files
        }.GetHashCode();
    }
}
