using System.Security.Claims;

namespace Attendances.Shared.Commons.Helpers;

public static class IdentityHelper
{
    public static Guid? GetAccountUuid(this ClaimsPrincipal principal)
    {
        var uuid = principal.FindFirstValue(ClaimTypes.PrimarySid);
        return Guid.TryParse(uuid, out var result) ? result : null;
    }
    
    public static long? GetUserId(this ClaimsPrincipal principal)
    {
        var uuid = principal.FindFirstValue(ClaimTypes.Sid);
        return long.TryParse(uuid, out var result) ? result : null;
    }
    
    public static string? GetUsername(this ClaimsPrincipal principal) => principal.FindFirstValue(ClaimTypes.Name);
}