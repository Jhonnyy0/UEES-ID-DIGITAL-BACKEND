using System;
using System.Collections.Generic;
using System.Text;
using UeesDigital.Domain.Entities;
using UeesDigital.Domain.Interfaces;

namespace UeesDigital.Application.Services
{
    public class AuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;

        public AuthService(IUserRepository userRepository, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        public async Task<Estudiante> RegisterUser(Estudiante estudiante)
        {
            await _userRepository.CreateUser(estudiante);
            return estudiante;
        }

        public async Task<String> Login(string email, string password, bool remember)
        {
            var usuario = await _userRepository.GetUserByEmail(email);

            if (usuario == null)
            {
                return "Usuario no encontrado";
            }

            var creedencialesValidas = await _userRepository.CheckPasswordAsync(usuario.Id.ToString(), password);

            if (!creedencialesValidas)
            {
                return "Creedenciales invalidas";
            }

            var roles = await _userRepository.GetUserRoles(email);

            return _jwtService.GenerateToken(usuario.Id.ToString(), usuario.Email, roles);
        }
    }
}