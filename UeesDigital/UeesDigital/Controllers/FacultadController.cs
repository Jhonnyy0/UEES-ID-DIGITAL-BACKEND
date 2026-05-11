using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UeesDigital.Infrastructure.Persistence;

namespace UeesDigital.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FacultadesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public FacultadesController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// GET /api/facultades — público, usado por el frontend para los selects
    /// </summary>
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var facultades = await _context.Facultades
            .OrderBy(f => f.IdFacultad)
            .Select(f => new
            {
                f.IdFacultad,
                f.Nombre,
                f.Codigo
            })
            .ToListAsync();

        return Ok(facultades);
    }
}