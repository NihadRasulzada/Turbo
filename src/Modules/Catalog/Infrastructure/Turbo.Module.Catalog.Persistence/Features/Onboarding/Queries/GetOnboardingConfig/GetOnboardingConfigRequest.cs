using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Catalog.Persistence.Features.Onboarding.Queries.GetOnboardingConfig;

public sealed record GetOnboardingConfigRequest : IQuery<AppConc.Response<GetOnboardingConfigResponse>>;