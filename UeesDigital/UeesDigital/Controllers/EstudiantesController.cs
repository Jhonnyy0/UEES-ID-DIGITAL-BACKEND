using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UeesDigital.Application.Services;
using UeesDigital.Domain.Entities;
using UeesDigital.Domain.Interfaces;
using UeesDigital.DTOs;

namespace UeesDigital.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EstudiantesController : ControllerBase
{
    private readonly IEstudianteRepository _estudianteRepository;
    private readonly ICarreraRepository    _carreraRepository;

    public EstudiantesController(
        IEstudianteRepository estudianteRepository,
        ICarreraRepository carreraRepository)
    {
        _estudianteRepository = estudianteRepository;
        _carreraRepository    = carreraRepository;
    }

    [HttpGet("carnet/{carnet}")]
    public async Task<ActionResult<EstudianteResponseDto>> GetByCarnet(string carnet)
    {
        var estudiante = await _estudianteRepository.GetByCIFAsync(carnet);
        return estudiante is null ? NotFound() : Ok(ToDto(estudiante));
    }

    [Authorize(Roles = "Admin,Gestor")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EstudianteResponseDto>>> GetAll()
    {
        var estudiantes = await _estudianteRepository.GetAllAsync();
        return Ok(estudiantes.Select(ToDto));
    }

    [Authorize(Roles = "Admin,Gestor")]
    [HttpPost]
    public async Task<ActionResult<EstudianteResponseDto>> Create(CrearEstudianteRequestDto request)
    {
        var carrera = await _carreraRepository.FindFirstOrDefaultAsync(
            c => c.IdCarrera == request.IdCarrera && !c.IsDelete);

        if (carrera == null)
            return BadRequest(new ErrorResponseDto { Message = $"La carrera con ID {request.IdCarrera} no existe." });

        var estudiante = new Estudiante
        {
            Id        = Guid.NewGuid(),
            FirstName = request.FirstName.Trim(),
            LastName  = request.LastName.Trim(),
            Email     = request.Email.Trim(),
            Carnet    = request.Carnet,
            IdCarrera = request.IdCarrera,
        };

        var created = await _estudianteRepository.CreateAsync(estudiante);
        return CreatedAtAction(nameof(GetByCarnet), new { carnet = created.Carnet }, ToDto(created));
    }

    private static EstudianteResponseDto ToDto(Estudiante e) => new()
    {
        Id           = e.Id,
        FirstName    = e.FirstName,
        LastName     = e.LastName,
        FullName     = e.FullName,
        Email        = e.Email,
        Carnet       = e.Carnet,
        IdCarrera    = e.IdCarrera,
        CarreraNombre = e.Carrera?.Nombre
    };
}
