using Microsoft.EntityFrameworkCore;
using UeesDigital.Domain.Entities;
using UeesDigital.Domain.Interfaces;
using UeesDigital.Infrastructure.Persistence;

namespace UeesDigital.Infrastructure.Persistence.Repositories
{
    public class EstudianteRepository : IEstudianteRepository
    {
        private readonly ApplicationDbContext _context;
        public EstudianteRepository(ApplicationDbContext context) => _context = context;

        public async Task<IEnumerable<Estudiante>> GetAllAsync() =>
            await _context.Estudiantes
                .Include(e => e.Carrera).ThenInclude(c => c.Facultad)
                .ToListAsync();

        public async Task<Estudiante?> GetByCIFAsync(string cif) =>
            await _context.Estudiantes
                .Include(e => e.Carrera).ThenInclude(c => c.Facultad)  
                .FirstOrDefaultAsync(e => e.Carnet.ToString() == cif);

        public async Task<Estudiante> CreateAsync(Estudiante estudiante)
        {
            _context.Estudiantes.Add(estudiante);
            await _context.SaveChangesAsync();
            return estudiante;
        }
    }
}