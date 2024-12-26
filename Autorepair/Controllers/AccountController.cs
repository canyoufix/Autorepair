using Autorepair.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class AccountController : Controller
{
	private readonly AuthService _authService;
	public AccountController(AuthService authService)
	{
		_authService = authService;
	}


	[HttpGet]
	public IActionResult Index()
	{
		return RedirectToAction("Login");
	}

	public IActionResult Login()
	{
		return View();
	}

	public async Task<IActionResult> AddTestUser()
	{
		var newUser = new User
		{
			Email = "admin@example.com",
			PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123") // Захешируйте пароль
		};

		// Получаем контекст через DI
		using (var scope = HttpContext.RequestServices.CreateScope())
		{
			var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
			dbContext.Users.Add(newUser);
			await dbContext.SaveChangesAsync();
		}

		return Content("Тестовый пользователь добавлен: admin@example.com / admin123");
	}


	// Обработка входа
	[HttpPost]
	public async Task<IActionResult> Login(string email, string password)
	{
		if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
		{
			ModelState.AddModelError("", "Пожалуйста, введите Email и Пароль.");
			return View();
		}

		var isValidUser = await _authService.ValidateUserAsync(email, password);
		if (!isValidUser)
		{
			ModelState.AddModelError("", "Неверный Email или Пароль.");
			return View();
		}

		// Вход пользователя
		await _authService.SignInAsync(HttpContext, email);

		return RedirectToAction("Index", "Home", new { area = "Admin" });
	}

	// Страница выхода
	[HttpPost]
	public async Task<IActionResult> Logout()
	{
		// Выход пользователя и удаление всех куки
		await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

		// Перенаправление на страницу логина
		return RedirectToAction("Login", "Account");
	}


}