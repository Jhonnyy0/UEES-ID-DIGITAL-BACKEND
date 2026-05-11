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
        // Consulta directa con todos los joins necesarios
        var query = _context.Tramites
            .Include(t => t.Estudiante)
                .ThenInclude(e => e.Carrera)
                    .ThenInclude(c => c.Facultad)
            .Include(t => t.Horario)
                .ThenInclude(h => h.FechaDisponible)
            .Where(t => !t.IsDelete);

        // Filtro de búsqueda por nombre de estudiante o código de confirmación
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
            .Include(t => t.Estudiante)
                .ThenInclude(e => e.Carrera)
                    .ThenInclude(c => c.Facultad)
            .Include(t => t.Horario)
                .ThenInclude(h => h.FechaDisponible)
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
            Estado = EstadoTramite.Pendiente
        };

        var created = await _tramiteService.Add(tramite);

        // Recargamos con todos los joins para retornar datos completos
        var full = await _context.Tramites
            .Include(t => t.Estudiante)
                .ThenInclude(e => e.Carrera)
                    .ThenInclude(c => c.Facultad)
            .Include(t => t.Horario)
                .ThenInclude(h => h.FechaDisponible)
            .FirstOrDefaultAsync(t => t.IdTramite == created.IdTramite);

        return CreatedAtAction(nameof(GetById), new { id = created.IdTramite }, ToDto(full ?? created));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TramiteResponseDto>> Update(Guid id, UpdateTramiteRequestDto request)
    {
        var tramite = await _tramiteService.FindByIdAsync(id);
        if (tramite is null) return NotFound();

        tramite.IdHorario = request.IdHorario;
        tramite.TipoTramite = request.TipoTramite;
        tramite.Estado = request.Estado;

        await _tramiteService.Update(tramite);

        // Retornamos con datos completos
        var full = await _context.Tramites
            .Include(t => t.Estudiante)
                .ThenInclude(e => e.Carrera)
                    .ThenInclude(c => c.Facultad)
            .Include(t => t.Horario)
                .ThenInclude(h => h.FechaDisponible)
            .FirstOrDefaultAsync(t => t.IdTramite == id);

        return Ok(ToDto(full ?? tramite));
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

    // Mapeo completo: Tramite → TramiteResponseDto
    private static TramiteResponseDto ToDto(Tramite t) => new()
    {
        IdTramite = t.IdTramite,
        IdHorario = t.IdHorario,
        IdEstudiante = t.IdEstudiante,
        FechaRegistro = t.FechaRegistro,
        CodigoConfirmacion = t.CodigoConfirmacion,
        TipoTramite = t.TipoTramite,
        Estado = t.Estado,

        // Estudiante (tabla Estudiantes)
        EstudianteNombre = t.Estudiante != null
                             ? $"{t.Estudiante.FirstName} {t.Estudiante.LastName}"
                             : null,
        EstudianteCarnet = t.Estudiante?.Carnet,

        // Carrera (tabla Carreras)
        CarreraNombre = t.Estudiante?.Carrera?.Nombre,

        // Facultad (tabla Facultades)
        FacultadNombre = t.Estudiante?.Carrera?.Facultad?.Nombre,

        // Horario y Fecha
        FechaCita = t.Horario?.FechaDisponible?.Fecha,
        HoraInicio = t.Horario?.HoraInicio,
    };

    private static int NormalizeTake(int take) => take is < 1 or > 100 ? 20 : take;
    private static int NormalizePage(int page) => page < 1 ? 1 : page;
}