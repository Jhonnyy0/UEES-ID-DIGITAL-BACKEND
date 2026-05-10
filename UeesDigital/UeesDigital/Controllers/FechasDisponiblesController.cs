using Microsoft.AspNetCore.Mvc;
using UeesDigital.Application.Services;
using UeesDigital.Domain.Entities;
using UeesDigital.DTOs;

namespace UeesDigital.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FechasDisponiblesController : ControllerBase
{
    private readonly FechaDisponibleService _fechaDisponibleService;

    public FechasDisponiblesController(FechaDisponibleService fechaDisponibleService)
    {
        _fechaDisponibleService = fechaDisponibleService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FechaDisponibleResponseDto>>> GetAll(
        [FromQuery] int take = 20,
        [FromQuery] int page = 1,
        [FromQuery] string search = "")
    {
        var fechas = await _fechaDisponibleService.GetAll(NormalizeTake(take), NormalizePage(page), search ?? string.Empty);
        return Ok(fechas.Select(ToDto));
    }

    [HttpGet("activas")]
    public async Task<ActionResult<IEnumerable<FechaDisponibleResponseDto>>> GetActivas()
    {
        var fechas = await _fechaDisponibleService.GetActivasAsync();
        return Ok(fechas.Select(ToDto));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<FechaDisponibleResponseDto>> GetById(int id)
    {
        var fecha = await _fechaDisponibleService.FindByIdAsync(id);
        return fecha is null ? NotFound() : Ok(ToDto(fecha));
    }

    [HttpPost]
    public async Task<ActionResult<FechaDisponibleResponseDto>> Create(FechaDisponibleRequestDto request)
    {
        var fecha = new FechaDisponible
        {
            Fecha = request.Fecha,
            Activo = request.Activo
        };

        var created = await _fechaDisponibleService.Add(fecha);
        return CreatedAtAction(nameof(GetById), new { id = created.IdFechaDisponible }, ToDto(created));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<FechaDisponibleResponseDto>> Update(int id, FechaDisponibleRequestDto request)
    {
        var fecha = await _fechaDisponibleService.FindByIdAsync(id);
        if (fecha is null)
        {
            return NotFound();
        }

        fecha.Fecha = request.Fecha;
        fecha.Activo = request.Activo;

        var updated = await _fechaDisponibleService.Update(fecha);
        return Ok(ToDto(updated));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _fechaDisponibleService.Delete(id);
        return deleted ? NoContent() : NotFound();
    }

    private static FechaDisponibleResponseDto ToDto(FechaDisponible fecha)
    {
        return new FechaDisponibleResponseDto
        {
            IdFechaDisponible = fecha.IdFechaDisponible,
            Fecha = fecha.Fecha,
            Activo = fecha.Activo,
            Horarios = fecha.Horarios?.Select(HorarioToResumenDto) ?? []
        };
    }

    private static HorarioDisponibleResumenDto HorarioToResumenDto(HorarioDisponible horario)
    {
        return new HorarioDisponibleResumenDto
        {
            IdHorario = horario.IdHorario,
            HoraInicio = horario.HoraInicio,
            CuposMaximos = horario.CuposMaximos,
            CuposOcupados = horario.CuposOcupados,
            CuposDisponibles = Math.Max(0, horario.CuposMaximos - horario.CuposOcupados),
            Activo = horario.Activo
        };
    }

    private static int NormalizeTake(int take) => take is < 1 or > 100 ? 20 : take;
    private static int NormalizePage(int page) => page < 1 ? 1 : page;
}
