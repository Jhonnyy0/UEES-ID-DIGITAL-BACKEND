using System.ComponentModel.DataAnnotations;

namespace UeesDigital.DTOs;

public class CarreraRequestDto
{
    [Range(1, int.MaxValue)]
    public int IdFacultad { get; set; }

    [Required]
    public string Nombre { get; set; } = string.Empty;
}

public class CarreraResponseDto
{
    public int IdCarrera { get; set; }
    public int IdFacultad { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? FacultadNombre { get; set; }
    public int EstudiantesTotal { get; set; }
}
