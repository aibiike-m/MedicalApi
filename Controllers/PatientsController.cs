using Microsoft.AspNetCore.Mvc;
using MedicalApi.Models;
using MedicalApi.Data;
using Microsoft.EntityFrameworkCore;

namespace MedicalApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly MedicalDbContext _db;

    public PatientsController(MedicalDbContext db)
    {
        _db = db;
    }

    private string? ValidatePatient(Patient patient)
    {
        if (string.IsNullOrWhiteSpace(patient.Name))
            return "Patient name cannot be empty.";
        if (patient.BirthDate > DateOnly.FromDateTime(DateTime.UtcNow))
            return "Birth date cannot be in the future.";
        if (string.IsNullOrWhiteSpace(patient.Phone))
            return "Phone number is required.";
        return null;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<Patient>), 200)]
    public async Task<ActionResult<List<Patient>>> GetAll()
    {
        return Ok(await _db.Patients.ToListAsync());
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Patient), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<Patient>> GetById(int id)
    {
        var patient = await _db.Patients.FirstOrDefaultAsync(p => p.Id == id);
        if (patient == null) return NotFound("Patient not found.");
        return Ok(patient);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Patient), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<Patient>> Create(Patient patient)
    {
        var error = ValidatePatient(patient);
        if (error != null) return BadRequest(error);

        // ID больше не вычисляем вручную, его выдает база (SERIAL)
        _db.Patients.Add(patient);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = patient.Id }, patient);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Patient), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<Patient>> Update(int id, Patient updatedPatient)
    {
        var patient = await _db.Patients.FirstOrDefaultAsync(p => p.Id == id);
        if (patient == null) return NotFound("Patient not found.");

        var error = ValidatePatient(updatedPatient);
        if (error != null) return BadRequest(error);

        patient.Name = updatedPatient.Name;
        patient.BirthDate = updatedPatient.BirthDate;
        patient.Phone = updatedPatient.Phone;
        patient.HasInsurance = updatedPatient.HasInsurance;

        await _db.SaveChangesAsync();
        return Ok(patient);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> Delete(int id)
    {
        var patient = await _db.Patients.FirstOrDefaultAsync(p => p.Id == id);
        if (patient == null) return NotFound("Patient not found.");

        _db.Patients.Remove(patient);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("{id}/spravki")]
    [ProducesResponseType(typeof(List<Spravka>), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<List<Spravka>>> GetSpravki(int id)
    {
        var patientExists = await _db.Patients.AnyAsync(p => p.Id == id);
        if (!patientExists) return NotFound("Patient not found.");

        var patientSpravki = await _db.Spravki.Where(s => s.PatientId == id).ToListAsync();
        return Ok(patientSpravki);
    }
}