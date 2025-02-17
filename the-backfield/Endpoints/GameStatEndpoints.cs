using TheBackfield.DTOs;
using TheBackfield.Interfaces;
using TheBackfield.Models;

namespace TheBackfield.Endpoints;

public static class GameStatEndpoints
{
    public static void MapGameStatEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("").WithTags(nameof(GameStat));

        group.MapPost("/game-stats", async (IGameStatService gameStatService, GameStatSubmitDTO gameStatSubmit) =>
        {
            ResponseDTO<GameStat> response = await gameStatService.CreateGameStatAsync(gameStatSubmit);
            if (response.Error || response.Resource == null)
            {
                return response.ThrowError();
            }

            return Results.Created($"/game-stats/{response.Resource.Id}", response.Resource);
        })
            .WithOpenApi()
            .Produces<GameStat>(StatusCodes.Status201Created);

        group.MapPut("/game-stats/{gameStatId}", async (IGameStatService gameStatService, int gameStatId, GameStatSubmitDTO gameStatSubmit) =>
        {
            if (gameStatId != gameStatSubmit.Id)
            {
                return Results.BadRequest("Id in payload must be the same as the gameStatId in the URI");
            }

            ResponseDTO<GameStat> response = await gameStatService.UpdateGameStatAsync(gameStatSubmit);
            if (response.Error || response.Resource == null)
            {
                return response.ThrowError();
            }

            return Results.Ok(response.Resource);
        })
            .WithOpenApi()
            .Produces<GameStat>(StatusCodes.Status200OK);

        group.MapDelete("/game-stats/{gameStatId}", async (IGameStatService gameStatService, int gameStatId, string sessionKey) =>
        {
            ResponseDTO<GameStat> response = await gameStatService.DeleteGameStatAsync(gameStatId, sessionKey);
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