using Microsoft.AspNetCore.Mvc;
using MedicalApi.Models;

namespace MedicalApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpravkaTypesController : ControllerBase
{
    public static List<SpravkaType> Types = new List<SpravkaType>
    {
        new SpravkaType { Id = 1, Name = "Medical Certificate 083", Price = 100, ValidityDays = 30, RequiresDoctorExam = true },
        new SpravkaType { Id = 2, Name = "Sanitary Booklet", Price = 750, ValidityDays = 365, RequiresDoctorExam = true },
        new SpravkaType { Id = 3, Name = "Pool Health Clearance", Price = 250, ValidityDays = 90, RequiresDoctorExam = false },
        new SpravkaType { Id = 4, Name = "Driver's License Certificate", Price = 1200, ValidityDays = 365, RequiresDoctorExam = true }
    };

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
    public ActionResult<List<SpravkaType>> GetAll()
    {
        return Ok(Types);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SpravkaType), 200)]
    [ProducesResponseType(404)]
    public ActionResult<SpravkaType> GetById(int id)
    {
        var spravkaType = Types.FirstOrDefault(t => t.Id == id);
        if (spravkaType == null) return NotFound("Certificate type not found.");
        return Ok(spravkaType);
    }

    [HttpPost]
    [ProducesResponseType(typeof(SpravkaType), 201)]
    [ProducesResponseType(400)]
    public ActionResult<SpravkaType> Create(SpravkaType spravkaType)
    {
        var error = ValidateType(spravkaType);
        if (error != null) return BadRequest(error);

        spravkaType.Id = Types.Any() ? Types.Max(t => t.Id) + 1 : 1;
        Types.Add(spravkaType);

        return CreatedAtAction(nameof(GetById), new { id = spravkaType.Id }, spravkaType);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(SpravkaType), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public ActionResult<SpravkaType> Update(int id, SpravkaType updatedType)
    {
        var spravkaType = Types.FirstOrDefault(t => t.Id == id);
        if (spravkaType == null) return NotFound("Certificate type not found.");

        var error = ValidateType(updatedType);
        if (error != null) return BadRequest(error);

        spravkaType.Name = updatedType.Name;
        spravkaType.Price = updatedType.Price;
        spravkaType.ValidityDays = updatedType.ValidityDays;
        spravkaType.RequiresDoctorExam = updatedType.RequiresDoctorExam;

        return Ok(spravkaType);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public ActionResult Delete(int id)
    {
        var spravkaType = Types.FirstOrDefault(t => t.Id == id);
        if (spravkaType == null) return NotFound("Certificate type not found.");

        Types.Remove(spravkaType);
        return NoContent();
    }
}