using Microsoft.AspNetCore.Mvc;
using MedicalApi.Models;

namespace MedicalApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpravkiController : ControllerBase
{
    public static List<Spravka> Spravki = new List<Spravka>
    {
        new Spravka { Id = 1, PatientId = 1, SpravkaTypeId = 1, IssueDate = DateTime.Now.AddDays(-2) },
        new Spravka { Id = 2, PatientId = 2, SpravkaTypeId = 3, IssueDate = DateTime.Now.AddDays(-1) }
    };

    [HttpGet]
    [ProducesResponseType(typeof(List<Spravka>), 200)]
    public ActionResult<List<Spravka>> GetAll([FromQuery] int? patientId)
    {
        if (patientId.HasValue)
        {
            var filtered = Spravki.Where(s => s.PatientId == patientId.Value).ToList();
            return Ok(filtered);
        }
        return Ok(Spravki);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Spravka), 200)]
    [ProducesResponseType(404)]
    public ActionResult<Spravka> GetById(int id)
    {
        Spravka? spravka = Spravki.FirstOrDefault(s => s.Id == id);
        if (spravka == null) return NotFound("Certificate not found.");
        return Ok(spravka);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Spravka), 201)]
    [ProducesResponseType(400)]
    public ActionResult<Spravka> Create(Spravka spravka)
    {
        var patientExists = PatientsController.Patients.Any(p => p.Id == spravka.PatientId);
        if (!patientExists)
            return BadRequest("Specified patient does not exist.");

        var typeExists = SpravkaTypesController.Types.Any(t => t.Id == spravka.SpravkaTypeId);
        if (!typeExists)
            return BadRequest("Specified certificate type not found in catalog.");

        spravka.Id = Spravki.Any() ? Spravki.Max(s => s.Id) + 1 : 1;
        spravka.IssueDate = DateTime.Now;
        Spravki.Add(spravka);

        return CreatedAtAction(nameof(GetById), new { id = spravka.Id }, spravka);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Spravka), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public ActionResult<Spravka> Update(int id, Spravka updatedSpravka)
    {
        Spravka? spravka = Spravki.FirstOrDefault(s => s.Id == id);
        if (spravka == null) return NotFound("Certificate not found.");

        var patientExists = PatientsController.Patients.Any(p => p.Id == updatedSpravka.PatientId);
        if (!patientExists)
            return BadRequest("Specified patient does not exist.");

        var typeExists = SpravkaTypesController.Types.Any(t => t.Id == updatedSpravka.SpravkaTypeId);
        if (!typeExists)
            return BadRequest("Specified certificate type not found in catalog.");

        spravka.PatientId = updatedSpravka.PatientId;
        spravka.SpravkaTypeId = updatedSpravka.SpravkaTypeId;

        return Ok(spravka);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public ActionResult Delete(int id)
    {
        Spravka? spravka = Spravki.FirstOrDefault(s => s.Id == id);
        if (spravka == null) return NotFound("Certificate not found.");

        Spravki.Remove(spravka);
        return NoContent();
    }
}