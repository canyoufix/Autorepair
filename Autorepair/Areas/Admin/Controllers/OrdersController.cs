using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Autorepair.Models;
using Autorepair.Services;
using Microsoft.AspNetCore.Authorization;

namespace Autorepair.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize]
	public class OrdersController : Controller
    {
        private readonly AutorepairDbContext _context;

        public OrdersController(AutorepairDbContext context)
        {
            _context = context;
        }

		// GET: Admin/Orders
		public IActionResult Index(string search)
		{
			// Конвертируем введенный госномер в латиницу и кириллицу
			string convertedSearch = string.IsNullOrEmpty(search) ? "" : StringUtils.ConvertToCyrillic(search);
			string reverseConvertedSearch = string.IsNullOrEmpty(search) ? "" : StringUtils.ConvertToLatin(convertedSearch);

			// Получаем все заказы, а затем фильтруем по введенному номеру
			var orders = _context.Order
				.Include(o => o.Car)  // Подключаем данные о машине
				.Include(o => o.Employee)  // Подключаем данные о сотруднике
				.AsQueryable();

			// Если введен номер, фильтруем по нему
			if (!string.IsNullOrEmpty(search))
			{
				orders = orders.Where(o => o.Car.CarNumber.Contains(search) ||
											o.Car.CarNumber.Contains(convertedSearch) ||
											o.Car.CarNumber.Contains(reverseConvertedSearch));
			}

			return View(orders.ToList());
		}

		// GET: Admin/Orders/Details/5
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

        // GET: Admin/Orders/Create
        public IActionResult Create()
        {
            ViewData["CarId"] = new SelectList(_context.Car, "Id", "CarNumber");
            ViewData["EmployeeId"] = new SelectList(_context.Employee, "Id", "LastName");
            return View();
        }

        // POST: Admin/Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CarId,EmployeeId,OrderDate,CompletionDate,TotalCost,IsCompleted")] Order order)
        {
            if (ModelState.IsValid)
            {
                order.Id = Guid.NewGuid();
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CarId"] = new SelectList(_context.Car, "Id", "CarNumber", order.CarId);
            ViewData["EmployeeId"] = new SelectList(_context.Employee, "Id", "LastName", order.EmployeeId);
            return View(order);
        }

        // GET: Admin/Orders/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["CarId"] = new SelectList(_context.Car, "Id", "CarNumber", order.CarId);
            ViewData["EmployeeId"] = new SelectList(_context.Employee, "Id", "LastName", order.EmployeeId);
            return View(order);
        }

        // POST: Admin/Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,CarId,EmployeeId,OrderDate,CompletionDate,TotalCost,IsCompleted")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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
            ViewData["CarId"] = new SelectList(_context.Car, "Id", "CarNumber", order.CarId);
            ViewData["EmployeeId"] = new SelectList(_context.Employee, "Id", "LastName", order.EmployeeId);
            return View(order);
        }

        // GET: Admin/Orders/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
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

        // POST: Admin/Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var order = await _context.Order.FindAsync(id);
            if (order != null)
            {
                _context.Order.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(Guid id)
        {
            return _context.Order.Any(e => e.Id == id);
        }
    }
}
