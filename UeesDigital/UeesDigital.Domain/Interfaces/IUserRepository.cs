using UeesDigital.Domain.Entities;

namespace UeesDigital.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<Estudiante> CreateUser(Estudiante usuario);
        Task<Estudiante?> GetUserByEmail(string email);
        Task<bool> CheckPasswordAsync(string userId, string password);
        Task<IList<string>> GetUserRoles(string email);
    }
}