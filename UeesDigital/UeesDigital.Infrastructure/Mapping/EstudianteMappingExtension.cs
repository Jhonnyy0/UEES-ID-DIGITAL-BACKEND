using UeesDigital.Domain.Entities;
using UeesDigital.Infrastructure.Identity;

namespace UeesDigital.Infrastructure.Mapping
{
    public static class EstudianteMappingExtension
    {
        public static AppIdentityUser ToIdentityUser(this Estudiante estudiante)
        {
            return new AppIdentityUser
            {
                Id = estudiante.Id.ToString(),
                UserName = estudiante.Email,
                Email = estudiante.Email,
                NombreCompleto = estudiante.FullName
            };
        }
    }
}
