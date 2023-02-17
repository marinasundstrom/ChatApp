using System.Security.Claims;

namespace ChatApp.Services;

public interface ICurrentUserService
{
    string? UserId { get; }

    void SetUser(ClaimsPrincipal user);
}
