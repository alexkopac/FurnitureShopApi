using FurnitureShopApi.Data;
using FurnitureShopApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FurnitureShopApi.Repositories
{
    // Реализуем интерфейс. Это гарантирует, что у нас есть все нужные методы.
    public class FurnitureRepository : IFurnitureRepository
    {
        // Переменная для доступа к базе данных через Entity Framework
        private readonly AppDbContext _context;

        // Через конструктор получаем доступ к базе (Dependency Injection)
        public FurnitureRepository(AppDbContext context) => _context = context;

        // Асинхронно достаем абсолютно все записи из таблицы Furnitures
        // ToListAsync() превращает данные из базы в C#-список
        public async Task<IEnumerable<Furniture>> GetAllAsync() =>
            await _context.Furnitures.ToListAsync();

        // FindAsync - это специальный быстрый метод Entity Framework для поиска по первичному ключу (ID)
        public async Task<Furniture?> GetByIdAsync(int id) =>
            await _context.Furnitures.FindAsync(id);

        public async Task AddAsync(Furniture furniture)
        {
            // Добавляем новый объект в "оперативную память" Entity Framework
            await _context.Furnitures.AddAsync(furniture);

            // SaveChangesAsync физически отправляет SQL-запрос INSERT в базу данных Supabase
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Furniture furniture)
        {
            // Помечаем объект как измененный
            _context.Furnitures.Update(furniture);

            // Физически отправляем SQL-запрос UPDATE в базу
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            // Сначала пытаемся найти мебель по ID
            var furniture = await _context.Furnitures.FindAsync(id);

            // Если такая мебель реально существует в базе
            if (furniture != null)
            {
                // Помечаем её на удаление
                _context.Furnitures.Remove(furniture);

                // Физически отправляем SQL-запрос DELETE в базу
                await _context.SaveChangesAsync();
            }
        }
    }
}