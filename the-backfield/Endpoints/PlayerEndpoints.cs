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
            ResponseDTO<List<Player>> response = await playerService.GetPlayersAsync(sessionKey);

            if (response.ErrorMessage != null)
            {
                return response.ThrowError();
            }

            return Results.Ok(response.Resource);
        })
            .WithOpenApi()
            .Produces<List<Player>>(StatusCodes.Status200OK);

        group.MapGet("/players/{playerId}", async (IPlayerService playerService, int playerId, string sessionKey) =>
        {
            ResponseDTO<Player> response = await playerService.GetSinglePlayerAsync(playerId, sessionKey);
            if (response.ErrorMessage != null || response.Resource == null)
            {
                return response.ThrowError();
            }

            return Results.Ok(response.Resource);
        })
            .WithOpenApi()
            .Produces<Player>(StatusCodes.Status200OK);

        group.MapPost("/players", async (IPlayerService playerService, PlayerSubmitDTO playerSubmit) =>
        {
            ResponseDTO<Player> response = await playerService.CreatePlayerAsync(playerSubmit);
            if (response.ErrorMessage != null || response.Resource == null)
            {
                return response.ThrowError();
            }
            return Results.Created($"/players/{response.Resource.Id}", response.Resource);
        })
            .WithOpenApi()
            .Produces<Player>(StatusCodes.Status201Created);

        group.MapPut("/players/{playerId}", async (IPlayerService playerService, PlayerSubmitDTO playerSubmit, int playerId) =>
        {
            if (playerSubmit.Id != playerId)
            {
                return Results.BadRequest("Id in payload does not match playerId in URI");
            }

            ResponseDTO<Player> response = await playerService.UpdatePlayerAsync(playerSubmit);

            if (response.ErrorMessage == "Invalid player id")
            {
                response = await playerService.CreatePlayerAsync(playerSubmit);
                if (response.ErrorMessage != null || response.Resource == null)
                {
                    return response.ThrowError();
                }
                return Results.Created($"/players/{response.Resource.Id}", response.Resource);
            }

            if (response.ErrorMessage != null || response.Resource == null)
            {
                return response.ThrowError();
            }

            return Results.Ok(response.Resource);
        })
            .WithOpenApi()
            .Produces<Player>(StatusCodes.Status200OK)
            .Produces<Player>(StatusCodes.Status201Created);

        group.MapPost("/players/{playerId}/add-positions", async (IPlayerService playerService, PlayerPositionSubmitDTO playerPositionSubmit, int playerId) =>
        {
            if (playerPositionSubmit.PlayerId != playerId)
            {
                return Results.BadRequest("Id in payload does not match playerId in URI");
            }

            ResponseDTO<Player> response = await playerService.AddPlayerPositionsAsync(playerPositionSubmit);
            if (response.Error)
            {
                return response.ThrowError();
            }

            return Results.Ok(response.Resource);
        })
            .WithOpenApi()
            .Produces<Player>(StatusCodes.Status200OK);

        group.MapPost("/players/{playerId}/remove-positions", async (IPlayerService playerService, PlayerPositionSubmitDTO playerPositionSubmit, int playerId) =>
        {
            if (playerPositionSubmit.PlayerId != playerId)
            {
                return Results.BadRequest("Id in payload does not match playerId in URI");
            }

            ResponseDTO<Player> response = await playerService.RemovePlayerPositionsAsync(playerPositionSubmit);
            if (response.Error)
            {
                return response.ThrowError();
            }

            return Results.Ok(response.Resource);
        })
            .WithOpenApi()
            .Produces<Player>(StatusCodes.Status200OK);

        group.MapDelete("/players/{playerId}", async (IPlayerService playerService, int playerId, string sessionKey) =>
        {
            ResponseDTO<Player> response = await playerService.DeletePlayerAsync(playerId, sessionKey);
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