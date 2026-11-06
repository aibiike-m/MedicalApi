using Microsoft.AspNetCore.Mvc;
using MedicalApi.Models;

namespace MedicalApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    public static List<Patient> Patients = new List<Patient>
    {
        new Patient { Id = 1, Name = "Alice", BirthDate = new DateTime(2000, 5, 15), Phone = "+996555123456", HasInsurance = true },
        new Patient { Id = 2, Name = "Timur", BirthDate = new DateTime(1995, 8, 22), Phone = "+996700987654", HasInsurance = false }
    };

    private string? ValidatePatient(Patient patient)
    {
        if (string.IsNullOrWhiteSpace(patient.Name))
            return "Patient name cannot be empty.";
        if (patient.BirthDate > DateTime.Now)
            return "Birth date cannot be in the future.";
        if (string.IsNullOrWhiteSpace(patient.Phone))
            return "Phone number is required.";
        return null;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<Patient>), 200)]
    public ActionResult<List<Patient>> GetAll()
    {
        return Ok(Patients);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Patient), 200)]
    [ProducesResponseType(404)]
    public ActionResult<Patient> GetById(int id)
    {
        var patient = Patients.FirstOrDefault(p => p.Id == id);
        if (patient == null) return NotFound("Patient not found.");
        return Ok(patient);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Patient), 201)]
    [ProducesResponseType(400)]
    public ActionResult<Patient> Create(Patient patient)
    {
        var error = ValidatePatient(patient);
        if (error != null) return BadRequest(error);

        patient.Id = Patients.Any() ? Patients.Max(p => p.Id) + 1 : 1;
        Patients.Add(patient);

        return CreatedAtAction(nameof(GetById), new { id = patient.Id }, patient);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Patient), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public ActionResult<Patient> Update(int id, Patient updatedPatient)
    {
        var patient = Patients.FirstOrDefault(p => p.Id == id);
        if (patient == null) return NotFound("Patient not found.");

        var error = ValidatePatient(updatedPatient);
        if (error != null) return BadRequest(error);

        patient.Name = updatedPatient.Name;
        patient.BirthDate = updatedPatient.BirthDate;
        patient.Phone = updatedPatient.Phone;
        patient.HasInsurance = updatedPatient.HasInsurance;

        return Ok(patient);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public ActionResult Delete(int id)
    {
        var patient = Patients.FirstOrDefault(p => p.Id == id);
        if (patient == null) return NotFound("Patient not found.");

        Patients.Remove(patient);
        return NoContent();
    }

    [HttpGet("{id}/spravki")]
    [ProducesResponseType(typeof(List<Spravka>), 200)]
    [ProducesResponseType(404)]
    public ActionResult<List<Spravka>> GetSpravki(int id)
    {
        var patient = Patients.FirstOrDefault(p => p.Id == id);
        if (patient == null) return NotFound("Patient not found.");

        var patientSpravki = SpravkiController.Spravki.Where(s => s.PatientId == id).ToList();
        return Ok(patientSpravki);
    }
}