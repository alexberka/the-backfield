using TheBackfield.DTOs;
using TheBackfield.DTOs.GameStream;
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

        group.MapPost("/plays", async (IPlayService playService, PlaySubmitDTO playSubmit) =>
        {
            PlayResponseDTO response = await playService.CreatePlayAsync(playSubmit);
            if (response.Error || response.Play == null)
            {
                return response.ThrowError();
            }

            return Results.Created($"/plays/{response.Play.Id}", response.Play);
        })
            .WithOpenApi()
            .Produces<Play>(StatusCodes.Status201Created);

        group.MapDelete("/plays/{playId}", async (IPlayService playService, int playId, string sessionKey) =>
        {
            PlayResponseDTO response = await playService.DeletePlayAsync(playId, sessionKey);
            if (response.Error)
            {
                return response.ThrowError();
            }

            return Results.NoContent();
        })
            .WithOpenApi()
            .Produces(StatusCodes.Status204NoContent);
    }
}
