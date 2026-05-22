namespace Turbo.API.Controllers.Requests;

/// <summary>Pricing and description for step 3.</summary>
public sealed class SubmitDraftPricingHttpRequest
{
    /// <example>25000</example>
    public int Price { get; set; }
    /// <example>Well-maintained, single owner, full service history.</example>
    public string Description { get; set; } = string.Empty;
}