using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StudentService.Models;

[PrimaryKey(nameof(LessonId), nameof(StudentId))]
[Index(nameof(IsPresent))]
[Index(nameof(AttendedTime))]
public class Attendance
{
    public Guid LessonId { get; set; }
    public Guid StudentId { get; set; }
    [ForeignKey(nameof(LessonId))] public virtual Lesson Lesson { get; set; }
    [ForeignKey(nameof(StudentId))] public virtual Student Student { get; set; }
    public DateTime AttendedTime { get; set; }
    public bool IsPresent { get; set; }
}