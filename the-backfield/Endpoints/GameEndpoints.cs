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
    }
}