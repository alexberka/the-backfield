﻿using System.ComponentModel.DataAnnotations;

namespace TheBackfield.Models.PlayEntities
{
    public class Lateral
    {
        public int Id { get; set; }
        [Required]
        public int PlayId { get; set; }
        public Play? Play { get; set; }
        public int? PrevCarrierId { get; set; } = null;
        public Player? PrevCarrier { get; set; }
        public int? NewCarrierId { get; set; } = null;
        public Player? NewCarrier { get; set; }
        public int TeamId { get; set; }
        public int? PossessionAt { get; set; }
        public int? CarriedTo { get; set; }
        public int Yardage { get; set; }
        public string YardageType { get; set; } = "";
    }
}
