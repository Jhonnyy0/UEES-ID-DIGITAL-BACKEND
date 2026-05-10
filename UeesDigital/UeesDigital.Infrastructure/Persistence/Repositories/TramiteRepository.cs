using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
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
                .Include(t => t.Estudiante).ThenInclude(e => e.Carrera).ThenInclude(c => c.Facultad)
                .Include(t => t.Horario).ThenInclude(h => h.FechaDisponible)
                .ToListAsync();

        public async Task<Tramite?> GetByIdAsync(Guid id) =>
            await _context.Tramites
                .Include(t => t.Estudiante)
                .Include(t => t.Horario).ThenInclude(h => h.FechaDisponible)
                .FirstOrDefaultAsync(t => t.IdTramite == id);

        public async Task<Tramite> AddAsync(Tramite tramite)
        {
            tramite.FechaRegistro      = DateTime.Now;
            tramite.CodigoConfirmacion = Guid.NewGuid().ToString("N")[..10].ToUpper();
            _context.Tramites.Add(tramite);
            await _context.SaveChangesAsync();

            // Recargar con includes para devolver datos completos
            return await GetByIdAsync(tramite.IdTramite) ?? tramite;
        }

        public async Task<Tramite> Update(Tramite tramite)
        {
            var existing = await _context.Tramites.FindAsync(tramite.IdTramite);
            if (existing == null) return tramite;
            existing.Estado     = tramite.Estado;
            existing.TipoTramite = tramite.TipoTramite;
            existing.IdHorario  = tramite.IdHorario;
            await _context.SaveChangesAsync();

            // Recargar con includes
            return await GetByIdAsync(existing.IdTramite) ?? existing;
        }

        public async Task<bool> Delete(int id) => false;

        public async Task<Tramite?> FindFirstOrDefaultAsync(Expression<Func<Tramite, bool>> predicate, params Expression<Func<Tramite, object>>[] includes)
        {
            var query = _context.Tramites
                .Include(t => t.Estudiante)
                .Include(t => t.Horario).ThenInclude(h => h.FechaDisponible)
                .AsQueryable();
            foreach (var include in includes)
                query = query.Include(include);
            return await query.FirstOrDefaultAsync(predicate);
        }

        // ← GetAll ahora incluye ThenInclude(FechaDisponible)
        public async Task<IEnumerable<Tramite>> GetAll(Expression<Func<Tramite, bool>> predicate, int take, int page, string search)
        {
            return await _context.Tramites
                .Include(t => t.Estudiante)
                .Include(t => t.Horario).ThenInclude(h => h.FechaDisponible)
                .Where(predicate)
                .Skip((page - 1) * take)
                .Take(take)
                .ToListAsync();
        }
    }
}
