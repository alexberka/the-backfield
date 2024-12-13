using TheBackfield.DTOs.GameStream;
using TheBackfield.DTOs;
using TheBackfield.Interfaces;
using TheBackfield.Models;

namespace TheBackfield.Endpoints;

public static class GameEndpoints
{
    public static void MapGameEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("").WithTags(nameof(Game));

        group.MapGet("/games", async (IGameService gameService, string sessionKey) =>
        {
            GameResponseDTO response = await gameService.GetAllGamesAsync(sessionKey);
            if (response.ErrorMessage != null)
            {
                return response.ThrowError();
            }

            return Results.Ok(response.Games);
        })
            .WithOpenApi()
            .Produces<List<Game>>(StatusCodes.Status200OK);

        group.MapGet("/games/{gameId}", async (IGameService gameService, int gameId, string sessionKey) =>
        {
            GameResponseDTO response = await gameService.GetSingleGameAsync(gameId, sessionKey);
            if (response.ErrorMessage != null)
            {
                return response.ThrowError();
            }

            return Results.Ok(response.Game);
        })
            .WithOpenApi()
            .Produces<Game>(StatusCodes.Status200OK);

        group.MapGet("/games/{gameId}/game-stream", async (IGameService gameService, int gameId) =>
        {
            GameStreamDTO? gameStream = await gameService.GetGameStreamAsync(gameId);
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
            GameResponseDTO response = await gameService.CreateGameAsync(gameSubmit);
            if (response.Error || response.Game == null)
            {
                return response.ThrowError();
            }

            return Results.Created($"/games/{response.Game.Id}", response.Game);
        })
            .WithOpenApi()
            .Produces<Game>(StatusCodes.Status201Created);

        group.MapPut("/games/{gameId}", async (IGameService gameService, int gameId, GameSubmitDTO gameSubmit) =>
        {
            if (gameId != gameSubmit.Id)
            {
                return Results.BadRequest("Id in payload must be the same as the gameId in the URI");
            }

            GameResponseDTO response = await gameService.UpdateGameAsync(gameSubmit);
            if (response.ErrorMessage == "Invalid game id")
            {
                response = await gameService.CreateGameAsync(gameSubmit);
                if (response.Error || response.Game == null)
                {
                    return response.ThrowError();
                }
                return Results.Created($"/games/{response.Game.Id}", response.Game);
            }

            if (response.Error || response.Game == null)
            {
                return response.ThrowError();
            }

            return Results.Ok(response.Game);
        })
            .WithOpenApi()
            .Produces<Game>(StatusCodes.Status200OK)
            .Produces<Game>(StatusCodes.Status201Created);

        group.MapDelete("/games/{gameId}", async (IGameService gameService, int gameId, string sessionKey) =>
        {
            GameResponseDTO response = await gameService.DeleteGameAsync(gameId, sessionKey);
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