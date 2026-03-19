using FurnitureShopApi.Models;

namespace FurnitureShopApi.Repositories
{
    public interface IFurnitureRepository
    {
        Task<IEnumerable<Furniture>> GetAllAsync();
        Task<Furniture?> GetByIdAsync(int id);
        Task AddAsync(Furniture furniture);
        Task UpdateAsync(Furniture furniture);
        Task DeleteAsync(int id);
    }
}