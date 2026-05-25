using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Turbo.Module.Catalog.Persistence.Contexts;
using Turbo.Module.Catalog.Persistence.Features.Brand.Commands.CreateBrand;
using Turbo.Module.Catalog.Persistence.Features.Brand.Commands.DeleteBrand;
using Turbo.Module.Catalog.Persistence.Features.Brand.Commands.UpdateBrand;
using Turbo.Module.Catalog.Persistence.Features.Brand.Queries.GetAllBrands;
using Turbo.Module.Catalog.Persistence.Features.Brand.Queries.GetBrandById;
using Turbo.Module.Catalog.Persistence.Features.Model.Commands.CreateModel;
using Turbo.Module.Catalog.Persistence.Features.Model.Commands.DeleteModel;
using Turbo.Module.Catalog.Persistence.Features.Model.Commands.UpdateModel;
using Turbo.Module.Catalog.Persistence.Features.Model.Queries.GetAllModels;
using Turbo.Module.Catalog.Persistence.Features.Model.Queries.GetModelById;
using Turbo.Module.Catalog.Persistence.Features.Cars.Commands.CreateDraft;
using Turbo.Module.Catalog.Persistence.Features.Cars.Commands.SubmitDraftDetails;
using Turbo.Module.Catalog.Persistence.Features.Cars.Commands.SubmitDraftImages;
using Turbo.Module.Catalog.Persistence.Features.Cars.Commands.SubmitDraftPricing;
using Turbo.Module.Catalog.Persistence.Features.Cars.Queries.GetDraft;
using Turbo.Module.Catalog.Persistence.Features.Cars.Queries.GetCarConfig;
using Turbo.Module.Media.DependencyInjection.Extensions;
using Turbo.Shared.Application.Abstraction;
using Turbo.Shared.Application.Pipeline;
using Turbo.Shared.Infrastructure.Implementations;
using Turbo.Shared.Infrastructure.Pipeline;
using Turbo.Shared.Infrastructure.Settings;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

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

// ── Catalog DbContexts ────────────────────────────────────────────────────────
builder.Services.AddDbContext<CommandDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("CommandDb"))
);

builder.Services.AddDbContext<QueryDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("QueryDb"))
);

// ── Brand ─────────────────────────────────────────────────────────────────────
builder.Services.AddScoped<
    IQueryHandler<GetAllBrandsRequest, AppConc.Response<IReadOnlyList<GetAllBrandsResponse>>>,
    GetAllBrandsHandler>();

builder.Services.AddScoped<
    IQueryHandler<GetBrandByIdRequest, AppConc.Response<GetBrandByIdResponse>>,
    GetBrandByIdHandler>();

builder.Services.AddScoped<
    ICommandHandler<CreateBrandRequest, AppConc.Response<CreateBrandResponse>>,
    CreateBrandHandler>();
builder.Services.AddScoped<IValidator<CreateBrandRequest>, CreateBrandValidator>();

builder.Services.AddScoped<
    ICommandHandler<UpdateBrandRequest, AppConc.Response<UpdateBrandResponse>>,
    UpdateBrandHandler>();
builder.Services.AddScoped<IValidator<UpdateBrandRequest>, UpdateBrandValidator>();

builder.Services.AddScoped<
    ICommandHandler<DeleteBrandRequest, AppConc.Response>,
    DeleteBrandHandler>();

// ── Model ─────────────────────────────────────────────────────────────────────
builder.Services.AddScoped<
    IQueryHandler<GetAllModelsRequest, AppConc.Response<IReadOnlyList<GetAllModelsResponse>>>,
    GetAllModelsHandler>();

builder.Services.AddScoped<
    IQueryHandler<GetModelByIdRequest, AppConc.Response<GetModelByIdResponse>>,
    GetModelByIdHandler>();

builder.Services.AddScoped<
    ICommandHandler<CreateModelRequest, AppConc.Response<CreateModelResponse>>,
    CreateModelHandler>();
builder.Services.AddScoped<IValidator<CreateModelRequest>, CreateModelValidator>();

builder.Services.AddScoped<
    ICommandHandler<UpdateModelRequest, AppConc.Response<UpdateModelResponse>>,
    UpdateModelHandler>();
builder.Services.AddScoped<IValidator<UpdateModelRequest>, UpdateModelValidator>();

builder.Services.AddScoped<
    ICommandHandler<DeleteModelRequest, AppConc.Response>,
    DeleteModelHandler>();

// ── Cars commands ─────────────────────────────────────────────────────────────
builder.Services.AddScoped<
    ICommandHandler<CreateDraftRequest, AppConc.Response<CreateDraftResponse>>,
    CreateDraftHandler>();

builder.Services.AddScoped<
    ICommandHandler<SubmitDraftImagesRequest, AppConc.Response<SubmitDraftImagesResponse>>,
    SubmitDraftImagesHandler>();

builder.Services.AddScoped<
    ICommandHandler<SubmitDraftDetailsRequest, AppConc.Response<SubmitDraftDetailsResponse>>,
    SubmitDraftDetailsHandler>();
builder.Services.AddScoped<IValidator<SubmitDraftDetailsRequest>, SubmitDraftDetailsValidator>();

builder.Services.AddScoped<
    ICommandHandler<SubmitDraftPricingRequest, AppConc.Response<SubmitDraftPricingResponse>>,
    SubmitDraftPricingHandler>();
builder.Services.AddScoped<IValidator<SubmitDraftPricingRequest>, SubmitDraftPricingValidator>();

// ── Cars queries ──────────────────────────────────────────────────────────────
builder.Services.AddScoped<
    IQueryHandler<GetCarConfigRequest, AppConc.Response<GetCarConfigResponse>>,
    GetCarConfigHandler>();

builder.Services.AddScoped<
    IQueryHandler<GetDraftRequest, AppConc.Response<GetDraftResponse>>,
    GetDraftHandler>();

// ── Media ─────────────────────────────────────────────────────────────────────
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
