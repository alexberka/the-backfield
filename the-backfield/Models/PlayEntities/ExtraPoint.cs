﻿using System.ComponentModel.DataAnnotations;

namespace TheBackfield.Models.PlayEntities
{
    public class ExtraPoint
    {
        public int Id { get; set; }
        [Required]
        public int PlayId { get; set; }
        public Play? Play { get; set; }
        public int? KickerId { get; set; } = null;
        public Player? Kicker { get; set; }
        public int TeamId { get; set; }
        public bool Good { get; set; }
        public bool Fake { get; set; }
        public bool DefensiveConversion { get; set; }
        public int? ReturnerId { get; set; } = null;
        public Player? Returner { get; set; }
        public int ReturnTeamId { get; set; }
    }
}
