using TheBackfield.DTOs.GameStream;
using TheBackfield.DTOs;
using TheBackfield.Interfaces;
using TheBackfield.Models;
using TheBackfield.Data;

namespace TheBackfield.Endpoints;

public static class GameEndpoints
{
    public static void MapGameEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("").WithTags(nameof(Game));

        group.MapGet("/games", async (IGameService gameService, string sessionKey) =>
        {
            ResponseDTO<List<Game>> response = await gameService.GetAllGamesAsync(sessionKey);
            if (response.ErrorMessage != null)
            {
                return response.ThrowError();
            }

            return Results.Ok(response.Resource);
        })
            .WithOpenApi()
            .Produces<List<Game>>(StatusCodes.Status200OK);

        group.MapGet("/games/{gameId}", async (IGameService gameService, int gameId, string sessionKey) =>
        {
            ResponseDTO<Game> response = await gameService.GetSingleGameAsync(gameId, sessionKey);
            if (response.ErrorMessage != null)
            {
                return response.ThrowError();
            }

            return Results.Ok(response.Resource);
        })
            .WithOpenApi()
            .Produces<Game>(StatusCodes.Status200OK);

        group.MapGet("/games/{gameId}/game-stream", async (IGameStreamService gameStreamService, int gameId) =>
        {
            GameStreamDTO? gameStream = await gameStreamService.GetGameStreamAsync(gameId);
            if (gameStream == null)
            {
                return Results.BadRequest("Invalid gameId");
            }

            return Results.Ok(gameStream);
        })
            .WithOpenApi()
            .Produces<Game>(StatusCodes.Status200OK);

        group.MapPost("/games", async (IGameService gameService, GameSubmitDTO gameSubmit) =>
        {
            ResponseDTO<Game> response = await gameService.CreateGameAsync(gameSubmit);
            if (response.Error || response.Resource == null)
            {
                return response.ThrowError();
            }

            return Results.Created($"/games/{response.Resource.Id}", response.Resource);
        })
            .WithOpenApi()
            .Produces<Game>(StatusCodes.Status201Created);

        group.MapPut("/games/{gameId}", async (IGameService gameService, int gameId, GameSubmitDTO gameSubmit) =>
        {
            if (gameId != gameSubmit.Id)
            {
                return Results.BadRequest("Id in payload must be the same as the gameId in the URI");
            }

            ResponseDTO<Game> response = await gameService.UpdateGameAsync(gameSubmit);
            if (response.ErrorMessage == "Invalid game id")
            {
                response = await gameService.CreateGameAsync(gameSubmit);
                if (response.Error || response.Resource == null)
                {
                    return response.ThrowError();
                }
                return Results.Created($"/games/{response.Resource.Id}", response.Resource);
            }

            if (response.Error || response.Resource == null)
            {
                return response.ThrowError();
            }

            return Results.Ok(response.Resource);
        })
            .WithOpenApi()
            .Produces<Game>(StatusCodes.Status200OK)
            .Produces<Game>(StatusCodes.Status201Created);

        group.MapDelete("/games/{gameId}", async (IGameService gameService, int gameId, string sessionKey) =>
        {
            ResponseDTO<Game> response = await gameService.DeleteGameAsync(gameId, sessionKey);
            if (response.Error)
            {
                return response.ThrowError();
            }

            return Results.NoContent();
        })
            .WithOpenApi()
            .Produces(StatusCodes.Status204NoContent);

        group.MapHub<WatchGame>($"watch");
    }
}