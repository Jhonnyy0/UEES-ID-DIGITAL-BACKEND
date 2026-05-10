using System;
using UeesDigital.Domain.Entities;
using UeesDigital.Domain.Interfaces;

public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IEstudianteRepository _estudianteRepository;
    private readonly ICarreraRepository _carreraRepository;
    private readonly IJwtService _jwtService;

    public AuthService(
        IUserRepository userRepository,
        IEstudianteRepository estudianteRepository,
        ICarreraRepository carreraRepository,
        IJwtService jwtService)
    {
        _userRepository = userRepository;
        _estudianteRepository = estudianteRepository;
        _carreraRepository = carreraRepository;
        _jwtService = jwtService;
    }

    public async Task<Estudiante> RegisterUser(Estudiante estudiante)
    {
        var carrera = await _carreraRepository.FindFirstOrDefaultAsync(
            c => c.IdCarrera == estudiante.IdCarrera && !c.IsDelete);

        if (carrera == null)
            throw new InvalidOperationException($"La carrera con ID {estudiante.IdCarrera} no existe.");

        if (estudiante.Id == Guid.Empty)
            estudiante.Id = Guid.NewGuid();

        var password = estudiante.Password;

        await _userRepository.CreateUser(estudiante, password);

        estudiante.Password = string.Empty;
        await _estudianteRepository.CreateAsync(estudiante);

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

