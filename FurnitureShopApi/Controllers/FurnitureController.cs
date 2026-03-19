using FurnitureShopApi.DTOs;
using FurnitureShopApi.Models;
using FurnitureShopApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureShopApi.Controllers
{
    // Указывает, что этот класс - контроллер для API. 
    // Автоматически проверяет правильность входящих данных (валидация).
    [ApiController]

    // Настраивает URL, по которому будет доступен контроллер. 
    // [controller] подставит имя класса без слова Controller -> будет /api/Furniture
    [Route("api/[controller]")]
    public class FurnitureController : ControllerBase // ControllerBase дает нам методы Ok(), NotFound() и т.д.
    {
        // Переменная для хранения ссылки на репозиторий. 
        // readonly означает, что мы не можем случайно ее изменить после создания.
        private readonly IFurnitureRepository _repository;

        // КОНСТРУКТОР: Внедрение зависимостей (Dependency Injection).
        // Мы просим ASP.NET дать нам готовый репозиторий через интерфейс, 
        // чтобы не создавать его вручную (соблюдаем принцип SOLID - Dependency Inversion).
        public FurnitureController(IFurnitureRepository repository)
        {
            _repository = repository;
        }

        // Обрабатывает HTTP GET запросы по адресу /api/Furniture
        [HttpGet]
        // Task<IActionResult> означает, что метод асинхронный и вернет HTTP-ответ (статус код + данные)
        public async Task<IActionResult> GetAll()
        {
            // Ждем (await), пока репозиторий достанет всю мебель из базы данных
            var items = await _repository.GetAllAsync();

            // Возвращаем статус 200 (OK) и список мебели в формате JSON
            return Ok(items);
        }

        // Обрабатывает GET запросы с параметром ID, например: /api/Furniture/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            // Ищем конкретный элемент по ID
            var item = await _repository.GetByIdAsync(id);

            // Если в базе ничего не найдено, возвращаем статус 404 (Not Found)
            if (item == null) return NotFound();

            // Если нашли - возвращаем статус 200 и сам элемент
            return Ok(item);
        }

        // Обрабатывает POST запросы (добавление новых данных)
        [HttpPost]
        // Принимаем не сырую модель БД, а DTO (объект передачи данных), чтобы скрыть лишнее
        public async Task<IActionResult> Create(CreateFurnitureDto dto)
        {
            // Перекладываем данные из DTO в настоящую модель для базы данных
            var furniture = new Furniture
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Category = dto.Category,
                StockQuantity = dto.StockQuantity
            };

            // Передаем модель в репозиторий для сохранения в базу
            await _repository.AddAsync(furniture);

            // Возвращаем статус 201 (Created) и саму созданную запись
            return Ok(furniture);
        }

        // Обрабатывает PUT запросы (полное обновление записи) по ID
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Furniture furniture)
        {
            // Защита: проверяем, совпадает ли ID в URL с ID в теле запроса
            if (id != furniture.Id) return BadRequest(); // Статус 400 (Плохой запрос)

            // Отправляем обновленные данные в базу
            await _repository.UpdateAsync(furniture);

            // Статус 204 (No Content) - всё успешно, но серверу нечего возвращать в ответ
            return NoContent();
        }

        // Обрабатывает DELETE запросы (удаление записи) по ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // Просим репозиторий удалить запись с таким ID
            await _repository.DeleteAsync(id);

            // Возвращаем статус 204 (успешно удалено)
            return NoContent();
        }
    }
}