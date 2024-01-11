namespace StudentService.DTOs;

public class SubjectDto
{
    public Guid Id { get; set; }
    public Guid TeacherId { get; set; }
    public string Room { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public bool IsEnded { get; set; }
    public DateOnly DateStart { get; set; }
    public DateOnly DateEnd { get; set; }
}