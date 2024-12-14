using TheBackfield.Models.PlayEntities;

namespace TheBackfield.Models
{
    public class Play
    {
        public int Id { get; set; }
        public int? PrevPlayId { get; set; } = null;
        public Play? PrevPlay { get; set; }
        public int? GameId { get; set; } = null;
        public Game Game { get; set; }
        public int? TeamId { get; set; } = null;
        public Team Team { get; set; }
        public int? FieldPositionStart { get; set; } = null;
        public int? FieldPositionEnd { get; set; } = null;
        public int Down { get; set; } = 0;
        public int? ToGain { get; set; } = null;
        public int? ClockStart { get; set; } = null;
        public int? ClockEnd { get; set; } = null;
        public int? GamePeriod { get; set; } = null;
        public string Notes { get; set; } = "";
        public Pass? Pass { get; set; }
        public Rush? Rush { get; set; }
        public List<Tackle> Tacklers { get; set; } = [];
        public List<PassDefense> PassDefenders { get; set; } = [];
        public Kickoff? Kickoff { get; set; }
        public Punt? Punt { get; set; }
        public FieldGoal? FieldGoal { get; set; }
        public Touchdown? Touchdown { get; set; }
        public ExtraPoint? ExtraPoint { get; set; }
        public Conversion? Conversion { get; set; }
        public Safety? Safety { get; set; }
        public List<Fumble> Fumbles { get; set; } = [];
        public Interception? Interception { get; set; }
        public KickBlock? KickBlock { get; set; }
        public List<Lateral> Laterals { get; set; } = [];
        public List<PlayPenalty> Penalties { get; set; } = [];
    }
}
