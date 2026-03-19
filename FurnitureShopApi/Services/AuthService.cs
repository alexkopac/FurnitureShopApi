using FurnitureShopApi.Data;
using FurnitureShopApi.DTOs;
using FurnitureShopApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FurnitureShopApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context; // Доступ к БД
        private readonly IConfiguration _configuration; // Доступ к appsettings.json (чтобы достать секретный ключ)

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // Логика регистрации
        public async Task<string> RegisterAsync(UserDto dto)
        {
            // AnyAsync проверяет, есть ли уже в базе пользователь с таким же именем (чтобы не было дубликатов)
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
                return "User already exists";

            // Создаем нового пользователя. В реальном проекте пароль нужно хешировать (например, через BCrypt),
            // но для экзамена оставляем так, чтобы не усложнять.
            var user = new User { Username = dto.Username, PasswordHash = dto.Password };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return "User registered successfully";
        }

        // Логика входа (Логин)
        public async Task<string> LoginAsync(UserDto dto)
        {
            // Ищем пользователя, у которого совпадает и логин, и пароль
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username && u.PasswordHash == dto.Password);

            // Если не нашли - значит данные неверные
            if (user == null) return "Invalid credentials";

            // Если нашли - генерируем и отдаем ему JWT-токен (цифровой пропуск)
            return GenerateJwtToken(user);
        }

        // ПРИВАТНЫЙ метод (используется только внутри этого класса) для создания токена
        private string GenerateJwtToken(User user)
        {
            // 1. Достаем наш секретный ключ из настроек и переводим его в массив байтов
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]!));

            // 2. Выбираем алгоритм шифрования (HmacSha256) и подписываем токен нашим ключом. 
            // Это гарантирует, что клиент не сможет изменить токен у себя в браузере.
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 3. Собираем сам токен
            var token = new JwtSecurityToken(
                // Claims - это информация, вшитая в токен (в данном случае - имя пользователя)
                claims: new[] { new Claim(ClaimTypes.Name, user.Username) },
                // Время жизни токена (умрет через 2 часа)
                expires: DateTime.UtcNow.AddHours(2),
                // Прикрепляем нашу цифровую подпись
                signingCredentials: creds
            );

            // 4. Превращаем объект токена в длинную строку (eyJh...) и возвращаем контроллеру
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}