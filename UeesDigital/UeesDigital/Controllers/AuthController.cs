using Microsoft.AspNetCore.Mvc;
using UeesDigital.Application.Services;
using UeesDigital.Domain.Entities;
using UeesDigital.DTOs;

namespace UeesDigital.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<EstudianteResponseDto>> Register(RegisterRequestDto request)
    {
        try
        {
            var estudiante = new Estudiante
            {
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Email = request.Email.Trim(),
                Password = request.Password,
                Carnet = request.Carnet,
                IdCarrera = request.IdCarrera
            };

            var created = await _authService.RegisterUser(estudiante);
            return CreatedAtAction("GetByCarnet", "Estudiantes", new { carnet = created.Carnet }, ToDto(created));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ErrorResponseDto { Message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginRequestDto request)
    {
        var token = await _authService.Login(request.Email.Trim(), request.Password, request.Remember);

        if (token == "Usuario no encontrado" || token == "Creedenciales invalidas")
        {
            return Unauthorized(new ErrorResponseDto { Message = token });
        }

        return Ok(new AuthResponseDto { Token = token });
    }

    private static EstudianteResponseDto ToDto(Estudiante estudiante)
    {
        return new EstudianteResponseDto
        {
            Id = estudiante.Id,
            FirstName = estudiante.FirstName,
            LastName = estudiante.LastName,
            FullName = estudiante.FullName,
            Email = estudiante.Email,
            Carnet = estudiante.Carnet,
            IdCarrera = estudiante.IdCarrera,
            CarreraNombre = estudiante.Carrera?.Nombre
        };
    }
}
