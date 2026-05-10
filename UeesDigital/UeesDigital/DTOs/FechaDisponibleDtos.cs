using System.ComponentModel.DataAnnotations;

namespace UeesDigital.DTOs;

public class FechaDisponibleRequestDto
{
    [Required]
    public DateTime Fecha { get; set; }

    public bool Activo { get; set; } = true;
}

public class FechaDisponibleResponseDto
{
    public int IdFechaDisponible { get; set; }
    public DateTime Fecha { get; set; }
    public bool Activo { get; set; }
    public IEnumerable<HorarioDisponibleResumenDto> Horarios { get; set; } = [];
}
