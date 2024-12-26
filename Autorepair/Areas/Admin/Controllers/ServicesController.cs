using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Autorepair.Models;
using Autorepair.Services;

namespace Autorepair.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ServicesController : Controller
    {
        private readonly AutorepairDbContext _context;

        public ServicesController(AutorepairDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Services
        public async Task<IActionResult> Index()
        {
            var autorepairDbContext = _context.Service.Include(s => s.Car).Include(s => s.Part);
            return View(await autorepairDbContext.ToListAsync());
        }

        // GET: Admin/Services/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.Service
                .Include(s => s.Car)
                .Include(s => s.Part)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

		// GET: Admin/Services/Create
		// GET: Admin/Services/Create
		public IActionResult Create()
		{
			ViewData["CarId"] = new SelectList(_context.Car, "Id", "CarNumber");
			ViewData["PartId"] = new SelectList(_context.Part, "Id", "Name");
			return View();
		}

		// POST: Admin/Services/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,CarId,PartId,ServiceDescription,ServiceCost,ServiceCompletionDate")] Service service)
		{
			if (ModelState.IsValid)
			{
				// Подтягиваем стоимость из таблицы Part
				var part = await _context.Part.FindAsync(service.PartId);
				if (part != null)
				{
					// Если часть найдена, устанавливаем стоимость
					service.ServiceCost = part.Price;
				}

				service.Id = Guid.NewGuid();
				_context.Add(service);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}

			// Если модель невалидна, повторно загружаем данные для формы
			ViewData["CarId"] = new SelectList(_context.Car, "Id", "CarNumber", service.CarId);
			ViewData["PartId"] = new SelectList(_context.Part, "Id", "Name", service.PartId);
			return View(service);
		}

		// GET: Admin/Services/Edit/5
		// GET: Admin/Services/Edit/5
		public async Task<IActionResult> Edit(Guid? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var service = await _context.Service.FindAsync(id);
			if (service == null)
			{
				return NotFound();
			}

			// Задаем ViewData для списка автомобилей и деталей (если необходимо)
			ViewData["CarId"] = new SelectList(_context.Car, "Id", "CarNumber", service.CarId);
			ViewData["PartId"] = new SelectList(_context.Part, "Id", "Name", service.PartId);

			return View(service);
		}

		// POST: Admin/Services/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(Guid id, [Bind("Id,CarId,PartId,ServiceDescription,ServiceCost,ServiceCompletionDate")] Service service)
		{
			if (id != service.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					// Перед тем как обновить запись, подтягиваем стоимость из таблицы Part
					var part = await _context.Part.FindAsync(service.PartId);
					if (part != null)
					{
						// Устанавливаем стоимость, если она связана с Part
						service.ServiceCost = part.Price;
					}

					_context.Update(service);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!ServiceExists(service.Id))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return RedirectToAction(nameof(Index));
			}

			// Если модель невалидна, повторно загружаем данные для формы
			ViewData["CarId"] = new SelectList(_context.Car, "Id", "CarNumber", service.CarId);
			ViewData["PartId"] = new SelectList(_context.Part, "Id", "Name", service.PartId);

			return View(service);
		}



		// GET: Admin/Services/Delete/5
		public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.Service
                .Include(s => s.Car)
                .Include(s => s.Part)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        // POST: Admin/Services/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var service = await _context.Service.FindAsync(id);
            if (service != null)
            {
                _context.Service.Remove(service);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

		public async Task<IActionResult> DeleteByCarNumber(string carNumber)
		{
			if (string.IsNullOrEmpty(carNumber))
			{
				return RedirectToAction(nameof(Index));  // Если номер автомобиля не указан, перенаправляем на индекс
			}

			// Конвертируем госномер в латиницу и кириллицу
			string convertedCarNumber = StringUtils.ConvertToCyrillic(carNumber);  // Преобразуем в кириллицу
			string reverseConvertedCarNumber = StringUtils.ConvertToLatin(convertedCarNumber);  // Преобразуем обратно в латиницу

			// Находим все записи в таблице Service, где номер машины совпадает с введенным, а также с вариантом в кириллице и латинице
			var servicesToDelete = await _context.Service
				.Where(s => s.Car.CarNumber == carNumber
							|| s.Car.CarNumber == convertedCarNumber
							|| s.Car.CarNumber == reverseConvertedCarNumber)
				.ToListAsync();

			if (servicesToDelete.Any())
			{
				_context.Service.RemoveRange(servicesToDelete);  // Удаляем все записи для данного номера автомобиля
				await _context.SaveChangesAsync();  // Сохраняем изменения в базе данных
			}

			return RedirectToAction(nameof(Index));  // Перенаправляем на индекс после удаления
		}


		// Получить все заказы с датами начала ремонта
		public IActionResult GetOrdersForInvoice()
		{
			var orders = _context.Order
				.Select(o => new
				{
					o.Id,  // ID заказа
					o.CarId,  // ID автомобиля
					o.Car.CarNumber,  // Номер автомобиля
					o.OrderDate  // Дата начала ремонта
				})
				.ToList();

			return Json(orders);
		}

		// Рассчитать стоимость ремонта для выбранного заказа
		public IActionResult GenerateInvoice(Guid orderId, bool isCompleted)
		{
			// Получаем заказ по его ID
			var order = _context.Order
				.Where(o => o.Id == orderId)
				.FirstOrDefault();

			if (order == null)
			{
				return Json(new { success = false, message = "Заказ не найден." });
			}

			// Получаем следующую дату начала ремонта для того же автомобиля, если она существует
			var nextOrder = _context.Order
				.Where(o => o.CarId == order.CarId && o.OrderDate > order.OrderDate)
				.OrderBy(o => o.OrderDate)
				.FirstOrDefault();

			// Если следующего ремонта нет, используем текущую дату
			DateTime endDate = nextOrder?.OrderDate ?? DateTime.Now;

			// Получаем все сервисы, где дата завершения ремонта попадает в интервал между началом текущего и следующим ремонтом
			var services = _context.Service
				.Where(s => s.CarId == order.CarId && s.ServiceCompletionDate >= order.OrderDate && s.ServiceCompletionDate < endDate)
				.ToList();

			// Считаем общую стоимость
			decimal totalCost = services.Sum(s => s.ServiceCost);

			// Обновляем таблицу Orders
			order.TotalCost = (int?)totalCost; // Здесь не нужно преобразовывать в int?
			order.CompletionDate = DateTime.Now;
			order.IsCompleted = isCompleted;  // Используем значение, переданное с клиента

			_context.SaveChanges();  // Сохраняем изменения

			return Json(new { success = true, totalCost });
		}






		// Метод для завершения работы с машиной (обновление статуса)
		[HttpPost]
		public IActionResult CompleteOrder(Guid carId, bool isCompleted)
		{
			var car = _context.Car.FirstOrDefault(c => c.Id == carId);
			if (car == null)
			{
				return Json(new { success = false, message = "Машина не найдена" });
			}

			// Получаем все сервисы для машины
			var services = _context.Service.Where(s => s.CarId == carId).ToList();

			// Обновляем статус завершения ремонта по всем сервисам
			foreach (var service in services)
			{
				if (isCompleted)
				{
					service.ServiceCompletionDate = DateTime.Now; // Устанавливаем дату завершения
				}
				else
				{
					service.ServiceCompletionDate = null; // Если ремонт не завершен, то очищаем дату
				}
			}

			_context.SaveChanges();

			return Json(new { success = true });
		}

		private bool ServiceExists(Guid id)
        {
            return _context.Service.Any(e => e.Id == id);
        }
    }
}
