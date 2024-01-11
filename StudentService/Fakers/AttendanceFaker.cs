using Bogus;
using StudentService.Models;

namespace StudentService.Fakers;

public sealed class AttendanceFaker : Faker<Attendance>
{
    public AttendanceFaker(Lesson lesson, ICollection<Student> students)
    {
        RuleFor(x => x.Lesson, lesson);
        RuleFor(x => x.Student, f => f.PickRandom(students));
        RuleFor(x => x.IsPresent, f => f.Random.Bool());
        RuleFor(x => x.AttendedTime, f => f.Date.Future());
    }
}