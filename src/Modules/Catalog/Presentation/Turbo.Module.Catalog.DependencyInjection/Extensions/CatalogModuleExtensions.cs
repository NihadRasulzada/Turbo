using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Catalog.DependencyInjection.Extensions;

public static class CatalogModuleExtensions
{
    public static IServiceCollection AddCatalogModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ── DbContexts ────────────────────────────────────────────────────────
        services.AddDbContext<CommandDbContext>(opt =>
            opt.UseNpgsql(configuration.GetConnectionString("CommandDb")));

        // QueryDbApp uses the read-only turbo_reader PostgreSQL user at runtime.
        // EF CLI migrations use QueryDb (admin) via QueryDbContextDesignTimeFactory.
        services.AddDbContext<QueryDbContext>(opt =>
            opt.UseNpgsql(configuration.GetConnectionString("QueryDbApp")));

        // ── DbContext interface aliases ────────────────────────────────────────
        services.AddScoped<ICatalogWriteDbContext>(sp => sp.GetRequiredService<CommandDbContext>());
        services.AddScoped<ICatalogReadDbContext>(sp => sp.GetRequiredService<QueryDbContext>());

        // ── Brand ─────────────────────────────────────────────────────────────
        services.AddScoped<
            IQueryHandler<GetAllBrandsRequest, AppConc.Response<IReadOnlyList<GetAllBrandsResponse>>>,
            GetAllBrandsHandler>();

        services.AddScoped<
            IQueryHandler<GetBrandByIdRequest, AppConc.Response<GetBrandByIdResponse>>,
            GetBrandByIdHandler>();

        services.AddScoped<
            ICommandHandler<CreateBrandRequest, AppConc.Response<CreateBrandResponse>>,
            CreateBrandHandler>();
        services.AddScoped<IValidator<CreateBrandRequest>, CreateBrandValidator>();

        services.AddScoped<
            ICommandHandler<UpdateBrandRequest, AppConc.Response<UpdateBrandResponse>>,
            UpdateBrandHandler>();
        services.AddScoped<IValidator<UpdateBrandRequest>, UpdateBrandValidator>();

        services.AddScoped<
            ICommandHandler<DeleteBrandRequest, AppConc.Response>,
            DeleteBrandHandler>();

        // ── Model ─────────────────────────────────────────────────────────────
        services.AddScoped<
            IQueryHandler<GetAllModelsRequest, AppConc.Response<IReadOnlyList<GetAllModelsResponse>>>,
            GetAllModelsHandler>();

        services.AddScoped<
            IQueryHandler<GetModelByIdRequest, AppConc.Response<GetModelByIdResponse>>,
            GetModelByIdHandler>();

        services.AddScoped<
            ICommandHandler<CreateModelRequest, AppConc.Response<CreateModelResponse>>,
            CreateModelHandler>();
        services.AddScoped<IValidator<CreateModelRequest>, CreateModelValidator>();

        services.AddScoped<
            ICommandHandler<UpdateModelRequest, AppConc.Response<UpdateModelResponse>>,
            UpdateModelHandler>();
        services.AddScoped<IValidator<UpdateModelRequest>, UpdateModelValidator>();

        services.AddScoped<
            ICommandHandler<DeleteModelRequest, AppConc.Response>,
            DeleteModelHandler>();

        // ── Cars commands ─────────────────────────────────────────────────────
        services.AddScoped<
            ICommandHandler<CreateDraftRequest, AppConc.Response<CreateDraftResponse>>,
            CreateDraftHandler>();

        services.AddScoped<
            ICommandHandler<SubmitDraftImagesRequest, AppConc.Response<SubmitDraftImagesResponse>>,
            SubmitDraftImagesHandler>();

        services.AddScoped<
            ICommandHandler<SubmitDraftDetailsRequest, AppConc.Response<SubmitDraftDetailsResponse>>,
            SubmitDraftDetailsHandler>();
        services.AddScoped<IValidator<SubmitDraftDetailsRequest>, SubmitDraftDetailsValidator>();

        services.AddScoped<
            ICommandHandler<SubmitDraftPricingRequest, AppConc.Response<SubmitDraftPricingResponse>>,
            SubmitDraftPricingHandler>();
        services.AddScoped<IValidator<SubmitDraftPricingRequest>, SubmitDraftPricingValidator>();

        // ── Cars queries ──────────────────────────────────────────────────────
        services.AddScoped<
            IQueryHandler<GetCarConfigRequest, AppConc.Response<GetCarConfigResponse>>,
            GetCarConfigHandler>();

        services.AddScoped<
            IQueryHandler<GetDraftRequest, AppConc.Response<GetDraftResponse>>,
            GetDraftHandler>();

        return services;
    }

    /// <summary>
    /// Catalog modulu üçün pending migration-ları hər iki DB-yə tətbiq edir.
    /// QueryDb (admin) istifadə edilir — QueryDbApp (read-only) yox.
    /// </summary>
    public static async Task MigrateCatalogAsync(this IServiceProvider services)
    {
        var config = services.GetRequiredService<IConfiguration>();
        var logger = services.GetRequiredService<ILogger<CommandDbContext>>();

        var commandConnStr = config.GetConnectionString("CommandDb")
            ?? throw new InvalidOperationException("ConnectionStrings:CommandDb tapılmadı.");
        var queryConnStr = config.GetConnectionString("QueryDb")
            ?? throw new InvalidOperationException("ConnectionStrings:QueryDb tapılmadı.");

        await using var commandCtx = new CommandDbContext(
            new DbContextOptionsBuilder<CommandDbContext>()
                .UseNpgsql(commandConnStr)
                .Options);
        logger.LogInformation("[Catalog] CommandDb migration tətbiq edilir...");
        await commandCtx.Database.MigrateAsync();

        await using var queryCtx = new QueryDbContext(
            new DbContextOptionsBuilder<QueryDbContext>()
                .UseNpgsql(queryConnStr)
                .Options);
        logger.LogInformation("[Catalog] QueryDb migration tətbiq edilir...");
        await queryCtx.Database.MigrateAsync();

        logger.LogInformation("[Catalog] Migration tamamlandı.");
    }
}
