using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentService.Data;
using StudentService.Models;

namespace StudentService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LessonsController : ControllerBase
{
    private readonly DataContext _context;

    public LessonsController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<Lesson>>> GetAllLessons(string updatedAt)
    {
        var query = _context.Lessons
            .OrderBy(x => x.UpdatedAt)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(updatedAt))
            query = query.Where(x => x.UpdatedAt.CompareTo(DateTime.Parse(updatedAt).ToUniversalTime()) > 0);
        
        return await query.ToListAsync();
    }
}