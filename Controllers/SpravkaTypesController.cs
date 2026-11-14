using Microsoft.AspNetCore.Mvc;
using MedicalApi.Models;
using MedicalApi.Data;
using Microsoft.EntityFrameworkCore;

namespace MedicalApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpravkaTypesController : ControllerBase
{
    private readonly MedicalDbContext _db;

    public SpravkaTypesController(MedicalDbContext db)
    {
        _db = db;
    }

    private string? ValidateType(SpravkaType type)
    {
        if (string.IsNullOrWhiteSpace(type.Name))
            return "Certificate type name cannot be empty.";
        if (type.Price <= 0)
            return "Price must be greater than zero.";
        if (type.ValidityDays <= 0)
            return "Validity period must be greater than 0 days.";
        return null;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<SpravkaType>), 200)]
    public async Task<ActionResult<List<SpravkaType>>> GetAll()
    {
        return Ok(await _db.SpravkaTypes.ToListAsync());
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SpravkaType), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<SpravkaType>> GetById(int id)
    {
        var spravkaType = await _db.SpravkaTypes.FirstOrDefaultAsync(t => t.Id == id);
        if (spravkaType == null) return NotFound("Certificate type not found.");
        return Ok(spravkaType);
    }

    [HttpPost]
    [ProducesResponseType(typeof(SpravkaType), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<SpravkaType>> Create(SpravkaType spravkaType)
    {
        var error = ValidateType(spravkaType);
        if (error != null) return BadRequest(error);

        _db.SpravkaTypes.Add(spravkaType);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = spravkaType.Id }, spravkaType);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(SpravkaType), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<SpravkaType>> Update(int id, SpravkaType updatedType)
    {
        var spravkaType = await _db.SpravkaTypes.FirstOrDefaultAsync(t => t.Id == id);
        if (spravkaType == null) return NotFound("Certificate type not found.");

        var error = ValidateType(updatedType);
        if (error != null) return BadRequest(error);

        spravkaType.Name = updatedType.Name;
        spravkaType.Price = updatedType.Price;
        spravkaType.ValidityDays = updatedType.ValidityDays;
        spravkaType.RequiresDoctorExam = updatedType.RequiresDoctorExam;

        await _db.SaveChangesAsync();
        return Ok(spravkaType);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> Delete(int id)
    {
        var spravkaType = await _db.SpravkaTypes.FirstOrDefaultAsync(t => t.Id == id);
        if (spravkaType == null) return NotFound("Certificate type not found.");

        _db.SpravkaTypes.Remove(spravkaType);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}