using System.Security.Claims;

namespace BlazorApp.Auth;

public static class ClaimsPrincipalExtensions
{
    // Devuelve el Id del usuario logueado, o null si no hay nadie logueado
    public static int? GetUserId(this ClaimsPrincipal user)
    {
        if (user.Identity is null || !user.Identity.IsAuthenticated)
        {
            return null;
        }

        string? idAsString = user.FindFirst("Id")?.Value;
        return int.TryParse(idAsString, out int id) ? id : null;
    }
}
