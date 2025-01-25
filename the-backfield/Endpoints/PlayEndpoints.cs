using System.Net.Http;
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

        group.MapPost("/plays", async (IPlayService playService, PlaySubmitDTO playSubmit) =>
        {
            ResponseDTO<Play> response = await playService.CreatePlayAsync(playSubmit);
            if (response.Error || response.Resource == null)
            {
                return response.ThrowError();
            }

            return Results.Created($"/plays/{response.Resource.Id}", response.Resource);
        })
            .WithOpenApi()
            .Produces<Play>(StatusCodes.Status201Created);

        group.MapPut("/plays/{playId}", async (IPlayService playService, PlaySubmitDTO playSubmit, int playId) =>
        {
            if (playSubmit.Id != playId)
            {
                return Results.BadRequest("Id in payload does not match playId in URI");
            }

            ResponseDTO<Play> response = await playService.UpdatePlayAsync(playSubmit);
            if (response.Error || response.Resource == null)
            {
                return response.ThrowError();
            }

            return Results.Ok(response.Resource);
        })
            .WithOpenApi()
            .Produces<Play>(StatusCodes.Status201Created);

        group.MapDelete("/plays/{playId}", async (IPlayService playService, int playId, string sessionKey) =>
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
        //group.MapGet("/play-segments/{playId}", async (IGameStreamService gameStreamService, int playId) =>
        //{
        //    List<PlaySegmentDTO> response = await gameStreamService.GetPlaySegmentsAsync(playId);

        //    return Results.Ok(response);
        //})
        //    .WithOpenApi()
        //    .Produces<Play>(StatusCodes.Status200OK);
    }
}
