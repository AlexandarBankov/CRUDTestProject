using Microsoft.AspNetCore.Identity;

namespace Management.Data.Entities
{
    public class User : IdentityUser
    {
        public required string TenantId { get; set; }

    }
}
