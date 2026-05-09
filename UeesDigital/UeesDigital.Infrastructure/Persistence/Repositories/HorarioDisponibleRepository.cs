using Microsoft.EntityFrameworkCore;
using UeesDigital.Domain.Entities;
using UeesDigital.Domain.Interfaces;
using UeesDigital.Infrastructure.Persistence;

namespace UeesDigital.Infrastructure.Persistence.Repositories
{
    public class HorarioDisponibleRepository : IHorarioDisponibleRepository
    {
        private readonly ApplicationDbContext _context;
        public HorarioDisponibleRepository(ApplicationDbContext context) => _context = context;

        public async Task<IEnumerable<HorarioDisponible>> GetDisponiblesByFechaAsync(int idFecha) =>
            await _context.HorariosDisponibles
                .Where(h => h.IdFechaDisponible == idFecha && h.Activo && h.CuposOcupados < h.CuposMaximos)
                .ToListAsync();

        public async Task<HorarioDisponible?> GetByIdAsync(int id) =>
            await _context.HorariosDisponibles.FindAsync(id);
    }
}