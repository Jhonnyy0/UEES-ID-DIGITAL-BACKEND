using UeesDigital.Domain.Entities;

namespace UeesDigital.Domain.Interfaces
{
    public interface ITramiteRepository : IBaseRepository<Tramite>
    {
        Task<IEnumerable<Tramite>> GetAllAsync();
        Task<Tramite?> GetByIdAsync(Guid id);
    }
}