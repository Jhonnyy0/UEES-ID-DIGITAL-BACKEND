using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UeesDigital.Domain.Entities;
using UeesDigital.Domain.Interfaces;

namespace UeesDigital.Application.Services
{
    public class HorarioDisponibleService
    {
        private readonly IHorarioDisponibleRepository _horarioDisponibleRepository;

        public HorarioDisponibleService(IHorarioDisponibleRepository horarioDisponibleRepository)
        {
            _horarioDisponibleRepository = horarioDisponibleRepository;
        }

        public async Task<HorarioDisponible> Add(HorarioDisponible horarioDisponible)
        {
            return await _horarioDisponibleRepository.AddAsync(horarioDisponible);
        }

        public async Task<bool> Delete(int horarioDisponibleId)
        {
            return await _horarioDisponibleRepository.Delete(horarioDisponibleId);
        }

        public async Task<HorarioDisponible> Update(HorarioDisponible horarioDisponible)
        {
            return await _horarioDisponibleRepository.Update(horarioDisponible);
        }

        public async Task<HorarioDisponible?> FindByIdAsync(int id)
        {
            return await _horarioDisponibleRepository.FindFirstOrDefaultAsync(h => h.IdHorario == id);
        }

        public async Task<List<HorarioDisponible>> GetAll(int take, int page, string search)
        {
            var result = await _horarioDisponibleRepository.GetAll(h => !h.IsDelete, take, page, search);

            return result.ToList();
        }

        public async Task<List<HorarioDisponible>> GetDisponiblesByFechaAsync(int idFecha)
        {
            var result = await _horarioDisponibleRepository.GetDisponiblesByFechaAsync(idFecha);
            return result.Where(h => !h.IsDelete).ToList();
        }
    }
}
