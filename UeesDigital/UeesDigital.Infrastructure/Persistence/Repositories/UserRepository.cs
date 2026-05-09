using Microsoft.AspNetCore.Identity;
using UeesDigital.Domain.Entities;
using UeesDigital.Domain.Interfaces;
using UeesDigital.Infrastructure.Identity;
using UeesDigital.Infrastructure.Mapping;

namespace UeesDigital.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<AppIdentityUser> _userManager;

        public UserRepository(UserManager<AppIdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Estudiante> CreateUser(Estudiante estudiante)
        {
            var identityUser = estudiante.ToIdentityUser();
            await _userManager.CreateAsync(identityUser, estudiante.Password);
            return estudiante;
        }

        public async Task<Estudiante?> GetUserByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return null;
            return new Estudiante
            {
                Id = Guid.Parse(user.Id),
                Email = user.Email!,
                FirstName = user.NombreCompleto?.Split(' ')[0] ?? string.Empty,
                LastName = user.NombreCompleto?.Split(' ')[1] ?? string.Empty
            };
        }

        public async Task<bool> CheckPasswordAsync(string userId, string password)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;
            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<IList<string>> GetUserRoles(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return new List<string>();
            return await _userManager.GetRolesAsync(user);
        }
    }
}