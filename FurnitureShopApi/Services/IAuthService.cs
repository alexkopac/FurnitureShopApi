using FurnitureShopApi.DTOs;

namespace FurnitureShopApi.Services
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(UserDto dto);
        Task<string> LoginAsync(UserDto dto);
    }
}