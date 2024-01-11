using Bogus;
using StudentService.Models;

namespace StudentService.Fakers;

public sealed class LessonFaker : Faker<Lesson>
{
    public LessonFaker()
    {
        RuleFor(x => x.StartTime, f => f.Date.Future());
        RuleFor(x => x.EndTime, f => f.Date.Future());
    }
}