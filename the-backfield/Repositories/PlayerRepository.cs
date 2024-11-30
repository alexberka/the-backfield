using System.Numerics;
using Microsoft.EntityFrameworkCore;
using TheBackfield.Data;
using TheBackfield.DTOs;
using TheBackfield.Interfaces;
using TheBackfield.Models;

namespace TheBackfield.Repositories;

public class PlayerRepository : IPlayerRepository
{
    private readonly TheBackfieldDbContext _dbContext;

    public PlayerRepository(TheBackfieldDbContext context)
    {
        _dbContext = context;
    }

    public async Task<Player> CreatePlayerAsync(PlayerSubmitDTO playerSubmit, int userId)
    {
        Player newPlayer = new Player
        {
            FirstName = playerSubmit.FirstName ?? "",
            LastName = playerSubmit.LastName ?? "",
            ImageUrl = playerSubmit.ImageUrl ?? "",
            BirthDate = playerSubmit.BirthDate ?? DateTime.MinValue,
            Hometown = playerSubmit.Hometown ?? "",
            TeamId = playerSubmit.TeamId,
            JerseyNumber = playerSubmit.JerseyNumber ?? 0,
            UserId = userId
        };

        _dbContext.Players.Add(newPlayer);
        await _dbContext.SaveChangesAsync();
        return newPlayer;
    }

    public async Task<string?> DeletePlayerAsync(int playerId)
    {
        Player? player = await _dbContext.Players.FindAsync(playerId);
        if (player == null)
        {
            return "Invalid player id";
        }

        _dbContext.Players.Remove(player);
        await _dbContext.SaveChangesAsync();
        return null;
    }

    public async Task<List<Player>> GetPlayersAsync(int userId)
    {
        return await _dbContext.Players.AsNoTracking()
            .Include(p => p.Team)
            .Include(p => p.Positions)
            .Where(p => p.UserId == userId)
            .ToListAsync();
    }

    public async Task<Player?> GetSinglePlayerAsync(int playerId)
    {
        return await _dbContext.Players.AsNoTracking()
            .Include(p => p.Team)
            .Include(p => p.Positions)
            .SingleOrDefaultAsync(p => p.Id == playerId);
    }

    public async Task<Player?> UpdatePlayerAsync(PlayerSubmitDTO playerSubmit)
    {
        Player? currentPlayer = await _dbContext.Players.SingleOrDefaultAsync(p => p.Id == playerSubmit.Id);
        if (currentPlayer == null)
        {
            return null;
        }

        currentPlayer.FirstName = playerSubmit.FirstName ?? currentPlayer.FirstName;
        currentPlayer.LastName = playerSubmit.LastName ?? currentPlayer.LastName;
        currentPlayer.ImageUrl = playerSubmit.ImageUrl ?? playerSubmit.ImageUrl;
        currentPlayer.BirthDate = playerSubmit.BirthDate ?? currentPlayer.BirthDate;
        currentPlayer.Hometown = playerSubmit.Hometown ?? currentPlayer.Hometown;
        currentPlayer.TeamId = playerSubmit.TeamId != 0 ? playerSubmit.TeamId : currentPlayer.TeamId;
        currentPlayer.JerseyNumber = playerSubmit.JerseyNumber ?? currentPlayer.JerseyNumber;
        
        await _dbContext.SaveChangesAsync();
        return currentPlayer;
    }

    public async Task<Player?> SetPlayerPositionsAsync(int playerId, List<int> positionIds)
    {
        Player? player = await _dbContext.Players
            .Include(p => p.Positions)
            .SingleOrDefaultAsync(p => p.Id == playerId);

        if (player == null)
        {
            return null;
        }

        player.Positions.RemoveAll(pp => !positionIds.Any(pi => pi == pp.Id));

        List<Position> playerPositions = await _dbContext.Positions
            .Where(p => positionIds.Any(pi => pi == p.Id) && !player.Positions.Contains(p))
            .ToListAsync();

        player.Positions.AddRange(playerPositions);

        await _dbContext.SaveChangesAsync();

        return player;
    }

    public async Task<Player?> AddPlayerPositionsAsync(int playerId, List<int> positionIds)
    {
        Player? player = await _dbContext.Players
            .Include(p => p.Positions)
            .SingleOrDefaultAsync(p => p.Id == playerId);

        if (player == null)
        {
            return null;
        }

        List<Position> playerPositions = await _dbContext.Positions
            .Where(p => positionIds.Any(pi => pi == p.Id) && !player.Positions.Contains(p))
            .ToListAsync();

        player.Positions.AddRange(playerPositions);

        await _dbContext.SaveChangesAsync();

        return player;
    }

    public async Task<Player?> RemovePlayerPositionsAsync(int playerId, List<int> positionIds)
    {
        Player? player = await _dbContext.Players
            .Include(p => p.Positions)
            .SingleOrDefaultAsync(p => p.Id == playerId);

        if (player == null)
        {
            return null;
        }

        player.Positions.RemoveAll(pp => positionIds.Any(pi => pi == pp.Id));

        await _dbContext.SaveChangesAsync();

        return player;
    }
}