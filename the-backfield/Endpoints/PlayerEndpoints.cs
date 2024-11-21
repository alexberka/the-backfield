using TheBackfield.DTOs;
using TheBackfield.Interfaces;
using TheBackfield.Models;

namespace TheBackfield.Endpoints;

public static class PlayerEndpoints
{
    public static void MapPlayerEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("").WithTags(nameof(Player));

        group.MapGet("/players", async (IPlayerService playerService, string sessionKey) =>
        {
            PlayerResponseDTO response = await playerService.GetPlayersAsync(sessionKey);

            if (response.ErrorMessage != null)
            {
                return response.ThrowError();
            }

            return Results.Ok(response.Players);
        })
            .WithOpenApi()
            .Produces<List<Player>>(StatusCodes.Status200OK);

        group.MapGet("/players/{playerId}", async (IPlayerService playerService, int playerId, string sessionKey) =>
        {
            PlayerResponseDTO response = await playerService.GetSinglePlayerAsync(playerId, sessionKey);
            if (response.ErrorMessage != null || response.Player == null)
            {
                return response.ThrowError();
            }

            return Results.Ok(response.Player);
        })
            .WithOpenApi()
            .Produces<Player>(StatusCodes.Status200OK);

        group.MapPost("/players", async (IPlayerService playerService, PlayerSubmitDTO playerSubmit) =>
        {
            PlayerResponseDTO response = await playerService.CreatePlayerAsync(playerSubmit);
            if (response.ErrorMessage != null || response.Player == null)
            {
                return response.ThrowError();
            }
            return Results.Created($"/players/{response.Player.Id}", response.Player);
        })
            .WithOpenApi()
            .Produces<Player>(StatusCodes.Status201Created);

        group.MapPut("/players/{playerId}", async (IPlayerService playerService, PlayerSubmitDTO playerSubmit, int playerId) =>
        {
            if (playerSubmit.Id != playerId)
            {
                return Results.BadRequest("Id in payload does not match playerId in URI");
            }

            PlayerResponseDTO response = await playerService.UpdatePlayerAsync(playerSubmit);

            if (response.ErrorMessage == "Invalid player id")
            {
                response = await playerService.CreatePlayerAsync(playerSubmit);
                if (response.ErrorMessage != null || response.Player == null)
                {
                    return response.ThrowError();
                }
                return Results.Created($"/players/{response.Player.Id}", response.Player);
            }

            if (response.ErrorMessage != null || response.Player == null)
            {
                return response.ThrowError();
            }

            return Results.Ok(response.Player);
        })
            .WithOpenApi()
            .Produces<Player>(StatusCodes.Status200OK)
            .Produces<Player>(StatusCodes.Status201Created);
    }
}