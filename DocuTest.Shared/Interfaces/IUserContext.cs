using DocuTest.Shared.Enums;

namespace DocuTest.Shared.Interfaces
{
    public interface IUserContext
    {
        Guid Id { get; }

        string Name { get; }

        string Email { get; }

        IEnumerable<Role> Roles { get; }

        bool InRole(params Role[] roles);

        bool IsInAllRoles(params Role[] roles);
    }
}
