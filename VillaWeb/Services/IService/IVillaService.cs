using VillaWeb.Models;

namespace VillaWeb.Services.IService
{
    public interface IVillaService
    {
        //we have to pass token otherwise the api will not know you are authoriezd
        Task<T> GetAllAsync<T>(string token);
        Task<T> GetAsync<T>(int id, string token);
        Task<T> CreateAsync<T>(VillaCreateDTO dto, string token);
        Task<T> UpdateAsync<T>(VillaUpdateDTO dto, string token);
        Task<T> DeleteAsync<T>(int id, string token);
    }
}
