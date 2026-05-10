using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UeesDigital.Application.Services;
using UeesDigital.Domain.Entities;
using UeesDigital.DTOs;

namespace UeesDigital.Controllers;

[Authorize(Roles = "Admin,Gestor")]
[ApiController]
[Route("api/[controller]")]
public class TramitesController : ControllerBase
{
    private readonly TramiteService _tramiteService;

    public TramitesController(TramiteService tramiteService)
    {
        _tramiteService = tramiteService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TramiteResponseDto>>> GetAll(
        [FromQuery] int take = 20,
        [FromQuery] int page = 1,
        [FromQuery] string search = "")
    {
        var tramites = await _tramiteService.GetAll(NormalizeTake(take), NormalizePage(page), search ?? string.Empty);
        return Ok(tramites.Select(ToDto));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TramiteResponseDto>> GetById(Guid id)
    {
        var tramite = await _tramiteService.FindByIdAsync(id);
        return tramite is null ? NotFound() : Ok(ToDto(tramite));
    }

    // AllowAnonymous permite que el chatbot público cree trámites sin token
    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<TramiteResponseDto>> Create(CreateTramiteRequestDto request)
    {
        var tramite = new Tramite
        {
            IdHorario = request.IdHorario,
            IdEstudiante = request.IdEstudiante,
            TipoTramite = request.TipoTramite,
            Estado = EstadoTramite.Pendiente
        };

        var created = await _tramiteService.Add(tramite);
        return CreatedAtAction(nameof(GetById), new { id = created.IdTramite }, ToDto(created));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TramiteResponseDto>> Update(Guid id, UpdateTramiteRequestDto request)
    {
        var tramite = await _tramiteService.FindByIdAsync(id);
        if (tramite is null) return NotFound();

        tramite.IdHorario = request.IdHorario;
        tramite.TipoTramite = request.TipoTramite;
        tramite.Estado = request.Estado;

        var updated = await _tramiteService.Update(tramite);
        return Ok(ToDto(updated));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var tramite = await _tramiteService.FindByIdAsync(id);
        if (tramite is null) return NotFound();

        tramite.Estado = EstadoTramite.Cancelado;
        await _tramiteService.Update(tramite);
        return NoContent();
    }

    private static TramiteResponseDto ToDto(Tramite t) => new()
    {
        IdTramite = t.IdTramite,
        IdHorario = t.IdHorario,
        IdEstudiante = t.IdEstudiante,
        FechaRegistro = t.FechaRegistro,
        CodigoConfirmacion = t.CodigoConfirmacion,
        TipoTramite = t.TipoTramite,
        Estado = t.Estado,
        EstudianteNombre = t.Estudiante?.FullName,
        FechaCita = t.Horario?.FechaDisponible?.Fecha,
        HoraInicio = t.Horario?.HoraInicio
    };

    private static int NormalizeTake(int take) => take is < 1 or > 100 ? 20 : take;
    private static int NormalizePage(int page) => page < 1 ? 1 : page;
}