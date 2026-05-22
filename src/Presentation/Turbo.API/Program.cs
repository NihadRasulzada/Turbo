using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Turbo.Module.Catalog.Persistence.Contexts;
using Turbo.Module.Catalog.Persistence.Features.Car.Commands.Add;
using Turbo.Shared.Application.Abstraction;
using Turbo.Shared.Application.ResponseObject.Concreate;
using Turbo.Shared.Infrastructure.Implementations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddDbContext<CommandDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("CatalogCommand"))
);

builder.Services.AddDbContext<QueryDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("CatalogQuery"))
);

builder.Services.AddScoped<ICommandDispatcher, CommandDispatcher>();
builder.Services.AddScoped<IQueryDispatcher, QueryDispatcher>();

builder.Services.AddScoped<
    ICommandHandler<AddCarRequest, Response<AddCarResponse>>,
    AddCarHandler
>();

builder.Services.AddScoped<IValidator<AddCarRequest>, AddCarValidator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
