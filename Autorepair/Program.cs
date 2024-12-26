using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<AuthService>();
builder.Services.AddDbContext<AutorepairDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AutorepairDbContext") ?? 
	throw new InvalidOperationException("Connection string 'AutorepairDbContext' not found.")));

builder.Services.AddDbContext<AuthDbContext>(options =>
	options.UseNpgsql(builder.Configuration.GetConnectionString("AuthDbContext") ??
	throw new InvalidOperationException("Connection string 'AutorepairDbContext' not found.")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>
	{
		options.LoginPath = "/Account/Login";   // Путь к странице входа
		options.AccessDeniedPath = "/Account/AccessDenied";  // Путь, если нет прав
		options.SlidingExpiration = true;   // Периодическая проверка истечения срока действия сессии
		options.ExpireTimeSpan = TimeSpan.FromMinutes(10); // Время жизни куки
	});

builder.Services.AddDistributedMemoryCache();  // Использование памяти для хранения сессий
builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(30);  // Время жизни сессии
	options.Cookie.HttpOnly = true;  // Чтобы cookie использовался только для HTTP-запросов
	options.Cookie.IsEssential = true;  // Cookie обязательна для работы сессий
});

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

var defaultCulture = new CultureInfo("ru-RU");
var localizationOptions = new RequestLocalizationOptions
{
	DefaultRequestCulture = new RequestCulture(defaultCulture),
	SupportedCultures = new List<CultureInfo> { defaultCulture },
	SupportedUICultures = new List<CultureInfo> { defaultCulture }
};

app.UseRequestLocalization(localizationOptions);


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseSession();



app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();   // Включение аутентификации
app.UseAuthorization();    // Включение авторизации

app.MapStaticAssets();


app.MapControllerRoute(
	name: "areas",
	pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"); // Подключение областей

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}"); // Обычный маршрут


app.Run();
