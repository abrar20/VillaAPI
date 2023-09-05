namespace VillaAPI.Models.VillaDTO
{
    public class LoginResponseDTO
    {
        public UserDTO User { get; set; }
        //we can exract it from token
        //public string Role { get; set; }
        public string Token { get; set; }
    }
}
