using MassTransit;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Turbo.Module.Catalog.DependencyInjection.Extensions;
using Turbo.Module.Media.DependencyInjection.Extensions;
using Turbo.Shared.Application.Abstraction;
using Turbo.Shared.Application.Pipeline;
using Turbo.Shared.Infrastructure.Implementations;
using Turbo.Shared.Infrastructure.Pipeline;
using Turbo.Shared.Infrastructure.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// ── OpenAPI / Scalar ──────────────────────────────────────────────────────────
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, ct) =>
    {
        document.Info = new OpenApiInfo
        {
            Title = "Turbo API",
            Version = "v1",
            Description = "REST API for the Turbo car marketplace platform."
        };
        return Task.CompletedTask;
    });
});

// ── Options ──────────────────────────────────────────────────────────────────
builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMq"));

// ── Dispatchers ───────────────────────────────────────────────────────────────
builder.Services.AddScoped<ICommandDispatcher, CommandDispatcher>();
builder.Services.AddScoped<IQueryDispatcher, QueryDispatcher>();

// ── Pipeline behaviors ────────────────────────────────────────────────────────
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));

// ── Modules ───────────────────────────────────────────────────────────────────
builder.Services.AddCatalogModule(builder.Configuration);
builder.Services.AddMediaModule(builder.Configuration);

// ── MassTransit / RabbitMQ ────────────────────────────────────────────────────
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

// ── Database migrations ───────────────────────────────────────────────────────
await app.Services.MigrateCatalogAsync();
await app.Services.MigrateMediaAsync();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "Turbo API";
        options.Theme = ScalarTheme.Default;
    });
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
