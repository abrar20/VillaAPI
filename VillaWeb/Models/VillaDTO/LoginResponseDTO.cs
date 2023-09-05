using VillaWeb.Models.VillaDTO;

namespace VillaVillaWeb.Models.VillaDTO
{
    public class LoginResponseDTO
    {
        public UserDTO User { get; set; }
        public string Token { get; set; }
    }
}
