using Microsoft.AspNetCore.Mvc;
using UeesDigital.Application.Services;
using UeesDigital.DTOs;

namespace UeesDigital.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginRequestDto request)
    {
        var result = await _authService.Login(request.Email.Trim(), request.Password, request.Remember);

        if (!result.Success)
            return Unauthorized(new ErrorResponseDto { Message = result.Error });

        return Ok(new AuthResponseDto { Token = result.Token! });
    }
}
