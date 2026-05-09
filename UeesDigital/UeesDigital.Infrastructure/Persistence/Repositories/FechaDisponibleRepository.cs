using Microsoft.EntityFrameworkCore;
using UeesDigital.Domain.Entities;
using UeesDigital.Domain.Interfaces;
using UeesDigital.Infrastructure.Persistence;

namespace UeesDigital.Infrastructure.Persistence.Repositories
{
    public class FechaDisponibleRepository : IFechaDisponibleRepository
    {
        private readonly ApplicationDbContext _context;
        public FechaDisponibleRepository(ApplicationDbContext context) => _context = context;

        public async Task<IEnumerable<FechaDisponible>> GetActivasAsync() =>
            await _context.FechasDisponibles
                .Include(f => f.Horarios)
                .Where(f => f.Activo)
                .ToListAsync();
    }
}