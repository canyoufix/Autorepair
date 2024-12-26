using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

public class AuthService
{
	private readonly AuthDbContext _context;

	public AuthService(AuthDbContext context)
	{
		_context = context;
	}

	public async Task<bool> ValidateUserAsync(string email, string password)
	{
		var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
		if (user == null)
		{
			return false;
		}

		// Сравнение пароля с хешем в базе данных
		return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
	}

	public async Task SignInAsync(HttpContext httpContext, string email)
	{
		// Получаем пользователя
		var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
		if (user == null)
		{
			return;
		}

		// Создаем ClaimsIdentity для аутентификации
		var claims = new List<Claim>
		{
			new Claim(ClaimTypes.Name, user.Email),
			new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
		};

		var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
		var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

		// Вход в систему и создание куки
		await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
	}

	public async Task SignOutAsync(HttpContext httpContext)
	{
		// Выход из системы и удаление куки
		await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
	}
}
