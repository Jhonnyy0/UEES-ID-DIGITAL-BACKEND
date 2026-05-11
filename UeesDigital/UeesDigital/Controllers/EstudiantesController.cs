using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UeesDigital.Domain.Entities;
using UeesDigital.Domain.Interfaces;
using UeesDigital.DTOs;
using UeesDigital.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace UeesDigital.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EstudiantesController : ControllerBase
{
    private readonly IEstudianteRepository _estudianteRepository;
    private readonly ICarreraRepository _carreraRepository;
    private readonly ApplicationDbContext _context;

    public EstudiantesController(
        IEstudianteRepository estudianteRepository,
        ICarreraRepository carreraRepository,
        ApplicationDbContext context)
    {
        _estudianteRepository = estudianteRepository;
        _carreraRepository = carreraRepository;
        _context = context;
    }

    // GET /api/estudiantes/carnet/{carnet} — público (chatbot)
    [HttpGet("carnet/{carnet}")]
    public async Task<ActionResult<EstudianteResponseDto>> GetByCarnet(string carnet)
    {
        var estudiante = await _estudianteRepository.GetByCIFAsync(carnet);
        return estudiante is null ? NotFound() : Ok(ToDto(estudiante));
    }

    // GET /api/estudiantes — Admin/Gestor
    [Authorize(Roles = "Admin,Gestor")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EstudianteResponseDto>>> GetAll()
    {
        var estudiantes = await _estudianteRepository.GetAllAsync();
        return Ok(estudiantes.Select(ToDto));
    }

    // GET /api/estudiantes/{id} — Admin/Gestor
    [Authorize(Roles = "Admin,Gestor")]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EstudianteResponseDto>> GetById(Guid id)
    {
        var est = await _context.Estudiantes
            .Include(e => e.Carrera).ThenInclude(c => c.Facultad)
            .FirstOrDefaultAsync(e => e.Id == id);
        return est is null ? NotFound() : Ok(ToDto(est));
    }

    // POST /api/estudiantes — Admin/Gestor
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
            Id = Guid.NewGuid(),
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = request.Email.Trim(),
            Carnet = request.Carnet,
            IdCarrera = request.IdCarrera,
        };

        var created = await _estudianteRepository.CreateAsync(estudiante);
        return CreatedAtAction(nameof(GetByCarnet), new { carnet = created.Carnet }, ToDto(created));
    }

    // PUT /api/estudiantes/{id} — Admin/Gestor
    [Authorize(Roles = "Admin,Gestor")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<EstudianteResponseDto>> Update(Guid id, ActualizarEstudianteRequestDto request)
    {
        var est = await _context.Estudiantes.FindAsync(id);
        if (est is null) return NotFound();

        // Validar que la carrera existe
        var carrera = await _carreraRepository.FindFirstOrDefaultAsync(
            c => c.IdCarrera == request.IdCarrera && !c.IsDelete);
        if (carrera == null)
            return BadRequest(new ErrorResponseDto { Message = $"La carrera con ID {request.IdCarrera} no existe." });

        est.FirstName = request.FirstName.Trim();
        est.LastName = request.LastName.Trim();
        est.Email = request.Email.Trim();
        est.Carnet = request.Carnet;
        est.IdCarrera = request.IdCarrera;

        await _context.SaveChangesAsync();

        // Retornar con datos completos
        var updated = await _context.Estudiantes
            .Include(e => e.Carrera).ThenInclude(c => c.Facultad)
            .FirstAsync(e => e.Id == id);

        return Ok(ToDto(updated));
    }

    private static EstudianteResponseDto ToDto(Estudiante e) => new()
    {
        Id = e.Id,
        FirstName = e.FirstName,
        LastName = e.LastName,
        FullName = e.FullName,
        Email = e.Email,
        Carnet = e.Carnet,
        IdCarrera = e.IdCarrera,
        CarreraNombre = e.Carrera?.Nombre,
        FacultadNombre = e.Carrera?.Facultad?.Nombre,
    };
}