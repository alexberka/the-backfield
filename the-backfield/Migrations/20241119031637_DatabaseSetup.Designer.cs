﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TheBackfield.Data;

#nullable disable

namespace the_backfield.Migrations
{
    [DbContext(typeof(TheBackfieldDbContext))]
    [Migration("20241119031637_DatabaseSetup")]
    partial class DatabaseSetup
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("PlayerPosition", b =>
                {
                    b.Property<int>("PlayersId")
                        .HasColumnType("integer");

                    b.Property<int>("PositionsId")
                        .HasColumnType("integer");

                    b.HasKey("PlayersId", "PositionsId");

                    b.HasIndex("PositionsId");

                    b.ToTable("PlayerPosition");
                });

            modelBuilder.Entity("TheBackfield.Models.Game", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AwayTeamId")
                        .HasColumnType("integer");

                    b.Property<int>("AwayTeamScore")
                        .HasColumnType("integer");

                    b.Property<int>("GamePeriods")
                        .HasColumnType("integer");

                    b.Property<DateTime>("GameStart")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("HomeTeamId")
                        .HasColumnType("integer");

                    b.Property<int>("HomeTeamScore")
                        .HasColumnType("integer");

                    b.Property<int>("PeriodLength")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("AwayTeamId");

                    b.HasIndex("HomeTeamId");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("TheBackfield.Models.GameStat", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("FumblesCommitted")
                        .HasColumnType("integer");

                    b.Property<int>("FumblesForced")
                        .HasColumnType("integer");

                    b.Property<int>("FumblesRecovered")
                        .HasColumnType("integer");

                    b.Property<int>("GameId")
                        .HasColumnType("integer");

                    b.Property<int>("InterceptionsReceived")
                        .HasColumnType("integer");

                    b.Property<int>("InterceptionsThrown")
                        .HasColumnType("integer");

                    b.Property<int>("PassAttempts")
                        .HasColumnType("integer");

                    b.Property<int>("PassCompletions")
                        .HasColumnType("integer");

                    b.Property<int>("PassYards")
                        .HasColumnType("integer");

                    b.Property<int>("PlayerId")
                        .HasColumnType("integer");

                    b.Property<int>("ReceivingYards")
                        .HasColumnType("integer");

                    b.Property<int>("Receptions")
                        .HasColumnType("integer");

                    b.Property<int>("RushAttempts")
                        .HasColumnType("integer");

                    b.Property<int>("RushYards")
                        .HasColumnType("integer");

                    b.Property<int>("Tackles")
                        .HasColumnType("integer");

                    b.Property<int>("TeamId")
                        .HasColumnType("integer");

                    b.Property<int>("Touchdowns")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.HasIndex("PlayerId");

                    b.HasIndex("TeamId");

                    b.ToTable("GameStats");
                });

            modelBuilder.Entity("TheBackfield.Models.Player", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("BirthDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Hometown")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("JerseyNumber")
                        .HasColumnType("integer");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("TeamId")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("TeamId");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("TheBackfield.Models.Position", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Abbreviation")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Positions");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Abbreviation = "QB",
                            Name = "Quarterback"
                        },
                        new
                        {
                            Id = 2,
                            Abbreviation = "RB",
                            Name = "Running Back"
                        },
                        new
                        {
                            Id = 3,
                            Abbreviation = "HB",
                            Name = "Halfback"
                        },
                        new
                        {
                            Id = 4,
                            Abbreviation = "FB",
                            Name = "Fullback"
                        },
                        new
                        {
                            Id = 5,
                            Abbreviation = "WR",
                            Name = "Wide Receiver"
                        },
                        new
                        {
                            Id = 6,
                            Abbreviation = "TE",
                            Name = "Tight End"
                        },
                        new
                        {
                            Id = 7,
                            Abbreviation = "OL",
                            Name = "Offensive Line"
                        },
                        new
                        {
                            Id = 8,
                            Abbreviation = "C",
                            Name = "Center"
                        },
                        new
                        {
                            Id = 9,
                            Abbreviation = "OG",
                            Name = "Offensive Guard"
                        },
                        new
                        {
                            Id = 10,
                            Abbreviation = "LG",
                            Name = "Left Guard"
                        },
                        new
                        {
                            Id = 11,
                            Abbreviation = "RG",
                            Name = "Right Guard"
                        },
                        new
                        {
                            Id = 12,
                            Abbreviation = "OT",
                            Name = "Offensive Tackle"
                        },
                        new
                        {
                            Id = 13,
                            Abbreviation = "LT",
                            Name = "Left Tackle"
                        },
                        new
                        {
                            Id = 14,
                            Abbreviation = "RT",
                            Name = "Right Tackle"
                        },
                        new
                        {
                            Id = 15,
                            Abbreviation = "DL",
                            Name = "Defensive Line"
                        },
                        new
                        {
                            Id = 16,
                            Abbreviation = "NT",
                            Name = "Nose Tackle"
                        },
                        new
                        {
                            Id = 17,
                            Abbreviation = "DE",
                            Name = "Defensive End"
                        },
                        new
                        {
                            Id = 18,
                            Abbreviation = "DT",
                            Name = "Defensive Tackle"
                        },
                        new
                        {
                            Id = 19,
                            Abbreviation = "LB",
                            Name = "Linebacker"
                        },
                        new
                        {
                            Id = 20,
                            Abbreviation = "ILB",
                            Name = "Inside Linebacker"
                        },
                        new
                        {
                            Id = 21,
                            Abbreviation = "MLB",
                            Name = "Middle Linebacker"
                        },
                        new
                        {
                            Id = 22,
                            Abbreviation = "OLB",
                            Name = "Outside Linebacker"
                        },
                        new
                        {
                            Id = 23,
                            Abbreviation = "EDGE",
                            Name = "Edge Rusher"
                        },
                        new
                        {
                            Id = 24,
                            Abbreviation = "CB",
                            Name = "Cornerback"
                        },
                        new
                        {
                            Id = 25,
                            Abbreviation = "S",
                            Name = "Safety"
                        },
                        new
                        {
                            Id = 26,
                            Abbreviation = "FS",
                            Name = "Free Safety"
                        },
                        new
                        {
                            Id = 27,
                            Abbreviation = "SS",
                            Name = "Strong Safety"
                        },
                        new
                        {
                            Id = 28,
                            Abbreviation = "P",
                            Name = "Punter"
                        },
                        new
                        {
                            Id = 29,
                            Abbreviation = "PK",
                            Name = "Place Kicker"
                        },
                        new
                        {
                            Id = 30,
                            Abbreviation = "LS",
                            Name = "Long Snapper"
                        });
                });

            modelBuilder.Entity("TheBackfield.Models.Team", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ColorPrimary")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ColorSecondary")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("HomeField")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("HomeLocation")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LocationName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LogoUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Nickname")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("TheBackfield.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("SessionKey")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("SessionStart")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Uid")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("PlayerPosition", b =>
                {
                    b.HasOne("TheBackfield.Models.Player", null)
                        .WithMany()
                        .HasForeignKey("PlayersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TheBackfield.Models.Position", null)
                        .WithMany()
                        .HasForeignKey("PositionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TheBackfield.Models.Game", b =>
                {
                    b.HasOne("TheBackfield.Models.Team", "AwayTeam")
                        .WithMany()
                        .HasForeignKey("AwayTeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TheBackfield.Models.Team", "HomeTeam")
                        .WithMany()
                        .HasForeignKey("HomeTeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AwayTeam");

                    b.Navigation("HomeTeam");
                });

            modelBuilder.Entity("TheBackfield.Models.GameStat", b =>
                {
                    b.HasOne("TheBackfield.Models.Game", null)
                        .WithMany()
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TheBackfield.Models.Player", null)
                        .WithMany()
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TheBackfield.Models.Team", null)
                        .WithMany()
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TheBackfield.Models.Player", b =>
                {
                    b.HasOne("TheBackfield.Models.Team", "Team")
                        .WithMany("Players")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Team");
                });

            modelBuilder.Entity("TheBackfield.Models.Team", b =>
                {
                    b.Navigation("Players");
                });
#pragma warning restore 612, 618
        }
    }
}
