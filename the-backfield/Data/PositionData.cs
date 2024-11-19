using TheBackfield.Models;

namespace TheBackfield.Data
{
    public class PositionData
    {
        public static List<Position> Positions = new()
        {
            new() { Id = 1, Name = "Quarterback", Abbreviation = "QB"},
            new() { Id = 2, Name = "Running Back", Abbreviation = "RB"},
            new() { Id = 3, Name = "Halfback", Abbreviation = "HB"},
            new() { Id = 4, Name = "Fullback", Abbreviation = "FB"},
            new() { Id = 5, Name = "Wide Receiver", Abbreviation = "WR"},
            new() { Id = 6, Name = "Tight End", Abbreviation = "TE"},
            new() { Id = 7, Name = "Offensive Line", Abbreviation = "OL"},
            new() { Id = 8, Name = "Center", Abbreviation = "C"},
            new() { Id = 9, Name = "Offensive Guard", Abbreviation = "OG"},
            new() { Id = 10, Name = "Left Guard", Abbreviation = "LG"},
            new() { Id = 11, Name = "Right Guard", Abbreviation = "RG"},
            new() { Id = 12, Name = "Offensive Tackle", Abbreviation = "OT"},
            new() { Id = 13, Name = "Left Tackle", Abbreviation = "LT"},
            new() { Id = 14, Name = "Right Tackle", Abbreviation = "RT"},
            new() { Id = 15, Name = "Defensive Line", Abbreviation = "DL"},
            new() { Id = 16, Name = "Nose Tackle", Abbreviation = "NT"},
            new() { Id = 17, Name = "Defensive End", Abbreviation = "DE"},
            new() { Id = 18, Name = "Defensive Tackle", Abbreviation = "DT"},
            new() { Id = 19, Name = "Linebacker", Abbreviation = "LB"},
            new() { Id = 20, Name = "Inside Linebacker", Abbreviation = "ILB"},
            new() { Id = 21, Name = "Middle Linebacker", Abbreviation = "MLB"},
            new() { Id = 22, Name = "Outside Linebacker", Abbreviation = "OLB"},
            new() { Id = 23, Name = "Edge Rusher", Abbreviation = "EDGE"},
            new() { Id = 24, Name = "Cornerback", Abbreviation = "CB"},
            new() { Id = 25, Name = "Safety", Abbreviation = "S"},
            new() { Id = 26, Name = "Free Safety", Abbreviation = "FS"},
            new() { Id = 27, Name = "Strong Safety", Abbreviation = "SS"},
            new() { Id = 28, Name = "Punter", Abbreviation = "P"},
            new() { Id = 29, Name = "Place Kicker", Abbreviation = "PK"},
            new() { Id = 30, Name = "Long Snapper", Abbreviation = "LS"}
        };
    }
}
