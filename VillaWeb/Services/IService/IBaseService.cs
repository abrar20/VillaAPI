using VillaWeb.Models;

namespace VillaWeb.Services.IService
{
    public interface IBaseService
    {
        APIResponse responseModel { get; set; }
        Task <T> SendAsync <T> (APIRequest apiRequest);
    }
}
