using System;
using System.Collections.Generic;
using System.Text;
using Turbo.Module.Identity.Domain.Entity;

namespace Turbo.Module.Identity.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}