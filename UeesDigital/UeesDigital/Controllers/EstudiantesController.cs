using Microsoft.AspNetCore.Mvc;
using UeesDigital.Domain.Entities;
using UeesDigital.Domain.Interfaces;
using UeesDigital.DTOs;

namespace UeesDigital.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EstudiantesController : ControllerBase
{
    private readonly IEstudianteRepository _estudianteRepository;

    public EstudiantesController(IEstudianteRepository estudianteRepository)
    {
        _estudianteRepository = estudianteRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EstudianteResponseDto>>> GetAll()
    {
        var estudiantes = await _estudianteRepository.GetAllAsync();
        return Ok(estudiantes.Select(ToDto));
    }

    [HttpGet("carnet/{carnet}")]
    public async Task<ActionResult<EstudianteResponseDto>> GetByCarnet(string carnet)
    {
        var estudiante = await _estudianteRepository.GetByCIFAsync(carnet);
        return estudiante is null ? NotFound() : Ok(ToDto(estudiante));
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
