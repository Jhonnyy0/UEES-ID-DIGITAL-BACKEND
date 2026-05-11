using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UeesDigital.Domain.Entities;
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

    // GET /api/facultades — público (usado por chatbot y selects del admin)
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var facultades = await _context.Facultades
            .OrderBy(f => f.IdFacultad)
            .Select(f => new { f.IdFacultad, f.Nombre, f.Codigo })
            .ToListAsync();
        return Ok(facultades);
    }

    // GET /api/facultades/{id}
    [Authorize(Roles = "Admin,Gestor")]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var f = await _context.Facultades.FindAsync(id);
        return f is null ? NotFound() : Ok(new { f.IdFacultad, f.Nombre, f.Codigo });
    }

    // POST /api/facultades  { nombre, codigo }
    [Authorize(Roles = "Admin,Gestor")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] FacultadRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombre) || string.IsNullOrWhiteSpace(dto.Codigo))
            return BadRequest(new { message = "Nombre y Código son obligatorios." });

        var existe = await _context.Facultades
            .AnyAsync(f => f.Codigo.ToLower() == dto.Codigo.ToLower());
        if (existe)
            return BadRequest(new { message = $"Ya existe una facultad con el código '{dto.Codigo}'." });

        var facultad = new Facultad
        {
            Nombre = dto.Nombre.Trim(),
            Codigo = dto.Codigo.Trim().ToUpper(),
        };

        _context.Facultades.Add(facultad);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = facultad.IdFacultad },
            new { facultad.IdFacultad, facultad.Nombre, facultad.Codigo });
    }

    // PUT /api/facultades/{id}  { nombre, codigo }
    [Authorize(Roles = "Admin,Gestor")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] FacultadRequestDto dto)
    {
        var facultad = await _context.Facultades.FindAsync(id);
        if (facultad is null) return NotFound();

        // Verificar código duplicado (excluyendo la misma facultad)
        var codigoDup = await _context.Facultades
            .AnyAsync(f => f.Codigo.ToLower() == dto.Codigo.ToLower() && f.IdFacultad != id);
        if (codigoDup)
            return BadRequest(new { message = $"Ya existe otra facultad con el código '{dto.Codigo}'." });

        facultad.Nombre = dto.Nombre.Trim();
        facultad.Codigo = dto.Codigo.Trim().ToUpper();
        await _context.SaveChangesAsync();
        return Ok(new { facultad.IdFacultad, facultad.Nombre, facultad.Codigo });
    }

    // DELETE /api/facultades/{id} — solo Admin
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var facultad = await _context.Facultades.FindAsync(id);
        if (facultad is null) return NotFound();

        var tieneCarreras = await _context.Carreras.AnyAsync(c => c.IdFacultad == id && !c.IsDelete);
        if (tieneCarreras)
            return BadRequest(new { message = "No se puede eliminar: la facultad tiene carreras asociadas." });

        _context.Facultades.Remove(facultad);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

public class FacultadRequestDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
}