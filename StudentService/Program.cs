using ApplicationBase.Extensions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Polly;
using StudentService.Consumers;
using StudentService.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Logging.AddLoggingService(builder.Configuration);
builder.Services.AddApplicationService(builder.Configuration);
builder.Services.AddControllers();

builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddMassTransit(opts =>
{
    opts.AddEntityFrameworkOutbox<DataContext>(opt =>
    {
        opt.QueryDelay = TimeSpan.FromSeconds(10);
        opt.UseSqlServer();
        opt.UseBusOutbox();
    });

    opts.AddConsumersFromNamespaceContaining<StudentAuthConsumer>();

    opts.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("student", false));

    opts.UsingRabbitMq((context, cfg) =>
    {
        cfg.UseMessageRetry(r =>
        {
            r.Handle<RabbitMqConnectionException>();
            r.Interval(5, TimeSpan.FromSeconds(10));
        });

        cfg.Host(builder.Configuration["RabbitMQ:Host"], "/", host =>
        {
            host.Username(builder.Configuration.GetValue("RabbitMQ:Username", "guest"));
            host.Password(builder.Configuration.GetValue("RabbitMQ:Password", "guest"));
        });

        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapControllers();

var retryPolicy = Policy
    .Handle<Exception>()
    .WaitAndRetry(5, _ => TimeSpan.FromSeconds(10));

retryPolicy.ExecuteAndCapture(() => app.InitDb());

app.Run();