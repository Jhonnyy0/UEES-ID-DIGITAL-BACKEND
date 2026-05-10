using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UeesDigital.Application.Services;
using UeesDigital.Domain.Entities;
using UeesDigital.DTOs;

namespace UeesDigital.Controllers;

[Authorize(Roles = "Admin,Gestor")]
[ApiController]
[Route("api/[controller]")]
public class HorariosDisponiblesController : ControllerBase
{
    private readonly HorarioDisponibleService _horarioDisponibleService;

    public HorariosDisponiblesController(HorarioDisponibleService horarioDisponibleService)
    {
        _horarioDisponibleService = horarioDisponibleService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<HorarioDisponibleResponseDto>>> GetAll(
        [FromQuery] int take = 20, [FromQuery] int page = 1, [FromQuery] string search = "")
    {
        var horarios = await _horarioDisponibleService.GetAll(NormalizeTake(take), NormalizePage(page), search ?? string.Empty);
        return Ok(horarios.Select(ToDto));
    }

    [AllowAnonymous]
    [HttpGet("fecha/{idFecha:int}/disponibles")]
    public async Task<ActionResult<IEnumerable<HorarioDisponibleResponseDto>>> GetDisponiblesByFecha(int idFecha)
    {
        var horarios = await _horarioDisponibleService.GetDisponiblesByFechaAsync(idFecha);
        return Ok(horarios.Select(ToDto));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<HorarioDisponibleResponseDto>> GetById(int id)
    {
        var horario = await _horarioDisponibleService.FindByIdAsync(id);
        return horario is null ? NotFound() : Ok(ToDto(horario));
    }

    [HttpPost]
    public async Task<ActionResult<HorarioDisponibleResponseDto>> Create(HorarioDisponibleRequestDto request)
    {
        if (request.CuposOcupados > request.CuposMaximos)
            return BadRequest(new ErrorResponseDto { Message = "Los cupos ocupados no pueden superar los cupos máximos." });

        var horario = new HorarioDisponible
        {
            IdFechaDisponible = request.IdFechaDisponible,
            HoraInicio        = request.HoraInicio,
            CuposMaximos      = request.CuposMaximos,
            CuposOcupados     = request.CuposOcupados,
            Activo            = request.Activo
        };

        var created = await _horarioDisponibleService.Add(horario);
        return CreatedAtAction(nameof(GetById), new { id = created.IdHorario }, ToDto(created));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<HorarioDisponibleResponseDto>> Update(int id, HorarioDisponibleRequestDto request)
    {
        if (request.CuposOcupados > request.CuposMaximos)
            return BadRequest(new ErrorResponseDto { Message = "Los cupos ocupados no pueden superar los cupos máximos." });

        var horario = await _horarioDisponibleService.FindByIdAsync(id);
        if (horario is null) return NotFound();

        horario.IdFechaDisponible = request.IdFechaDisponible;
        horario.HoraInicio        = request.HoraInicio;
        horario.CuposMaximos      = request.CuposMaximos;
        horario.CuposOcupados     = request.CuposOcupados;
        horario.Activo            = request.Activo;

        var updated = await _horarioDisponibleService.Update(horario);
        return Ok(ToDto(updated));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _horarioDisponibleService.Delete(id);
        return deleted ? NoContent() : NotFound();
    }

    private static HorarioDisponibleResponseDto ToDto(HorarioDisponible h) => new()
    {
        IdHorario         = h.IdHorario,
        IdFechaDisponible = h.IdFechaDisponible,
        Fecha             = h.FechaDisponible?.Fecha,
        HoraInicio        = h.HoraInicio,
        CuposMaximos      = h.CuposMaximos,
        CuposOcupados     = h.CuposOcupados,
        CuposDisponibles  = Math.Max(0, h.CuposMaximos - h.CuposOcupados),
        Activo            = h.Activo
    };

    private static int NormalizeTake(int take) => take is < 1 or > 100 ? 20 : take;
    private static int NormalizePage(int page) => page < 1 ? 1 : page;
}
