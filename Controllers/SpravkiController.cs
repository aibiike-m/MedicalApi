using Microsoft.AspNetCore.Mvc;
using MedicalApi.Models;
using MedicalApi.Data;
using Microsoft.EntityFrameworkCore;

namespace MedicalApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpravkiController : ControllerBase
{
    private readonly MedicalDbContext _db;

    public SpravkiController(MedicalDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<Spravka>), 200)]
    public async Task<ActionResult<List<Spravka>>> GetAll([FromQuery] int? patientId)
    {
        if (patientId.HasValue)
        {
            var filtered = await _db.Spravki.Where(s => s.PatientId == patientId.Value).ToListAsync();
            return Ok(filtered);
        }
        return Ok(await _db.Spravki.ToListAsync());
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Spravka), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<Spravka>> GetById(int id)
    {
        Spravka? spravka = await _db.Spravki.FirstOrDefaultAsync(s => s.Id == id);
        if (spravka == null) return NotFound("Certificate not found.");
        return Ok(spravka);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Spravka), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<Spravka>> Create(Spravka spravka)
    {
        // Проверяем через базу, существует ли пациент
        var patientExists = await _db.Patients.AnyAsync(p => p.Id == spravka.PatientId);
        if (!patientExists)
            return BadRequest("Specified patient does not exist.");

        // Проверяем через базу, существует ли тип справки
        var typeExists = await _db.SpravkaTypes.AnyAsync(t => t.Id == spravka.SpravkaTypeId);
        if (!typeExists)
            return BadRequest("Specified certificate type not found in catalog.");

        spravka.IssueDate = DateTime.UtcNow;
        _db.Spravki.Add(spravka);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = spravka.Id }, spravka);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Spravka), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<Spravka>> Update(int id, Spravka updatedSpravka)
    {
        Spravka? spravka = await _db.Spravki.FirstOrDefaultAsync(s => s.Id == id);
        if (spravka == null) return NotFound("Certificate not found.");

        var patientExists = await _db.Patients.AnyAsync(p => p.Id == updatedSpravka.PatientId);
        if (!patientExists)
            return BadRequest("Specified patient does not exist.");

        var typeExists = await _db.SpravkaTypes.AnyAsync(t => t.Id == updatedSpravka.SpravkaTypeId);
        if (!typeExists)
            return BadRequest("Specified certificate type not found in catalog.");

        spravka.PatientId = updatedSpravka.PatientId;
        spravka.SpravkaTypeId = updatedSpravka.SpravkaTypeId;

        await _db.SaveChangesAsync();
        return Ok(spravka);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> Delete(int id)
    {
        Spravka? spravka = await _db.Spravki.FirstOrDefaultAsync(s => s.Id == id);
        if (spravka == null) return NotFound("Certificate not found.");

        _db.Spravki.Remove(spravka);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}