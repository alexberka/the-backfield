using TheBackfield.Models;

namespace TheBackfield.Endpoints;

public static class GameStatEndpoints
{
    public static void MapGameStatEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("").WithTags(nameof(GameStat));
    }
}