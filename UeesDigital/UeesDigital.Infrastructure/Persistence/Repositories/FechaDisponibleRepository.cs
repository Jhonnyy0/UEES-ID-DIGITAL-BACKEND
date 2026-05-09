using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using UeesDigital.Domain.Entities;
using UeesDigital.Domain.Interfaces;
using UeesDigital.Infrastructure.Persistence;

namespace UeesDigital.Infrastructure.Persistence.Repositories
{
    public class FechaDisponibleRepository : IFechaDisponibleRepository
    {
        private readonly ApplicationDbContext _context;
        public FechaDisponibleRepository(ApplicationDbContext context) => _context = context;

        public async Task<FechaDisponible> AddAsync(FechaDisponible entity)
        {
            _context.FechasDisponibles.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<FechaDisponible> Update(FechaDisponible entity)
        {
            _context.FechasDisponibles.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> Delete(int id)
        {
            var fecha = await _context.FechasDisponibles.FindAsync(id);
            if (fecha == null) return false;
            fecha.IsDelete = true;
            return await _context.SaveChangesAsync() >= 1;
        }

        public async Task<FechaDisponible?> FindFirstOrDefaultAsync(Expression<Func<FechaDisponible, bool>> predicate, params Expression<Func<FechaDisponible, object>>[] includes)
        {
            var query = _context.FechasDisponibles.AsQueryable();
            foreach (var include in includes)
                query = query.Include(include);
            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<FechaDisponible>> GetAll(Expression<Func<FechaDisponible, bool>> predicate, int take, int page, string search)
        {
            return await _context.FechasDisponibles
                .Include(f => f.Horarios)
                .Where(predicate)
                .Skip((page - 1) * take)
                .Take(take)
                .ToListAsync();
        }

        public async Task<IEnumerable<FechaDisponible>> GetActivasAsync() =>
            await _context.FechasDisponibles
                .Include(f => f.Horarios)
                .Where(f => f.Activo)
                .ToListAsync();
    }
}