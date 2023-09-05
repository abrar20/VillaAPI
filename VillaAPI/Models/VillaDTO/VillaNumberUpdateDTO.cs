﻿
using System.ComponentModel.DataAnnotations;

namespace VillaAPI.Models.VillaDTO
{
    public class VillaNumberUpdateDTO
    {
        [Required]
        public int VillaNo { get; set; }
        public string SpecialDetails { get; set; }
        [Required]
        public int VillaID { get; set; }
    }
}
