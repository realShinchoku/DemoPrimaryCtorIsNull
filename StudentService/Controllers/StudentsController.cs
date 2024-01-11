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
    private readonly DataContext _context = context;
    private readonly GraphServiceClient _graphClient = graphClient;
    private readonly IMapper _mapper = mapper;

    [HttpGet]
    public async Task<ActionResult<List<StudentDto>>> GetAllStudents()
    {
        var query = _context.Students
            .OrderBy(x => x.StudentCode).AsQueryable();

        return await query.ProjectTo<StudentDto>(_mapper.ConfigurationProvider).ToListAsync();
    }

    [Authorize]
    // [HttpPut]
    // [AuthorizeForScopes(ScopeKeySection = "DownstreamApi:Scopes")]
    // [AuthorizeForScopes(ScopeKeySection = "AzureAd:Scopes")]
    public async Task<ActionResult<StudentDto>> Edit(StudentUpdateDto studentUpdateDto)
    {
        var user = await _graphClient.Me.GetAsync(rq => rq.QueryParameters.Select =
            ["id", "mail", "mobilePhone"]);

        var id = Guid.Parse(user.Id);

        var student = await _context.Students.FirstOrDefaultAsync(x => x.Id == id);
        if (student == null) return NotFound();

        _mapper.Map(studentUpdateDto, student);

        var result = await _context.SaveChangesAsync() > 0;

        if (!result) return BadRequest();

        return _mapper.Map<StudentDto>(student);
    }
}