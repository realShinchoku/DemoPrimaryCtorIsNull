using System.ComponentModel.DataAnnotations.Schema;

namespace StudentService.Models;

public class Lesson : BaseEntity
{
    public Guid SubjectId { get; set; }
    [ForeignKey(nameof(SubjectId))] public virtual Subject Subject { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public virtual ICollection<Attendance> Attendances { get; set; }
    public Dictionary<string, int> Wifi { get; set; }
    public Dictionary<string, int> Bluetooth { get; set; }
}