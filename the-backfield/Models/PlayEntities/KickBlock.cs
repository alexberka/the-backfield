﻿using System.ComponentModel.DataAnnotations;

namespace TheBackfield.Models.PlayEntities
{
    public class KickBlock
    {
        public int Id { get; set; }
        [Required]
        public int PlayId { get; set; }
        public Play Play { get; set; }
        public int? BlockedById { get; set; }
        public Player? BlockedBy { get; set; }
        public int? RecoveredById { get; set; }
        public Player? RecoveredBy { get; set; }
        public int ReturnedTo { get; set; }

    }
}