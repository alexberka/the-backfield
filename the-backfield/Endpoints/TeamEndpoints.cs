using TheBackfield.Models;

namespace TheBackfield.Endpoints;

public static class TeamEndpoints
{
    public static void MapTeamEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("").WithTags(nameof(Team));
    }
}