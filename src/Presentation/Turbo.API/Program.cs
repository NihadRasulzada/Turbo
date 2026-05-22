using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Turbo.Module.Catalog.Persistence.Contexts;
using Turbo.Module.Catalog.Persistence.Features.Car.Commands.Add;
using Turbo.Module.Media.DependencyInjection.Extensions;
using Turbo.Shared.Application.Abstraction;
using Turbo.Shared.Infrastructure.Implementations;
using Turbo.Shared.Infrastructure.Settings;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

// ── Options ──────────────────────────────────────────────────────────────────
builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMq"));

// ── Catalog ──────────────────────────────────────────────────────────────────
builder.Services.AddDbContext<CommandDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("CommandDb"))
);

builder.Services.AddDbContext<QueryDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("QueryDb"))
);

builder.Services.AddScoped<ICommandDispatcher, CommandDispatcher>();
builder.Services.AddScoped<IQueryDispatcher, QueryDispatcher>();

builder.Services.AddScoped<
    ICommandHandler<AddCarRequest, AppConc.Response<AddCarResponse>>,
    AddCarHandler
>();

builder.Services.AddScoped<IValidator<AddCarRequest>, AddCarValidator>();

// ── Media ────────────────────────────────────────────────────────────────────
builder.Services.AddMediaModule(builder.Configuration);

// ── MassTransit / RabbitMQ ───────────────────────────────────────────────────
var rabbit = builder.Configuration.GetSection("RabbitMq");

builder.Services.AddMassTransit(x =>
{
    x.AddMediaConsumers();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbit["Host"], rabbit["VirtualHost"], h =>
        {
            h.Username(rabbit["Username"] ?? string.Empty);
            h.Password(rabbit["Password"] ?? string.Empty);
        });

        cfg.ConfigureEndpoints(context);
    });
});

// ─────────────────────────────────────────────────────────────────────────────
var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
