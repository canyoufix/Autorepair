using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Autorepair.Models;
using Autorepair.Services;

namespace Autorepair.Controllers
{
	public class OrdersController : Controller
	{
		private readonly AutorepairDbContext _context;

		public OrdersController(AutorepairDbContext context)
		{
			_context = context;
		}

		public IActionResult Index(string search)
		{
			// Проверяем, если госномер был введен
			if (!string.IsNullOrEmpty(search))
			{
				// Сохраняем госномер в сессии
				HttpContext.Session.SetString("CarNumberSearch", search);

				// Конвертируем госномер в латиницу и кириллицу
				string convertedSearch = StringUtils.ConvertToCyrillic(search);
				string reverseConvertedSearch = StringUtils.ConvertToLatin(convertedSearch);

				// Фильтруем данные по госномеру (с учетом латиницы и кириллицы)
				var orders = _context.Order
					.Where(o => o.Car.CarNumber == search || o.Car.CarNumber == convertedSearch || o.Car.CarNumber == reverseConvertedSearch)
					.Include(o => o.Car)  // Подключаем данные о машине
					.Include(o => o.Employee)  // Подключаем данные о сотруднике
					.AsQueryable();

				return View(orders.ToList());
			}
			else
			{
				// Если госномер не введен, проверяем, есть ли сохраненный госномер в сессии
				string savedSearch = HttpContext.Session.GetString("CarNumberSearch");

				if (!string.IsNullOrEmpty(savedSearch))
				{
					// Если есть сохраненный госномер, фильтруем по нему
					string convertedSearch = StringUtils.ConvertToCyrillic(savedSearch);
					string reverseConvertedSearch = StringUtils.ConvertToLatin(convertedSearch);

					var orders = _context.Order
						.Where(o => o.Car.CarNumber == savedSearch || o.Car.CarNumber == convertedSearch || o.Car.CarNumber == reverseConvertedSearch)
						.Include(o => o.Car)
						.Include(o => o.Employee)
						.AsQueryable();

					return View(orders.ToList());
				}

				// Если госномер не найден в сессии, возвращаем пустую таблицу
				return View(Enumerable.Empty<Order>().ToList());
			}
		}


		// GET: Orders/Details/5
		public async Task<IActionResult> Details(Guid? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var order = await _context.Order
				.Include(o => o.Car)
				.Include(o => o.Employee)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (order == null)
			{
				return NotFound();
			}

			return View(order);
		}

	}
}
