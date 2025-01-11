using Microsoft.AspNetCore.SignalR;
using TheBackfield.Data;
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
            ResponseDTO<Play> response = await playService.GetSinglePlayAsync(playId, sessionKey);
            if (response.Error)
            {
                return response.ThrowError();
            };

            return Results.Ok(response.Resource);
        })
            .WithOpenApi()
            .Produces<Play>(StatusCodes.Status200OK);

        group.MapPost("/plays", async (IPlayService playService, IGameService gameService, IHubContext <WatchGame, IWatchClient> streamContext, PlaySubmitDTO playSubmit) =>
        {
            ResponseDTO<Play> response = await playService.CreatePlayAsync(playSubmit);
            if (response.Error || response.Resource == null)
            {
                return response.ThrowError();
            }

            GameStreamDTO? gameStream = await gameService.GetGameStreamAsync(playSubmit.GameId);
            if (gameStream != null)
            {
                await streamContext.Clients.Groups($"watch-{playSubmit.GameId}").UpdateGameStream(gameStream);
            }


            return Results.Created($"/plays/{response.Resource.Id}", response.Resource);
        })
            .WithOpenApi()
            .Produces<Play>(StatusCodes.Status201Created);

        group.MapDelete("/plays/{playId}", async (IPlayService playService, IGameService gameService, int playId, string sessionKey) =>
        {
            ResponseDTO<Play> response = await playService.DeletePlayAsync(playId, sessionKey);
            if (response.Error)
            {
                return response.ThrowError();
            }

            return Results.NoContent();
        })
            .WithOpenApi()
            .Produces(StatusCodes.Status204NoContent);

        // For PlaySegment testing
        // v---------------------v
        //group.MapGet("/play-segments/{playId}", async (IPlayService playService, int playId) =>
        //{
        //    List<PlaySegmentDTO> response = await playService.GetPlaySegmentsAsync(playId);

        //    return Results.Ok(response);
        //})
        //    .WithOpenApi()
        //    .Produces<Play>(StatusCodes.Status200OK);
    }
}
