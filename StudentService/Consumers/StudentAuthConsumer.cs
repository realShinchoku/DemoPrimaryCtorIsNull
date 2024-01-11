using AutoMapper;
using Contracts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using StudentService.Data;
using StudentService.Models;

namespace StudentService.Consumers;

public abstract class StudentAuthConsumer : IConsumer<StudentAuth>
{
    private readonly DataContext _dbContext;
    private readonly ILogger<StudentAuthConsumer> _logger;
    private readonly IMapper _mapper;

    protected StudentAuthConsumer(DataContext dbContext, IMapper mapper, ILogger<StudentAuthConsumer> logger)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<StudentAuth> context)
    {
        _logger.LogInformation("==> Consuming StudentAuth event");

        var student = await _dbContext.Students.FirstOrDefaultAsync(x => x.Id == context.Message.Id);
        if (student != null)
        {
            await context.RespondAsync(_mapper.Map<StudentAuth>(student));
            return;
        }

        student = _mapper.Map<Student>(context.Message);
        _dbContext.Add(student);

        var result = await _dbContext.SaveChangesAsync() > 0;

        await context.RespondAsync(_mapper.Map<StudentAuth>(student));

        if (!result)
            throw new Exception("dbSaveError");
    }
}