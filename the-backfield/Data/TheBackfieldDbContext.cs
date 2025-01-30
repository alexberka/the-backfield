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
    public DbSet<Touchdown> Touchdowns { get; set; }
    public DbSet<ExtraPoint> ExtraPoints { get; set; }
    public DbSet<Conversion> Conversions { get; set; }
    public DbSet<Safety> Safeties { get; set; }
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
        modelBuilder.Entity<Play>()
            .HasOne(p => p.Game)
            .WithMany(g => g.Plays)
            .OnDelete(DeleteBehavior.Cascade);

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

        modelBuilder.Entity<Pass>()
            .HasOne(p => p.Passer)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<Pass>()
            .HasOne(p => p.Receiver)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Rush>()
            .HasOne(r => r.Rusher)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Tackle>()
            .HasOne(t => t.Tackler)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<PassDefense>()
            .HasOne(pd => pd.Defender)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Kickoff>()
            .HasOne(k => k.Kicker)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<Kickoff>()
            .HasOne(k => k.Returner)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Punt>()
            .HasOne(p => p.Kicker)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<Punt>()
            .HasOne(p => p.Returner)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<FieldGoal>()
            .HasOne(fg => fg.Kicker)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Touchdown>()
            .HasOne(fg => fg.Player)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<ExtraPoint>()
            .HasOne(ep => ep.Kicker)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<ExtraPoint>()
            .HasOne(ep => ep.Returner)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Conversion>()
            .HasOne(c => c.Passer)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<Conversion>()
            .HasOne(c => c.Receiver)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<Conversion>()
            .HasOne(c => c.Rusher)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<Conversion>()
            .HasOne(c => c.Returner)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Safety>()
            .HasOne(c => c.CedingPlayer)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Fumble>()
            .HasOne(f => f.FumbleCommittedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<Fumble>()
            .HasOne(f => f.FumbleForcedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<Fumble>()
            .HasOne(f => f.FumbleRecoveredBy)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Interception>()
            .HasOne(i => i.InterceptedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<KickBlock>()
            .HasOne(kb => kb.BlockedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<KickBlock>()
            .HasOne(kb => kb.RecoveredBy)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Lateral>()
            .HasOne(l => l.NewCarrier)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<Lateral>()
            .HasOne(l => l.PrevCarrier)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<PlayPenalty>()
            .HasOne(pp => pp.Player)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Penalty>().HasData(PenaltyData.Penalties);
        modelBuilder.Entity<Play>().HasData(PlayData.Plays);
        modelBuilder.Entity<Position>().HasData(PositionData.Positions);
    }
}