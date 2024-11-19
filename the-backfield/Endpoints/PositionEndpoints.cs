using TheBackfield.Models;

namespace TheBackfield.Endpoints;

public static class PositionEndpoints
{
    public static void MapPositionEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("").WithTags(nameof(Position));
    }
}