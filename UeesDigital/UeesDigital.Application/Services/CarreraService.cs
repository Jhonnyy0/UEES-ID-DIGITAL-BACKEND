using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UeesDigital.Domain.Entities;
using UeesDigital.Domain.Interfaces;

namespace UeesDigital.Application.Services
{
    public class CarreraService
    {
        private readonly ICarreraRepository _carreraRepository;

        public CarreraService(ICarreraRepository carreraRepository)
        {
            _carreraRepository = carreraRepository;
        }

        public async Task<Carrera> Add(Carrera carrera)
        {
            return await _carreraRepository.AddAsync(carrera);
        }

        public async Task<Carrera> Update(Carrera carrera)
        {
            return await _carreraRepository.Update(carrera);
        }

        public async Task<bool> Delete(int carreraId)
        {
            return await _carreraRepository.Delete(carreraId);
        }

        public async Task<Carrera?> FindByIdAsync(int id)
        {
            return await _carreraRepository.FindFirstOrDefaultAsync(c => c.IdCarrera == id, c => c.Estudiantes);
        }

        public async Task<List<Carrera>> GetAll(int take, int page, string search)
        {
            var result = await _carreraRepository.GetAll(c => !c.IsDelete, take, page, search);

            return result.ToList();
        }
    }
}
