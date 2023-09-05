
using System.ComponentModel.DataAnnotations;

namespace VillaAPI.Models.VillaDTO
{
    public class VillaNumberDTO
    {
        [Required]
        public int VillaNo { get; set; }
        public string SpecialDetails { get; set; }
        [Required]
        public int VillaID { get; set; }
        public VillaDTO Villa { get; set; }
    }
}
