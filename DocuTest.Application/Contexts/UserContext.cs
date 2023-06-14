using DocuTest.Shared.Enums;
using DocuTest.Shared.Interfaces;

namespace DocuTest.Application.Contexts
{
    public class UserContext : IUserContext
    {
        public UserContext(Guid id, string name, string email, params Role[] roles)
        {
            this.Id = id;
            this.Name = name;
            this.Email = email;
            this.Roles = roles;
        }

        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public string Email { get; private set; }

        public IEnumerable<Role> Roles { get; private set; }

        public bool InRole(params Role[] roles) => this.Roles.Any(role => roles.Contains(role));

        public bool IsInAllRoles(params Role[] roles) => roles.All(role => this.Roles.Contains(role));
    }
}
