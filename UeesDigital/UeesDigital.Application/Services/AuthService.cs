using UeesDigital.Domain.Interfaces;

namespace UeesDigital.Application.Services
{
    public record LoginResult(bool Success, string? Token = null, string? Error = null);

    public class AuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService     _jwtService;

        public AuthService(IUserRepository userRepository, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService     = jwtService;
        }

        public async Task<LoginResult> Login(string email, string password, bool remember)
        {
            var usuario = await _userRepository.GetUserByEmail(email);
            if (usuario == null)
                return new LoginResult(false, Error: "Usuario no encontrado");

            var credencialesValidas = await _userRepository.CheckPasswordAsync(usuario.Id.ToString(), password);
            if (!credencialesValidas)
                return new LoginResult(false, Error: "Credenciales inválidas");

            var roles = await _userRepository.GetUserRoles(email);
            var token = _jwtService.GenerateToken(usuario.Id.ToString(), usuario.Email, roles);

            return new LoginResult(true, Token: token);
        }
    }
}
