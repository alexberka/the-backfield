using TheBackfield.Models;

namespace TheBackfield.Data
{
    public class PenaltyData
    {
        // Default penalties derived from NFL 2024 Rule Book
        public static List<Penalty> Penalties = new()
        {
            new() { Id = 1, Name = "Penalty" },
            new() { Id = 2, Name = "Chop Block", Yardage = 15 },
            new() { Id = 3, Name = "Clipping", Yardage = 15 },
            new() { Id = 4, Name = "Defensive Holding", AutoFirstDown = true, Yardage = 5 },
            new() { Id = 5, Name = "Defensive Offside", Yardage = 5 },
            new() { Id = 6, Name = "Defensive Pass Interference", AutoFirstDown = true },
            new() { Id = 7, Name = "Defensive Too Many Men on Field", NoPlay = true, Yardage = 5 },
            new() { Id = 8, Name = "Delay of Game", Yardage = 5 },
            new() { Id = 9, Name = "Encroachment", Yardage = 5 },
            new() { Id = 10, Name = "Facemask", AutoFirstDown = true, Yardage = 15 },
            new() { Id = 11, Name = "Fair Catch Interference", Yardage = 15 },
            new() { Id = 12, Name = "False Start", Yardage = 5 },
            new() { Id = 13, Name = "Hip-Drop Tackle", AutoFirstDown = true, Yardage = 15 },
            new() { Id = 14, Name = "Horse-Collar Tackle", AutoFirstDown = true, Yardage = 15 },
            new() { Id = 15, Name = "Illegal Block in the Back", Yardage = 10 },
            new() { Id = 16, Name = "Illegal Contact", AutoFirstDown = true, Yardage = 5 },
            new() { Id = 17, Name = "Illegal Formation", Yardage = 5 },
            new() { Id = 18, Name = "Illegal Forward Pass", LossOfDown = true, Yardage = 5 },
            new() { Id = 19, Name = "Illegal Kicking Loose Ball", Yardage = 10 },
            new() { Id = 20, Name = "Illegal Motion", Yardage = 5 },
            new() { Id = 21, Name = "Illegal Shift", Yardage = 5 },
            new() { Id = 22, Name = "Illegal Substitution", Yardage = 5 },
            new() { Id = 23, Name = "Illegal Touch - Player Out of Bounds", LossOfDown = true },
            new() { Id = 24, Name = "Illegal Touch - Ineligible Receiver", LossOfDown = true, Yardage = 5 },
            new() { Id = 25, Name = "Illegal Use of Hands", AutoFirstDown = true, Yardage = 10 },
            new() { Id = 26, Name = "Impermissible Use of the Helmet", AutoFirstDown = true, Yardage = 15 },
            new() { Id = 27, Name = "Ineligible Player Downfield", Yardage = 5 },
            new() { Id = 28, Name = "Intentional Grounding", LossOfDown = true, Yardage = 10 },
            new() { Id = 29, Name = "Invalid Fair Catch Signal", Yardage = 5 },
            new() { Id = 30, Name = "Kickoff Out of Bounds" },
            new() { Id = 31, Name = "Leaping", Yardage = 15 },
            new() { Id = 32, Name = "Neutral Zone Infraction", Yardage = 5 },
            new() { Id = 33, Name = "Offensive Holding", Yardage = 10 },
            new() { Id = 34, Name = "Offensive Offside", Yardage = 5 },
            new() { Id = 35, Name = "Offensive Pass Interference", Yardage = 10 },
            new() { Id = 36, Name = "Offensive Too Many Men on Field", Yardage = 5 },
            new() { Id = 37, Name = "Roughing the Kicker", AutoFirstDown = true, Yardage = 15 },
            new() { Id = 38, Name = "Roughing the Passer", NoPlay = false, AutoFirstDown = true, Yardage = 15 },
            new() { Id = 39, Name = "Running into the Kicker", Yardage = 5 },
            new() { Id = 40, Name = "Taunting", NoPlay = false, Yardage = 15 },
            new() { Id = 41, Name = "Tripping", AutoFirstDown = true, Yardage = 15 },
            new() { Id = 42, Name = "Unnecessary Roughness", NoPlay = false, AutoFirstDown = true, Yardage = 15 },
            new() { Id = 43, Name = "Unsportsmanlike Conduct", NoPlay = false, AutoFirstDown = true, Yardage = 15 }
        };
    }
}
