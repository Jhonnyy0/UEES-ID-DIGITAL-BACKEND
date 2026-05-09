using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace UeesDigital.Application.Services
{
    public class TramiteService
    {
        private readonly ITramiteRepository _tramiteRepository;

        public TramiteService(ITramiteRepository tramiteRepository)
        {
            _tramiteRepository = tramiteRepository;
        }

        public async Task<Tramite> Add(Tramite tramite)
        {
            return await _tramiteRepository.AddAsync(tramite);
        }

        public async Task<bool> Delete(int tramiteId)
        {
            return await _tramiteRepository.Delete(tramiteId);
        }

        public async Task<Tramite> Update(Tramite tramite)
        {
            return await _tramiteRepository.Update(tramite);
        }

        public async Task<Tramite?> FindByIdAsync(int id)
        {
            return await _tramiteRepository.FindFirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<Tramite>> GetAll(int take, int page, string search)
        {
            var result = await _tramiteRepository.GetAll(c => !c.IsDelete, take, page, search);

            return result.ToList();
        }
    }
}
