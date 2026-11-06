namespace MedicalApi.Models;

public class Appointment
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public string DoctorSpecialty { get; set; } = "";
    public DateTime AppointmentTime { get; set; }
    public bool IsConfirmed { get; set; }
}