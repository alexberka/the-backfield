using System.Text.RegularExpressions;
using System.Threading.Tasks.Dataflow;
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

        group.MapGet("/teams", async (ITeamService teamService, string sessionKey) =>
        {
            TeamResponseDTO readResponse = await teamService.GetTeamsBySessionKeyAsync(sessionKey);
            if (readResponse.Unauthorized)
            {
                return Results.Unauthorized();
            }
            return Results.Ok(readResponse.Teams);
        })
            .WithOpenApi()
            .Produces<List<Team>>(StatusCodes.Status200OK);

        group.MapGet("/teams/{teamId}", async (ITeamService teamService, int teamId, string sessionKey) =>
        {
            TeamResponseDTO response = await teamService.GetSingleTeamAsync(teamId, sessionKey);
            if (response.NotFound)
            {
                return Results.NotFound(response.ErrorMessage);
            }

            if (response.Unauthorized)
            {
                return Results.Unauthorized();
            }

            if (response.Forbidden)
            {
                return Results.StatusCode(403);
            }

            return Results.Ok(response.Team);
        });

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

            TeamResponseDTO createResponse = await teamService.CreateTeamAsync(teamSubmit);

            if (createResponse.Unauthorized)
            {
                return Results.Unauthorized();
            }

            return Results.Created($"/teams/{createResponse.Team.Id}", createResponse.Team);
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

            TeamResponseDTO updateResponse = await teamService.UpdateTeamAsync(teamSubmit);
            if (updateResponse.NotFound)
            {
                TeamResponseDTO createResponse = await teamService.CreateTeamAsync(teamSubmit);

                if (createResponse.Unauthorized)
                {
                    return Results.Unauthorized();
                }
                
                return Results.Created($"/teams/{createResponse.Team.Id}", createResponse.Team);
            }

            if (updateResponse.Unauthorized)
            {
                return Results.Unauthorized();
            }

            if (updateResponse.Forbidden)
            {
                return Results.StatusCode(403);
            }

            return Results.Ok(updateResponse.Team);
        })
            .WithOpenApi()
            .Produces<Team>(StatusCodes.Status201Created);

        group.MapDelete("/teams/{teamId}", async (ITeamService teamService, int teamId, string sessionKey) =>
        {
            TeamResponseDTO response = await teamService.DeleteTeamAsync(teamId, sessionKey);
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