using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Turbo.Module.Identity.Infrastructure.Options;

public sealed class EmailOptions
{
    public const string SectionName = "Email";

    [Required] public string From { get; init; } = string.Empty;
    [Required] public string SmtpHost { get; init; } = string.Empty;
    [Range(1, 65535)] public int SmtpPort { get; init; } = 587;
    [Required] public string Username { get; init; } = string.Empty;
    [Required] public string Password { get; init; } = string.Empty;
}