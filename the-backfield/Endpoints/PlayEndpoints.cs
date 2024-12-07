using TheBackfield.Models;

namespace TheBackfield.Endpoints;

public static class PlayEndpoints
{
    public static void MapPlayEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("").WithTags(nameof(Play));
    }
}
