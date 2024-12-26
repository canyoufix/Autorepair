using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Autorepair.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize]
	public class HomeController : Controller
	{
		[Area("Admin")]
		[Authorize]
		// GET: HomeController
		public ActionResult Index()
		{
			return View();
		}

		// GET: HomeController/NDA
		public ActionResult NDA()
		{
			return View(); // Возвращает представление NDA.cshtml из области Admin
		}
	}
}