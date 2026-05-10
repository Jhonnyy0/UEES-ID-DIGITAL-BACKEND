using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UeesDigital.Application.Services;
using UeesDigital.Domain.Entities;
using UeesDigital.DTOs;

namespace UeesDigital.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarrerasController : ControllerBase
{
    private readonly CarreraService _carreraService;

    public CarrerasController(CarreraService carreraService)
    {
        _carreraService = carreraService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CarreraResponseDto>>> GetAll(
        [FromQuery] int take = 20, [FromQuery] int page = 1, [FromQuery] string search = "")
    {
        var carreras = await _carreraService.GetAll(NormalizeTake(take), NormalizePage(page), search ?? string.Empty);
        return Ok(carreras.Select(ToDto));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CarreraResponseDto>> GetById(int id)
    {
        var carrera = await _carreraService.FindByIdAsync(id);
        return carrera is null ? NotFound() : Ok(ToDto(carrera));
    }

    [Authorize(Roles = "Admin,Gestor")]
    [HttpPost]
    public async Task<ActionResult<CarreraResponseDto>> Create(CarreraRequestDto request)
    {
        var carrera = new Carrera { IdFacultad = request.IdFacultad, Nombre = request.Nombre.Trim() };
        var created = await _carreraService.Add(carrera);
        return CreatedAtAction(nameof(GetById), new { id = created.IdCarrera }, ToDto(created));
    }

    [Authorize(Roles = "Admin,Gestor")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<CarreraResponseDto>> Update(int id, CarreraRequestDto request)
    {
        var carrera = await _carreraService.FindByIdAsync(id);
        if (carrera is null) return NotFound();

        carrera.IdFacultad = request.IdFacultad;
        carrera.Nombre     = request.Nombre.Trim();

        var updated = await _carreraService.Update(carrera);
        return Ok(ToDto(updated));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _carreraService.Delete(id);
        return deleted ? NoContent() : NotFound();
    }

    private static CarreraResponseDto ToDto(Carrera c) => new()
    {
        IdCarrera       = c.IdCarrera,
        IdFacultad      = c.IdFacultad,
        Nombre          = c.Nombre,
        FacultadNombre  = c.Facultad?.Nombre,
        EstudiantesTotal = c.Estudiantes?.Count ?? 0
    };

    private static int NormalizeTake(int take) => take is < 1 or > 100 ? 20 : take;
    private static int NormalizePage(int page) => page < 1 ? 1 : page;
}
