using System;
using System.Collections.Generic;
using System.Text;

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

        public async Task<Usuario> RegisterUser(Usuario usuario)
        {
            await _userRepository.CreateUser(usuario);

            return usuario;
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

            return _jwtService.GenerateToken(usuario, roles);
        }
    }
}
