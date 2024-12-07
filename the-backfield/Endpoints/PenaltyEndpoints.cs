using TheBackfield.DTOs;
using TheBackfield.Interfaces;
using TheBackfield.Models;

namespace TheBackfield.Endpoints;
public static class PenaltyEndpoints
{
    public static void MapPenaltyEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("").WithTags(nameof(Penalty));

        group.MapGet("/penalties", async (IPenaltyService penaltyService, string sessionKey) =>
        {
            PenaltyResponseDTO response = await penaltyService.GetAllPenaltiesAsync(sessionKey);
            if (response.Error)
            {
                return response.ThrowError();
            }

            return Results.Ok(response.Penalties);
        })
            .WithOpenApi()
            .Produces<List<Penalty>>(StatusCodes.Status200OK);
    }
}
