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
                .Include(t => t.Estudiante).ThenInclude(e => e.Carrera).ThenInclude(c => c.Facultad)
                .Include(t => t.Horario).ThenInclude(h => h.FechaDisponible)
                .FirstOrDefaultAsync(t => t.IdTramite == id);

        // ── CREAR TRÁMITE — descuenta cupo ───────────────────────────────────
        public async Task<Tramite> AddAsync(Tramite tramite)
        {
            var horario = await _context.HorariosDisponibles
                .FirstOrDefaultAsync(h => h.IdHorario == tramite.IdHorario);

            if (horario == null)
                throw new InvalidOperationException("El horario seleccionado no existe.");

            if (horario.CuposOcupados >= horario.CuposMaximos)
                throw new InvalidOperationException("No hay cupos disponibles para el horario seleccionado.");

            tramite.FechaRegistro = DateTime.Now;
            tramite.CodigoConfirmacion = Guid.NewGuid().ToString("N")[..10].ToUpper();

            horario.CuposOcupados += 1; // Descuenta cupo (reemplaza el trigger)

            _context.Tramites.Add(tramite);
            await _context.SaveChangesAsync();

            return await _context.Tramites
                .Include(t => t.Estudiante).ThenInclude(e => e.Carrera).ThenInclude(c => c.Facultad)
                .Include(t => t.Horario).ThenInclude(h => h.FechaDisponible)
                .FirstAsync(t => t.IdTramite == tramite.IdTramite);
        }

        // ── ACTUALIZAR TRÁMITE — maneja cupos según cambios de estado/horario ─
        public async Task<Tramite> Update(Tramite tramite)
        {
            // Cargamos el estado ACTUAL desde la BD (antes del cambio)
            var existing = await _context.Tramites
                .FirstOrDefaultAsync(t => t.IdTramite == tramite.IdTramite);

            if (existing == null) return tramite;

            var estadoAnterior = existing.Estado;
            var estadoNuevo = tramite.Estado;
            var horarioAnteriorId = existing.IdHorario;
            var horarioNuevoId = tramite.IdHorario;

            // ── Caso 1: Cambia de horario (y el trámite no estaba cancelado) ──
            if (horarioAnteriorId != horarioNuevoId && estadoAnterior != EstadoTramite.Cancelado)
            {
                // Devolver cupo al horario anterior
                var horarioAnterior = await _context.HorariosDisponibles
                    .FirstOrDefaultAsync(h => h.IdHorario == horarioAnteriorId);
                if (horarioAnterior != null && horarioAnterior.CuposOcupados > 0)
                    horarioAnterior.CuposOcupados -= 1;

                // Descontar cupo del nuevo horario
                var horarioNuevo = await _context.HorariosDisponibles
                    .FirstOrDefaultAsync(h => h.IdHorario == horarioNuevoId);
                if (horarioNuevo != null)
                {
                    if (horarioNuevo.CuposOcupados >= horarioNuevo.CuposMaximos)
                        throw new InvalidOperationException("No hay cupos disponibles en el nuevo horario.");
                    horarioNuevo.CuposOcupados += 1;
                }

                existing.IdHorario = horarioNuevoId;
            }

            // ── Caso 2: Se cancela un trámite activo → devolver cupo ──────────
            if (estadoAnterior != EstadoTramite.Cancelado && estadoNuevo == EstadoTramite.Cancelado)
            {
                var horario = await _context.HorariosDisponibles
                    .FirstOrDefaultAsync(h => h.IdHorario == existing.IdHorario);
                if (horario != null && horario.CuposOcupados > 0)
                    horario.CuposOcupados -= 1;
            }

            // ── Caso 3: Se reactiva un trámite cancelado → descontar cupo ─────
            if (estadoAnterior == EstadoTramite.Cancelado && estadoNuevo != EstadoTramite.Cancelado)
            {
                var horario = await _context.HorariosDisponibles
                    .FirstOrDefaultAsync(h => h.IdHorario == existing.IdHorario);
                if (horario != null)
                {
                    if (horario.CuposOcupados >= horario.CuposMaximos)
                        throw new InvalidOperationException("No hay cupos disponibles para reactivar el trámite.");
                    horario.CuposOcupados += 1;
                }
            }

            // Aplicar cambios al trámite
            existing.Estado = estadoNuevo;
            existing.TipoTramite = tramite.TipoTramite;

            await _context.SaveChangesAsync();

            return await _context.Tramites
                .Include(t => t.Estudiante).ThenInclude(e => e.Carrera).ThenInclude(c => c.Facultad)
                .Include(t => t.Horario).ThenInclude(h => h.FechaDisponible)
                .FirstAsync(t => t.IdTramite == tramite.IdTramite);
        }

        public async Task<bool> Delete(int id) => false;

        public async Task<Tramite?> FindFirstOrDefaultAsync(
            Expression<Func<Tramite, bool>> predicate,
            params Expression<Func<Tramite, object>>[] includes)
        {
            var query = _context.Tramites.AsQueryable();
            foreach (var include in includes)
                query = query.Include(include);
            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<Tramite>> GetAll(
            Expression<Func<Tramite, bool>> predicate,
            int take,
            int page,
            string search)
        {
            return await _context.Tramites
                .Include(t => t.Estudiante).ThenInclude(e => e.Carrera).ThenInclude(c => c.Facultad)
                .Include(t => t.Horario).ThenInclude(h => h.FechaDisponible)
                .Where(predicate)
                .Skip((page - 1) * take)
                .Take(take)
                .ToListAsync();
        }
    }
}