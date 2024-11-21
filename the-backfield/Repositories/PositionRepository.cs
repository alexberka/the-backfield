using Microsoft.EntityFrameworkCore;
using TheBackfield.Data;
using TheBackfield.Interfaces;
using TheBackfield.Models;

namespace TheBackfield.Repositories;

public class PositionRepository : IPositionRepository
{
    private readonly TheBackfieldDbContext _dbContext;

    public PositionRepository(TheBackfieldDbContext context)
    {
        _dbContext = context;
    }
    public async Task<List<Position>> GetPositionsAsync()
    {
        return await _dbContext.Positions
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Position?> GetSinglePosition(int positionId)
    {
        return await _dbContext.Positions
            .AsNoTracking()
            .SingleOrDefaultAsync(p => p.Id == positionId);
    }
}