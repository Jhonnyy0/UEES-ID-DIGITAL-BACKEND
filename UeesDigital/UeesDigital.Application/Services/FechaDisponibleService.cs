using System;
using System.Collections.Generic;
using System.Text;
using UeesDigital.Domain.Entities;
using UeesDigital.Domain.Interfaces;

namespace UeesDigital.Application.Services
{
    public class FechaDisponibleService
    {
        private readonly IFechaDisponibleRepository _fechaDisponibleRepository;

        public FechaDisponibleService(IFechaDisponibleRepository fechaDisponibleRepository)
        {
            _fechaDisponibleRepository = fechaDisponibleRepository;
        }

        public async Task<FechaDisponible> Add(FechaDisponible fechaDisponible)
        {
            return await _fechaDisponibleRepository.AddAsync(fechaDisponible);
        }

        public async Task<bool> Delete(int fechaDisponibleId)
        {
            return await _fechaDisponibleRepository.Delete(fechaDisponibleId);
        }

        public async Task<FechaDisponible> Update(FechaDisponible fechaDisponible)
        {
            return await _fechaDisponibleRepository.Update(fechaDisponible);
        }

        public async Task<FechaDisponible?> FindByIdAsync(int id)
        {
            return await _fechaDisponibleRepository.FindFirstOrDefaultAsync(f => f.IdFechaDisponible == id);

        }

        public async Task<List<FechaDisponible>> GetAll(int take, int page, string search)
        {
            var result = await _fechaDisponibleRepository.GetAll(f => !f.IsDelete, take, page, search);

            return result.ToList();
        }
    }
}
