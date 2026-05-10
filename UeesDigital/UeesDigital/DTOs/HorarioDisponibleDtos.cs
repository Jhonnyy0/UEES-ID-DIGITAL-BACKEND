using System.ComponentModel.DataAnnotations;

namespace UeesDigital.DTOs;

public class HorarioDisponibleRequestDto
{
    [Range(1, int.MaxValue)]
    public int IdFechaDisponible { get; set; }

    [Required]
    public DateTime HoraInicio { get; set; }

    [Range(1, int.MaxValue)]
    public int CuposMaximos { get; set; } = 5;

    [Range(0, int.MaxValue)]
    public int CuposOcupados { get; set; }

    public bool Activo { get; set; } = true;
}

public class HorarioDisponibleResumenDto
{
    public int IdHorario { get; set; }
    public DateTime HoraInicio { get; set; }
    public int CuposMaximos { get; set; }
    public int CuposOcupados { get; set; }
    public int CuposDisponibles { get; set; }
    public bool Activo { get; set; }
}

public class HorarioDisponibleResponseDto : HorarioDisponibleResumenDto
{
    public int IdFechaDisponible { get; set; }
    public DateTime? Fecha { get; set; }
}
