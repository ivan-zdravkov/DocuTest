namespace DocuTest.Shared.Models
{
    public class Document
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid DocumentTypeId { get; set; }

        public Guid UserId { get; set; }

        public IEnumerable<File> Files { get; set; }
    }
}
