using DocuTest.Application.Interfaces;
using DocuTest.Shared.Enums;
using DocuTest.Shared.Interfaces;
using DocuTest.Shared.Models;
using DocuTest.Shared.Strategies;

namespace DocuTest.Application.Strategies
{
    public class DocumentRoleWriteStrategy : SqlStrategy<Document>, IDocumentWriteStrategy
    {
        public DocumentRoleWriteStrategy(IUserContext user) : base(
            column: "DocumentTypeId", 
            (DocumentType.Invoice, user.InRole(Role.Accountant, Role.Admin)),
            (DocumentType.CV, user.InRole(Role.User)),
            (DocumentType.Mail, user.InRole(Role.User, Role.Accountant)),
            (DocumentType.Other, user.InRole(Role.User, Role.Accountant, Role.Admin))
        ) { }

        public override bool Allows(Document value) => base.Records.Any(record => record.Value == value.DocumentTypeId.ToString() && record.Take);
    }
}
