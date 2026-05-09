using System;
using System.Collections.Generic;
using System.Text;
using UeesDigital.Domain.Entities;

namespace UeesDigital.Domain.Interfaces
{
    public interface IFechaDisponibleRepository : IBaseRepository<FechaDisponible>
    {
        Task<IEnumerable<FechaDisponible>> GetActivasAsync();
    }
}
