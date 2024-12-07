using TheBackfield.Models;

namespace TheBackfield.Endpoints;
public static class PenaltyEndpoints
{
    public static void MapPenaltyEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("").WithTags(nameof(Penalty));
    }
}
