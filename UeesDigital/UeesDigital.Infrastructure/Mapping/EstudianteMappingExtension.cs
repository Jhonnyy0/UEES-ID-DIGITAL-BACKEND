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
                UserName = estudiante.Correo,
                Email = estudiante.Correo,
                NombreCompleto = $"{estudiante.Nombre} {estudiante.Apellido}"
            };
        }

        public static Estudiante ToDomainEstudiante(this AppIdentityUser user)
        {
            return new Estudiante
            {
                Correo = user.Email,
                Nombre = user.NombreCompleto?.Split(' ')[0] ?? string.Empty,
                Apellido = user.NombreCompleto?.Split(' ')[1] ?? string.Empty
            };
        }
    }
}