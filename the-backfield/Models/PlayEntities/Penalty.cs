﻿using System.ComponentModel.DataAnnotations;

namespace TheBackfield.Models.PlayEntities
{
    public class Penalty
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = "";
        public bool NoPlay { get; set; }
        public bool AutoFirstDown { get; set; }
        public int Yardage { get; set; } = 0;
    }
}
