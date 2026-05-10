using System.ComponentModel.DataAnnotations;
using UeesDigital.Domain.Entities;

namespace UeesDigital.DTOs;

public class CreateTramiteRequestDto
{
    /// <example>1</example>
    [Range(1, int.MaxValue)]
    public int IdHorario { get; set; }

    /// <example>7e7eec7a-66b7-4766-9bac-0a08f296006b</example>
    [Required]
    public Guid IdEstudiante { get; set; }

    /// <example>PrimeraVez</example>
    [Required]
    public TipoTramite TipoTramite { get; set; }
}

public class UpdateTramiteRequestDto
{
    /// <example>1</example>
    [Range(1, int.MaxValue)]
    public int IdHorario { get; set; }

    /// <example>PrimeraVez</example>
    [Required]
    public TipoTramite TipoTramite { get; set; }

    /// <example>Pendiente</example>
    [Required]
    public EstadoTramite Estado { get; set; }
}

public class TramiteResponseDto
{
    public Guid          IdTramite          { get; set; }
    public int           IdHorario          { get; set; }
    public Guid          IdEstudiante       { get; set; }
    public DateTime      FechaRegistro      { get; set; }
    public string        CodigoConfirmacion { get; set; } = string.Empty;
    public TipoTramite   TipoTramite        { get; set; }
    public EstadoTramite Estado             { get; set; }
    public string?       EstudianteNombre   { get; set; }
    public DateTime?     FechaCita          { get; set; }
    public DateTime?     HoraInicio         { get; set; }
}
