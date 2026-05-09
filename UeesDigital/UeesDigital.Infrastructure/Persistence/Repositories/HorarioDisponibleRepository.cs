using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using UeesDigital.Domain.Entities;
using UeesDigital.Domain.Interfaces;
using UeesDigital.Infrastructure.Persistence;

namespace UeesDigital.Infrastructure.Persistence.Repositories
{
    public class HorarioDisponibleRepository : IHorarioDisponibleRepository
    {
        private readonly ApplicationDbContext _context;
        public HorarioDisponibleRepository(ApplicationDbContext context) => _context = context;

        public async Task<HorarioDisponible> AddAsync(HorarioDisponible entity)
        {
            _context.HorariosDisponibles.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<HorarioDisponible> Update(HorarioDisponible entity)
        {
            _context.HorariosDisponibles.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> Delete(int id)
        {
            var horario = await _context.HorariosDisponibles.FindAsync(id);
            if (horario == null) return false;
            horario.IsDelete = true;
            return await _context.SaveChangesAsync() >= 1;
        }

        public async Task<HorarioDisponible?> FindFirstOrDefaultAsync(Expression<Func<HorarioDisponible, bool>> predicate, params Expression<Func<HorarioDisponible, object>>[] includes)
        {
            var query = _context.HorariosDisponibles.AsQueryable();
            foreach (var include in includes)
                query = query.Include(include);
            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<HorarioDisponible>> GetAll(Expression<Func<HorarioDisponible, bool>> predicate, int take, int page, string search)
        {
            return await _context.HorariosDisponibles
                .Include(h => h.FechaDisponible)
                .Where(predicate)
                .Skip((page - 1) * take)
                .Take(take)
                .ToListAsync();
        }

        public async Task<IEnumerable<HorarioDisponible>> GetDisponiblesByFechaAsync(int idFecha) =>
            await _context.HorariosDisponibles
                .Where(h => h.IdFechaDisponible == idFecha && h.Activo && h.CuposOcupados < h.CuposMaximos)
                .ToListAsync();

        public async Task<HorarioDisponible?> GetByIdAsync(int id) =>
            await _context.HorariosDisponibles.FindAsync(id);
    }
}