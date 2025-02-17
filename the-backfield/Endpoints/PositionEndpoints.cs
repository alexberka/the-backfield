using TheBackfield.DTOs;
using TheBackfield.Interfaces;
using TheBackfield.Models;

namespace TheBackfield.Endpoints;

public static class PositionEndpoints
{
    public static void MapPositionEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("").WithTags(nameof(Position));

        group.MapGet("/positions", async (IPositionService positionService) =>
        {
            ResponseDTO<List<Position>> response = await positionService.GetPositionsAsync();
            if (response.ErrorMessage != null)
            {
                return response.ThrowError();
            }

            return Results.Ok(response.Resource);
        })
            .WithOpenApi()
            .Produces<List<Position>>(StatusCodes.Status200OK);
    }
}