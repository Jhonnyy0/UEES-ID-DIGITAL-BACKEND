using System.ComponentModel.DataAnnotations;

namespace UeesDigital.DTOs;

public class RegisterRequestDto
{
    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int Carnet { get; set; }

    [Range(1, int.MaxValue)]
    public int IdCarrera { get; set; }
}

public class LoginRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    public bool Remember { get; set; }
}

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
}
