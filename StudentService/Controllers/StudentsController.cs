using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using StudentService.Data;
using StudentService.DTOs;

namespace StudentService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController(DataContext context, IMapper mapper, GraphServiceClient graphClient) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<StudentDto>>> GetAllStudents()
    {
        var query = context.Students
            .OrderBy(x => x.StudentCode).AsQueryable();

        return await query.ProjectTo<StudentDto>(mapper.ConfigurationProvider).ToListAsync();
    }

    // [Authorize]
    [HttpPut]
    // [AuthorizeForScopes(ScopeKeySection = "DownstreamApi:Scopes")]
    // [AuthorizeForScopes(ScopeKeySection = "AzureAd:Scopes")]
    public async Task<ActionResult<StudentDto>> Edit(StudentUpdateDto studentUpdateDto)
    {
        var user = await graphClient.Me.GetAsync(rq => rq.QueryParameters.Select =
            ["id", "mail", "mobilePhone"]);

        var id = Guid.Parse(user.Id);

        var student = await context.Students.FirstOrDefaultAsync(x => x.Id == id);
        if (student == null) return NotFound();

        mapper.Map(studentUpdateDto, student);

        var result = await context.SaveChangesAsync() > 0;

        if (!result) return BadRequest();

        return mapper.Map<StudentDto>(student);
    }
}