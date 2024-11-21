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

    public async Task<Player> CreatePlayerAsync(Player newPlayer)
    {
        _dbContext.Players.Add(newPlayer);
        await _dbContext.SaveChangesAsync();
        return newPlayer;
    }

    public Task<string?> DeletePlayerAsync(int playerId, int userId)
    {
        throw new NotImplementedException();
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

    public async Task<Player?> UpdatePlayerAsync(Player updatedPlayer)
    {
        /*Player? currentPlayer = await _dbContext.Players.SingleOrDefaultAsync(p => p.Id == updatedPlayer.Id);
        if (currentPlayer == null)
        {
            return null;
        }

        currentPlayer.FirstName = updatedPlayer.FirstName;
        currentPlayer.LastName = updatedPlayer.LastName;
        currentPlayer.ImageUrl = updatedPlayer.ImageUrl;
        currentPlayer.BirthDate = updatedPlayer.BirthDate;
        currentPlayer.Hometown = updatedPlayer.Hometown;
        currentPlayer.TeamId = updatedPlayer.TeamId;
        currentPlayer.JerseyNumber = updatedPlayer.JerseyNumber;*/
        _dbContext.Update(updatedPlayer);
        await _dbContext.SaveChangesAsync();
        return updatedPlayer;
    }
}