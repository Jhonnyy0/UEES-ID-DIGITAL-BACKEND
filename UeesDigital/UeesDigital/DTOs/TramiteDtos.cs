using System.ComponentModel.DataAnnotations;
using UeesDigital.Domain.Entities;

namespace UeesDigital.DTOs;

public class CreateTramiteRequestDto
{
    [Range(1, int.MaxValue)]
    public int IdHorario { get; set; }

    [Required]
    public Guid IdEstudiante { get; set; }

    [Required]
    public TipoTramite TipoTramite { get; set; }
}

public class UpdateTramiteRequestDto
{
    [Range(1, int.MaxValue)]
    public int IdHorario { get; set; }

    [Required]
    public TipoTramite TipoTramite { get; set; }

    [Required]
    public EstadoTramite Estado { get; set; }
}

public class TramiteResponseDto
{
    public Guid IdTramite { get; set; }
    public int IdHorario { get; set; }
    public Guid IdEstudiante { get; set; }
    public DateTime FechaRegistro { get; set; }
    public string CodigoConfirmacion { get; set; } = string.Empty;
    public TipoTramite TipoTramite { get; set; }
    public EstadoTramite Estado { get; set; }

    // Datos del estudiante
    public string? EstudianteNombre { get; set; }
    public int? EstudianteCarnet { get; set; }       // ← NUEVO: Estudiante.Carnet
    public string? CarreraNombre { get; set; }        // ← NUEVO: Estudiante.Carrera.Nombre
    public string? FacultadNombre { get; set; }       // ← NUEVO: Estudiante.Carrera.Facultad.Nombre

    // Datos del horario
    public DateTime? FechaCita { get; set; }
    public DateTime? HoraInicio { get; set; }
}