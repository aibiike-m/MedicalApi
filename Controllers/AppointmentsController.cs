using Microsoft.AspNetCore.Mvc;
using MedicalApi.Models;

namespace MedicalApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private static List<Appointment> Appointments = new List<Appointment>
    {
        new Appointment { Id = 1, PatientId = 1, DoctorSpecialty = "General Practitioner", AppointmentTime = DateTime.Now.AddDays(1), IsConfirmed = true }
    };

    private string? ValidateAppointment(Appointment app)
    {
        if (string.IsNullOrWhiteSpace(app.DoctorSpecialty))
            return "Doctor specialty cannot be empty.";
        if (app.AppointmentTime < DateTime.Now)
            return "Appointment time cannot be in the past.";
        return null;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<Appointment>), 200)]
    public ActionResult<List<Appointment>> GetAll()
    {
        return Ok(Appointments);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Appointment), 200)]
    [ProducesResponseType(404)]
    public ActionResult<Appointment> GetById(int id)
    {
        var app = Appointments.FirstOrDefault(a => a.Id == id);
        if (app == null) return NotFound("Appointment not found.");
        return Ok(app);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Appointment), 201)]
    [ProducesResponseType(400)]
    public ActionResult<Appointment> Create(Appointment app)
    {
        var patientExists = PatientsController.Patients.Any(p => p.Id == app.PatientId);
        if (!patientExists)
            return BadRequest("Patient with specified ID does not exist.");

        var error = ValidateAppointment(app);
        if (error != null) return BadRequest(error);

        app.Id = Appointments.Any() ? Appointments.Max(a => a.Id) + 1 : 1;
        Appointments.Add(app);

        return CreatedAtAction(nameof(GetById), new { id = app.Id }, app);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Appointment), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public ActionResult<Appointment> Update(int id, Appointment updatedApp)
    {
        var app = Appointments.FirstOrDefault(a => a.Id == id);
        if (app == null) return NotFound("Appointment not found.");

        var patientExists = PatientsController.Patients.Any(p => p.Id == updatedApp.PatientId);
        if (!patientExists)
            return BadRequest("Patient with specified ID does not exist.");

        var error = ValidateAppointment(updatedApp);
        if (error != null) return BadRequest(error);

        app.PatientId = updatedApp.PatientId;
        app.DoctorSpecialty = updatedApp.DoctorSpecialty;
        app.AppointmentTime = updatedApp.AppointmentTime;
        app.IsConfirmed = updatedApp.IsConfirmed;

        return Ok(app);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public ActionResult Delete(int id)
    {
        var app = Appointments.FirstOrDefault(a => a.Id == id);
        if (app == null) return NotFound("Appointment not found.");

        Appointments.Remove(app);
        return NoContent();
    }
}