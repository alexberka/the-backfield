using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using the_backfield.Interfaces.PlayEntities;
using TheBackfield.Data;
using TheBackfield.Endpoints;
using TheBackfield.Interfaces;
using TheBackfield.Interfaces.PlayEntities;
using TheBackfield.Repositories;
using TheBackfield.Repositories.PlayEntities;
using TheBackfield.Services;

var builder = WebApplication.CreateBuilder(args);

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddNpgsql<TheBackfieldDbContext>(builder.Configuration["TheBackfieldDbConnectionString"]);

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IGameRepository, GameRepository>();

builder.Services.AddScoped<IGameStatService, GameStatService>();
builder.Services.AddScoped<IGameStatRepository, GameStatRepository>();

builder.Services.AddScoped<IPenaltyService, PenaltyService>();
builder.Services.AddScoped<IPenaltyRepository, PenaltyRepository>();

builder.Services.AddScoped<IPlayService, PlayService>();
builder.Services.AddScoped<IPlayRepository, PlayRepository>();

builder.Services.AddScoped<IConversionRepository, ConversionRepository>();
builder.Services.AddScoped<IExtraPointRepository, ExtraPointRepository>();
builder.Services.AddScoped<IFieldGoalRepository, FieldGoalRepository>();
builder.Services.AddScoped<IFumbleRepository, FumbleRepository>();
builder.Services.AddScoped<IInterceptionRepository, InterceptionRepository>();
builder.Services.AddScoped<IKickBlockRepository, KickBlockRepository>();
builder.Services.AddScoped<IKickoffRepository, KickoffRepository>();
builder.Services.AddScoped<ILateralRepository, LateralRepository>();
builder.Services.AddScoped<IPassDefenseRepository, PassDefenseRepository>();
builder.Services.AddScoped<IPassRepository, PassRepository>();
builder.Services.AddScoped<IPlayPenaltyRepository, PlayPenaltyRepository>();
builder.Services.AddScoped<IPuntRepository, PuntRepository>();
builder.Services.AddScoped<IRushRepository, RushRepository>();
builder.Services.AddScoped<ISafetyRepository, SafetyRepository> ();
builder.Services.AddScoped<ITackleRepository, TackleRepository>();
builder.Services.AddScoped<ITouchdownRepository, TouchdownRepository>();

builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();

builder.Services.AddScoped<IPositionService, PositionService>();
builder.Services.AddScoped<IPositionRepository, PositionRepository>();

builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<ITeamRepository, TeamRepository>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000")
            .AllowCredentials()
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
app.MapPenaltyEndpoints();
app.MapPlayEndpoints();
app.MapPlayerEndpoints();
app.MapPositionEndpoints();
app.MapTeamEndpoints();
app.MapUserEndpoints();

app.Run();
