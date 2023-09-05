using VillaAPI.Models;
using VillaAPI.Models.VillaDTO;

namespace VillaAPI.Repository.IRepository
{
    public interface IUserRepository
    {
        bool IsUniqueUser (string username);
        Task<LoginResponseDTO> Login (LoginRequestDTO loginRequestDTO);
        Task<UserDTO> Rgister(RegisterationRequestDTO registerationRequestDTO);
    }
}
