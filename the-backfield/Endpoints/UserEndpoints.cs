using TheBackfield.DTOs;
using TheBackfield.Interfaces;
using TheBackfield.Models;

namespace TheBackfield.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("").WithTags(nameof(User));

        group.MapGet("/users/{uid}", async (IUserService userService, string uid) =>
        {
            User? user = await userService.GetUserDataAsync(uid);

            if (user == null)
            {
                return Results.NotFound("User does not exist.");
            }

            return Results.Ok(user);
        })
            .WithOpenApi()
            .Produces<User>(StatusCodes.Status200OK);

        group.MapPost("/users", async (IUserService userService, UserSubmitDTO userSubmit) =>
        {
            if (userSubmit.Username == "" || userSubmit.Uid == "")
            {
                return Results.BadRequest("Username and Uid cannot be empty.");
            }

            User? newUser = await userService.CreateUserAsync(userSubmit);

            if (newUser == null)
            {
                return Results.BadRequest("Uid already in use by existing User.");
            }

            return Results.Created($"/users/{newUser.SessionKey}", newUser);
        })
            .WithOpenApi()
            .Produces<User>(StatusCodes.Status201Created);
    }
}