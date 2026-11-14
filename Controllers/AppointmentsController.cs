using Microsoft.AspNetCore.Mvc;
using MedicalApi.Models;
using MedicalApi.Data;
using Microsoft.EntityFrameworkCore;

namespace MedicalApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly MedicalDbContext _db;

    public AppointmentsController(MedicalDbContext db)
    {
        _db = db;
    }

    private string? ValidateAppointment(Appointment app)
    {
        if (string.IsNullOrWhiteSpace(app.DoctorSpecialty))
            return "Doctor specialty cannot be empty.";
        if (app.AppointmentTime < DateTime.UtcNow)
            return "Appointment time cannot be in the past.";
        return null;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<Appointment>), 200)]
    public async Task<ActionResult<List<Appointment>>> GetAll()
    {
        return Ok(await _db.Appointments.ToListAsync());
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Appointment), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<Appointment>> GetById(int id)
    {
        var app = await _db.Appointments.FirstOrDefaultAsync(a => a.Id == id);
        if (app == null) return NotFound("Appointment not found.");
        return Ok(app);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Appointment), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<Appointment>> Create(Appointment app)
    {
        var patientExists = await _db.Patients.AnyAsync(p => p.Id == app.PatientId);
        if (!patientExists)
            return BadRequest("Patient with specified ID does not exist.");

        var error = ValidateAppointment(app);
        if (error != null) return BadRequest(error);

        _db.Appointments.Add(app);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = app.Id }, app);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Appointment), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<Appointment>> Update(int id, Appointment updatedApp)
    {
        var app = await _db.Appointments.FirstOrDefaultAsync(a => a.Id == id);
        if (app == null) return NotFound("Appointment not found.");

        var patientExists = await _db.Patients.AnyAsync(p => p.Id == updatedApp.PatientId);
        if (!patientExists)
            return BadRequest("Patient with specified ID does not exist.");

        var error = ValidateAppointment(updatedApp);
        if (error != null) return BadRequest(error);

        app.PatientId = updatedApp.PatientId;
        app.DoctorSpecialty = updatedApp.DoctorSpecialty;
        app.AppointmentTime = updatedApp.AppointmentTime;
        app.IsConfirmed = updatedApp.IsConfirmed;

        await _db.SaveChangesAsync();
        return Ok(app);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> Delete(int id)
    {
        var app = await _db.Appointments.FirstOrDefaultAsync(a => a.Id == id);
        if (app == null) return NotFound("Appointment not found.");

        _db.Appointments.Remove(app);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}