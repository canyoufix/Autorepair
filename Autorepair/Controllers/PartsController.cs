using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Autorepair.Models;

namespace Autorepair.Controllers
{
    public class PartsController : Controller
    {
        private readonly AutorepairDbContext _context;

        public PartsController(AutorepairDbContext context)
        {
            _context = context;
        }

        // GET: Parts
        public async Task<IActionResult> Index()
        {
            return View(await _context.Part.ToListAsync());
        }

        // GET: Parts/Details/5
        

        private bool PartExists(Guid id)
        {
            return _context.Part.Any(e => e.Id == id);
        }
    }
}
