using Microsoft.EntityFrameworkCore;
using TheBackfield.Models;

namespace TheBackfield.Data;

public class TheBackfieldDbContext : DbContext
{
    public DbSet<Game> Games { get; set; }
    public DbSet<GameStat> GameStats { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<Position> Positions { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<User> Users { get; set; }

    public TheBackfieldDbContext(DbContextOptions<TheBackfieldDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}