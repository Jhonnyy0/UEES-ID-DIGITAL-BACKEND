using System;
using System.Collections.Generic;
using System.Text;
using UeesDigital.Domain.Entities;

namespace UeesDigital.Domain.Interfaces
{
    public interface IHorarioDisponibleRepository : IBaseRepository<HorarioDisponible> 
    {

        Task<IEnumerable<HorarioDisponible>> GetDisponiblesByFechaAsync(int idFecha);
        
        Task<HorarioDisponible?> GetByIdAsync(int id);
    }
}
