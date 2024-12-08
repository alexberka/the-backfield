using TheBackfield.DTOs;
using TheBackfield.Interfaces;
using TheBackfield.Models;

namespace TheBackfield.Endpoints;

public static class PlayEndpoints
{
    public static void MapPlayEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("").WithTags(nameof(Play));

        group.MapGet("/plays/{playId}", async (IPlayService playService, int playId, string sessionKey) =>
        {
            PlayResponseDTO response = await playService.GetSinglePlayAsync(playId, sessionKey);
            if (response.Error)
            {
                return response.ThrowError();
            };

            return Results.Ok(response.Play);
        })
            .WithOpenApi()
            .Produces<Play>(StatusCodes.Status200OK);
    }
}
