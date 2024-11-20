using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using TheBackfield.DTOs;
using TheBackfield.Interfaces;
using TheBackfield.Models;
using TheBackfield.Utilities;

namespace TheBackfield.Endpoints;

public static class TeamEndpoints
{
    public static void MapTeamEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("").WithTags(nameof(Team));

        group.MapPost("/teams", async (ITeamService teamService, TeamSubmitDTO teamSubmit) =>
        {
            if (string.IsNullOrEmpty(teamSubmit.LocationName) && string.IsNullOrEmpty(teamSubmit.Nickname))
            {
                return Results.BadRequest("LocationName and/or Nickname must be defined.");
            }

            if ((!Regex.IsMatch(teamSubmit.ColorPrimaryHex, @"^[#][0-9A-Fa-f]{6}$") && teamSubmit.ColorPrimaryHex != "")
            || (!Regex.IsMatch(teamSubmit.ColorSecondaryHex, @"^[#][0-9A-Fa-f]{6}$") && teamSubmit.ColorSecondaryHex != ""))
            {
                return Results.BadRequest("Color values must be stored as 6 digit hexadecimals in format '#xxxxxx' (lettercasing indifferent).");
            }

            ResponseDTO createResponse = await teamService.CreateTeamAsync(teamSubmit);

            if (createResponse.Unauthorized)
            {
                return Results.Unauthorized();
            }

            return Results.Created($"/teams/{createResponse.ResourceId}", createResponse.Resource);
        })
            .WithOpenApi()
            .Produces<Team>(StatusCodes.Status201Created);

        group.MapPut("/teams/{teamId}", async (ITeamService teamService, int teamId, TeamSubmitDTO teamSubmit) =>
        {
            if (teamSubmit.Id != teamId)
            {
                return Results.BadRequest("Id in payload does not match teamId in URI.");
            }

            if (string.IsNullOrEmpty(teamSubmit.LocationName) && string.IsNullOrEmpty(teamSubmit.Nickname))
            {
                return Results.BadRequest("LocationName and/or Nickname must be defined.");
            }

            if ((!Regex.IsMatch(teamSubmit.ColorPrimaryHex, @"^[#][0-9A-Fa-f]{6}$") && teamSubmit.ColorPrimaryHex != "")
            || (!Regex.IsMatch(teamSubmit.ColorSecondaryHex, @"^[#][0-9A-Fa-f]{6}$") && teamSubmit.ColorSecondaryHex != ""))
            {
                return Results.BadRequest("Color values must be stored as 6 digit hexadecimals in format '#xxxxxx' (lettercasing indifferent).");
            }

            ResponseDTO updateResponse = await teamService.UpdateTeamAsync(teamSubmit);
            if (updateResponse.NotFound)
            {
                ResponseDTO createResponse = await teamService.CreateTeamAsync(teamSubmit);

                if (createResponse.Unauthorized)
                {
                    return Results.Unauthorized();
                }
                
                return Results.Created($"/teams/{createResponse.ResourceId}", createResponse.Resource);
            }

            if (updateResponse.Unauthorized)
            {
                return Results.Unauthorized();
            }

            if (updateResponse.Forbidden)
            {
                return Results.Forbid();
            }

            return Results.Ok(updateResponse.Resource);
        })
            .WithOpenApi()
            .Produces<Team>(StatusCodes.Status201Created);
    }
}