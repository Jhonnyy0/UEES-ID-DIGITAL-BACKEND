namespace UeesDigital.DTOs;

public class EstudianteResponseDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Carnet { get; set; }
    public int IdCarrera { get; set; }
    public string? CarreraNombre { get; set; }
}
