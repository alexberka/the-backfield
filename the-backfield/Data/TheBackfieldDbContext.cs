using Microsoft.EntityFrameworkCore;
using TheBackfield.Models;
using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Data;

public class TheBackfieldDbContext : DbContext
{
    public DbSet<Game> Games { get; set; }
    public DbSet<GameStat> GameStats { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<Position> Positions { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Play> Plays { get; set; }
    public DbSet<Pass> Passes { get; set; }
    public DbSet<Rush> Rushes { get; set; }
    public DbSet<Tackle> Tackles { get; set; }
    public DbSet<PassDefense> PassDefenses { get; set; }
    public DbSet<Kickoff> Kickoffs { get; set; }
    public DbSet<Punt> Punts { get; set; }
    public DbSet<FieldGoal> FieldGoals { get; set; }
    public DbSet<ExtraPoint> ExtraPoints { get; set; }
    public DbSet<Conversion> Conversion { get; set; }
    public DbSet<Fumble> Fumbles { get; set; }
    public DbSet<Interception> Interceptions { get; set; }
    public DbSet<KickBlock> KickBlocks { get; set; }
    public DbSet<Lateral> Laterals { get; set; }
    public DbSet<Penalty> Penalties { get; set; }
    public DbSet<PlayPenalty> PlayPenalties { get; set; }

    public TheBackfieldDbContext(DbContextOptions<TheBackfieldDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GameStat>()
            .HasOne<Team>()
            .WithMany()
            .HasForeignKey(gs => gs.TeamId);

        modelBuilder.Entity<GameStat>()
            .HasOne<Game>()
            .WithMany(g => g.GameStats)
            .HasForeignKey(gs => gs.GameId);

        modelBuilder.Entity<GameStat>()
            .HasOne<Player>()
            .WithMany()
            .HasForeignKey(gs => gs.PlayerId);

        modelBuilder.Entity<Position>().HasData(PositionData.Positions);
    }
}