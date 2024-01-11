namespace StudentService.DTOs;

public class SubjectCreateDto
{
    public string Room { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public DateOnly DateStart { get; set; }
    public DateOnly DateEnd { get; set; }
    public ICollection<StudentCreateDto> Students { get; set; }
    public ICollection<LessonCreateDto> Lessons { get; set; }
}

public class LessonCreateDto
{
    public int DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}