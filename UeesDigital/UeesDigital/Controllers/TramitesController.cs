using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UeesDigital.Application.Services;
using UeesDigital.Domain.Entities;
using UeesDigital.DTOs;
using UeesDigital.Infrastructure.Persistence;

namespace UeesDigital.Controllers;

[Authorize(Roles = "Admin,Gestor")]
[ApiController]
[Route("api/[controller]")]
public class TramitesController : ControllerBase
{
    private readonly TramiteService _tramiteService;
    private readonly ApplicationDbContext _context;

    public TramitesController(TramiteService tramiteService, ApplicationDbContext context)
    {
        _tramiteService = tramiteService;
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TramiteResponseDto>>> GetAll(
        [FromQuery] int take = 20,
        [FromQuery] int page = 1,
        [FromQuery] string search = "")
    {
        var query = _context.Tramites
            .Include(t => t.Estudiante).ThenInclude(e => e.Carrera).ThenInclude(c => c.Facultad)
            .Include(t => t.Horario).ThenInclude(h => h.FechaDisponible)
            .Where(t => !t.IsDelete);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.ToLower();
            query = query.Where(t =>
                (t.Estudiante != null &&
                 (t.Estudiante.FirstName.ToLower().Contains(s) ||
                  t.Estudiante.LastName.ToLower().Contains(s))) ||
                t.CodigoConfirmacion.ToLower().Contains(s));
        }

        var tramites = await query
            .OrderByDescending(t => t.FechaRegistro)
            .Skip((page - 1) * take)
            .Take(take)
            .ToListAsync();

        return Ok(tramites.Select(ToDto));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TramiteResponseDto>> GetById(Guid id)
    {
        var tramite = await _context.Tramites
            .Include(t => t.Estudiante).ThenInclude(e => e.Carrera).ThenInclude(c => c.Facultad)
            .Include(t => t.Horario).ThenInclude(h => h.FechaDisponible)
            .FirstOrDefaultAsync(t => t.IdTramite == id);

        return tramite is null ? NotFound() : Ok(ToDto(tramite));
    }

    // El chatbot público puede crear trámites sin token
    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<TramiteResponseDto>> Create(CreateTramiteRequestDto request)
    {
        var tramite = new Tramite
        {
            IdHorario = request.IdHorario,
            IdEstudiante = request.IdEstudiante,
            TipoTramite = request.TipoTramite,
            Estado = EstadoTramite.Pendiente,
        };

        try
        {
            var created = await _tramiteService.Add(tramite);
            var full = await _context.Tramites
                .Include(t => t.Estudiante).ThenInclude(e => e.Carrera).ThenInclude(c => c.Facultad)
                .Include(t => t.Horario).ThenInclude(h => h.FechaDisponible)
                .FirstOrDefaultAsync(t => t.IdTramite == created.IdTramite);

            return CreatedAtAction(nameof(GetById), new { id = created.IdTramite }, ToDto(full ?? created));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TramiteResponseDto>> Update(Guid id, UpdateTramiteRequestDto request)
    {
        // ── CLAVE: leemos el estado ACTUAL con AsNoTracking para no contaminar el contexto
        var estadoActual = await _context.Tramites
            .AsNoTracking()
            .Where(t => t.IdTramite == id)
            .Select(t => new { t.Estado, t.IdHorario })
            .FirstOrDefaultAsync();

        if (estadoActual is null) return NotFound();

        try
        {
            // Ajuste de cupos ANTES de guardar el nuevo estado
            await AjustarCupos(
                horarioAnteriorId: estadoActual.IdHorario,
                horarioNuevoId: request.IdHorario,
                estadoAnterior: estadoActual.Estado,
                estadoNuevo: request.Estado
            );

            // Actualizar el trámite directamente
            await _context.Tramites
                .Where(t => t.IdTramite == id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(t => t.Estado, request.Estado)
                    .SetProperty(t => t.TipoTramite, request.TipoTramite)
                    .SetProperty(t => t.IdHorario, request.IdHorario));

            // Retornar datos completos
            var full = await _context.Tramites
                .Include(t => t.Estudiante).ThenInclude(e => e.Carrera).ThenInclude(c => c.Facultad)
                .Include(t => t.Horario).ThenInclude(h => h.FechaDisponible)
                .FirstOrDefaultAsync(t => t.IdTramite == id);

            return Ok(ToDto(full!));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var tramite = await _context.Tramites
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.IdTramite == id);

        if (tramite is null) return NotFound();

        if (tramite.Estado != EstadoTramite.Cancelado)
        {
            // Devolver cupo
            await _context.HorariosDisponibles
                .Where(h => h.IdHorario == tramite.IdHorario && h.CuposOcupados > 0)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(h => h.CuposOcupados, h => h.CuposOcupados - 1));
        }

        await _context.Tramites
            .Where(t => t.IdTramite == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(t => t.Estado, EstadoTramite.Cancelado));

        return NoContent();
    }

    // ── Lógica centralizada de cupos ─────────────────────────────────────────
    private async Task AjustarCupos(
        int horarioAnteriorId, int horarioNuevoId,
        EstadoTramite estadoAnterior, EstadoTramite estadoNuevo)
    {
        bool anteriorActivo = estadoAnterior != EstadoTramite.Cancelado;
        bool nuevoActivo = estadoNuevo != EstadoTramite.Cancelado;
        bool cambiaHorario = horarioAnteriorId != horarioNuevoId;

        if (anteriorActivo && !nuevoActivo)
        {
            // Se cancela → devolver cupo al horario actual
            await _context.HorariosDisponibles
                .Where(h => h.IdHorario == horarioAnteriorId && h.CuposOcupados > 0)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(h => h.CuposOcupados, h => h.CuposOcupados - 1));
        }
        else if (!anteriorActivo && nuevoActivo)
        {
            // Se reactiva → descontar cupo del horario nuevo
            var horario = await _context.HorariosDisponibles
                .FirstOrDefaultAsync(h => h.IdHorario == horarioNuevoId);
            if (horario != null && horario.CuposOcupados >= horario.CuposMaximos)
                throw new InvalidOperationException("No hay cupos disponibles para reactivar el trámite.");

            await _context.HorariosDisponibles
                .Where(h => h.IdHorario == horarioNuevoId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(h => h.CuposOcupados, h => h.CuposOcupados + 1));
        }
        else if (anteriorActivo && nuevoActivo && cambiaHorario)
        {
            // Cambia de horario sin cancelar → devolver anterior, descontar nuevo
            await _context.HorariosDisponibles
                .Where(h => h.IdHorario == horarioAnteriorId && h.CuposOcupados > 0)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(h => h.CuposOcupados, h => h.CuposOcupados - 1));

            var horarioNuevo = await _context.HorariosDisponibles
                .FirstOrDefaultAsync(h => h.IdHorario == horarioNuevoId);
            if (horarioNuevo != null && horarioNuevo.CuposOcupados >= horarioNuevo.CuposMaximos)
                throw new InvalidOperationException("No hay cupos disponibles en el nuevo horario.");

            await _context.HorariosDisponibles
                .Where(h => h.IdHorario == horarioNuevoId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(h => h.CuposOcupados, h => h.CuposOcupados + 1));
        }
        // Si anteriorActivo && nuevoActivo && !cambiaHorario → no cambian cupos
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
        EstudianteNombre = t.Estudiante != null ? $"{t.Estudiante.FirstName} {t.Estudiante.LastName}" : null,
        EstudianteCarnet = t.Estudiante?.Carnet,
        CarreraNombre = t.Estudiante?.Carrera?.Nombre,
        FacultadNombre = t.Estudiante?.Carrera?.Facultad?.Nombre,
        FechaCita = t.Horario?.FechaDisponible?.Fecha,
        HoraInicio = t.Horario?.HoraInicio,
    };
}