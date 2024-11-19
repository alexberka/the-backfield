using TheBackfield.Models;

namespace TheBackfield.Endpoints;

public static class PlayerEndpoints
{
    public static void MapPlayerEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("").WithTags(nameof(Player));
    }
}