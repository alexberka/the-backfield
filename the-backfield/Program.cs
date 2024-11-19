using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using TheBackfield.Data;
using TheBackfield.Endpoints;
using TheBackfield.Interfaces;
using TheBackfield.Repositories;
using TheBackfield.Services;

var builder = WebApplication.CreateBuilder(args);

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddNpgsql<TheBackfieldDbContext>(builder.Configuration["TheBackfieldConnectionString"]);

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IGameRepository, GameRepository>();

builder.Services.AddScoped<IGameStatService, GameStatService>();
builder.Services.AddScoped<IGameStatRepository, GameStatRepository>();

builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();

builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<ITeamRepository, TeamRepository>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000")
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseHttpsRedirection();

app.MapGameEndpoints();
app.MapGameStatEndpoints();
app.MapPlayerEndpoints();
app.MapPositionEndpoints();
app.MapTeamEndpoints();
app.MapUserEndpoints();

app.Run();
