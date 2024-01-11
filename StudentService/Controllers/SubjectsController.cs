using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using StudentService.Data;
using StudentService.DTOs;
using StudentService.Models;

namespace StudentService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubjectsController(DataContext context, IMapper mapper) : ControllerBase
{
    [HttpGet("{room}")]
    // [Authorize]
    // [AuthorizeForScopes(ScopeKeySection = "DownstreamApi:Scopes")]
    // [AuthorizeForScopes(ScopeKeySection = "AzureAd:Scopes")]
    public async Task<ActionResult<SubjectDto>> GetByRoom(string room)
    {
        var userId = Guid.Parse(User.GetObjectId());

        if (!await context.Students.AnyAsync(x => x.Id == userId))
            return NotFound("Student not found");

        var dateTimeNow = DateTime.UtcNow;
        var dateNow = DateOnly.FromDateTime(dateTimeNow);

        var subject = await context.Subjects
            .AsSplitQuery()
            .Include(x => x.Students)
            .Where(x => x.Students.Any(s => s.Id == userId))
            .Include(x => x.Lessons)
            .Where(x => x.Lessons.Any(l => l.StartTime <= dateTimeNow && dateTimeNow <= l.EndTime))
            .ProjectTo<SubjectDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(x =>
                x.Room.ToLower().Contains(room.ToLower())
                && x.DateStart <= dateNow && dateNow <= x.DateEnd
                && !x.IsEnded
            );

        if (subject == null) return NotFound("Subject not found");

        return subject;
    }

    [HttpPost]
    [Authorize]
    [AuthorizeForScopes(ScopeKeySection = "DownstreamApi:Scopes")]
    [AuthorizeForScopes(ScopeKeySection = "AzureAd:Scopes")]
    public async Task<ActionResult<SubjectDto>> Create(SubjectCreateDto subjectCreateDto)
    {
        var userId = Guid.Parse(User.GetObjectId());

        if (!await context.Teachers.AnyAsync(x => x.Id == userId))
            return NotFound("Teacher not found");

        var subject = mapper.Map<Subject>(subjectCreateDto);
        subject.Students.Add(new Student { Id = userId });

        await context.Subjects.AddAsync(subject);

        var result = await context.SaveChangesAsync() > 0;

        if (!result) return BadRequest();

        return mapper.Map<SubjectDto>(subject);
    }
}