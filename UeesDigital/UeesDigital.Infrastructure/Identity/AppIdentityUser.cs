using Microsoft.AspNetCore.Identity;

namespace UeesDigital.Infrastructure.Identity
{
    public class AppIdentityUser : IdentityUser
    {
        public string NombreCompleto { get; set; }
    }
}