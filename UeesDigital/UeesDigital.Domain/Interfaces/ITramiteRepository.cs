using System;
using System.Collections.Generic;
using System.Text;
using UeesDigital.Domain.Entities;

namespace UeesDigital.Domain.Interfaces
{
    public interface ITramiteRepository
    {
        Task<IEnumerable<Tramite>> GetAllAsync();
        
        Task<Tramite?> GetByIdAsync(int id);
        

        Task<Tramite?> UpdateAsync(Tramite tramite);

        Task<Tramite> CreateAsync(Tramite tramite);

        Task<bool> DeleteAsync(int id);
    }
}
