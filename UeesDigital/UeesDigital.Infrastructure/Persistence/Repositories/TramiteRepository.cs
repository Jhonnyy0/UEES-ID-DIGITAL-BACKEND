using Microsoft.EntityFrameworkCore;
using UeesDigital.Domain.Entities;
using UeesDigital.Domain.Interfaces;
using UeesDigital.Infrastructure.Persistence;

namespace UeesDigital.Infrastructure.Persistence.Repositories
{
    public class TramiteRepository : ITramiteRepository
    {
        private readonly ApplicationDbContext _context;
        public TramiteRepository(ApplicationDbContext context) => _context = context;

        public async Task<IEnumerable<Tramite>> GetAllAsync() =>
            await _context.Tramites
                .Include(t => t.TipoDeTramite)
                .Include(t => t.Estudiante).ThenInclude(e => e.Carrera).ThenInclude(c => c.Facultad)
                .Include(t => t.Horario).ThenInclude(h => h.FechaDisponible)
                .ToListAsync();

        public async Task<Tramite?> GetByIdAsync(int id) =>
            await _context.Tramites
                .Include(t => t.TipoDeTramite)
                .Include(t => t.Estudiante)
                .Include(t => t.Horario).ThenInclude(h => h.FechaDisponible)
                .FirstOrDefaultAsync(t => t.IdTramite == id);

        public async Task<Tramite> CreateAsync(Tramite tramite)
        {
            tramite.FechaRegistro = DateTime.Now;
            tramite.CodigoConfirmacion = Guid.NewGuid().ToString("N")[..10].ToUpper();
            _context.Tramites.Add(tramite);
            await _context.SaveChangesAsync();
            return tramite;
        }

        public async Task<Tramite?> UpdateAsync(Tramite tramite)
        {
            var existing = await _context.Tramites.FindAsync(tramite.IdTramite);
            if (existing == null) return null;
            existing.Estado = tramite.Estado;
            existing.IdTipoDeTramite = tramite.IdTipoDeTramite;
            existing.IdHorario = tramite.IdHorario;
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var tramite = await _context.Tramites.FindAsync(id);
            if (tramite == null) return false;
            tramite.Estado = "Cancelado";
            return await _context.SaveChangesAsync() >= 1;
        }
    }
}