namespace DocuTest.Shared.Models
{
    public class File
    {
        public Guid Id { get; set; }

        public Guid DocumentId { get; set; }

        public string Name { get; set; }

        public string Extension { get; set; }

        public byte[] Content { get; set; }

        public IEnumerable<Metadata> Metadata { get; set; }
    }
}
