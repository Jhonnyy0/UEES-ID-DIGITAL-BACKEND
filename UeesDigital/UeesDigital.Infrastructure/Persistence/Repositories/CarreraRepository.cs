using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using UeesDigital.Domain.Entities;
using UeesDigital.Domain.Interfaces;
using UeesDigital.Infrastructure.Persistence;

namespace UeesDigital.Infrastructure.Persistence.Repositories
{
    public class CarreraRepository : ICarreraRepository
    {
        private readonly ApplicationDbContext _context;
        public CarreraRepository(ApplicationDbContext context) => _context = context;

        public async Task<Carrera> AddAsync(Carrera entity)
        {
            _context.Carreras.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Carrera> Update(Carrera entity)
        {
            _context.Carreras.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> Delete(int id)
        {
            var carrera = await _context.Carreras.FindAsync(id);
            if (carrera == null) return false;
            carrera.IsDelete = true;
            return await _context.SaveChangesAsync() >= 1;
        }

        public async Task<Carrera?> FindFirstOrDefaultAsync(Expression<Func<Carrera, bool>> predicate, params Expression<Func<Carrera, object>>[] includes)
        {
            var query = _context.Carreras.Include(c => c.Facultad).AsQueryable(); // ← siempre incluye Facultad
            foreach (var include in includes)
                query = query.Include(include);
            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<Carrera>> GetAll(Expression<Func<Carrera, bool>> predicate, int take, int page, string search)
        {
            return await _context.Carreras
                .Include(c => c.Facultad)
                .Include(c => c.Estudiantes)
                .Where(predicate)
                .Where(c => string.IsNullOrEmpty(search) || c.Nombre.Contains(search))
                .Skip((page - 1) * take)
                .Take(take)
                .ToListAsync();
        }
    }
}
