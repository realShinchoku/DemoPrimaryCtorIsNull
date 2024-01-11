using Microsoft.EntityFrameworkCore;
using StudentService.Fakers;

namespace StudentService.Data;

public static class DbInitializer
{
    public static async Task InitDb(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        await SeedData(scope.ServiceProvider.GetService<DataContext>());
    }

    private static async Task SeedData(DataContext context)
    {
        await context.Database.MigrateAsync();

        if (context.Teachers.Any()) return;
        var teachers = new TeacherFaker().Generate(30);
        await context.BulkInsertAsync(teachers, opt => opt.IncludeGraph = true);
        var students = new StudentFaker().Generate(10000);
        await context.BulkInsertAsync(students);
        foreach (var lesson in from teacher in teachers
                 from subject in teacher.Subjects
                 from lesson in subject.Lessons
                 select lesson)
            await context.BulkInsertAsync(new AttendanceFaker(lesson, students).Generate(5),
                opt => opt.AllowDuplicateKeys = false);

        await context.BulkSaveChangesAsync();
    }
}