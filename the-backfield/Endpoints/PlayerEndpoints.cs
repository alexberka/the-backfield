using TheBackfield.DTOs;
using TheBackfield.Interfaces;
using TheBackfield.Models;

namespace TheBackfield.Endpoints;

public static class PlayerEndpoints
{
    public static void MapPlayerEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("").WithTags(nameof(Player));

        group.MapPost("/players", async (IPlayerService playerService, PlayerSubmitDTO playerSubmit) =>
        {
            PlayerResponseDTO response = await playerService.CreatePlayerAsync(playerSubmit);
            if (response.ErrorMessage != null || response.Player == null)
            {
                return response.ParseError();
            }
            return Results.Created($"/players/{response.Player.Id}", response.Player);
        })
            .WithOpenApi()
            .Produces<Player>(StatusCodes.Status201Created);
    }
}