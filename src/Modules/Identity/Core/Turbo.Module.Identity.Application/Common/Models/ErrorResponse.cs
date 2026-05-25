using System;
using System.Collections.Generic;
using System.Text;

namespace Turbo.Module.Identity.Application.Common.Models;

public sealed record ErrorResponse(
    string Code,
    string Message
);