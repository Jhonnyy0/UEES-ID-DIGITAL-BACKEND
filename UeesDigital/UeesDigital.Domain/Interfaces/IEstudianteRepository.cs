using System;
using System.Collections.Generic;
using System.Text;
using UeesDigital.Domain.Entities;

namespace UeesDigital.Domain.Interfaces
{
    public interface IEstudianteRepository
    {
        Task<IEnumerable<Estudiante>> GetAllAsync();
        Task<Estudiante?> GetByCIFAsync(string cif);
        Task<Estudiante> CreateAsync(Estudiante estudiante);
    }
}
