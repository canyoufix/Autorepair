
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.Serialization;
using System;
using System.IO;
using System.Xml;
using Autorepair.Models;
using Microsoft.AspNetCore.Authorization;

namespace Autorepair.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize]
	public class XMLController : Controller
	{
		private readonly AutorepairDbContext _context;
		private readonly IConfiguration _configuration;

		public string xmlFilePathCars = string.Empty;
		public string xmlFilePathClients = string.Empty;
		public string xmlFilePathEmployees = string.Empty;
		public string xmlFilePathOrders = string.Empty;
		public string xmlFilePathParts = string.Empty;
		public string xmlFilePathServices = string.Empty;

		public XMLController(AutorepairDbContext context, IConfiguration configuration)
		{
			_context = context;
			_configuration = configuration;
			CreateAllFiles();
		}


		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Export()
		{
			CreateDirectory();
			CreateAllFiles();


			SerializeData(_context.Set<Car>().AsNoTracking().ToList(), xmlFilePathCars);
			SerializeData(_context.Set<Client>().AsNoTracking().ToList(), xmlFilePathClients);
			SerializeData(_context.Set<Employee>().AsNoTracking().ToList(), xmlFilePathEmployees);
			SerializeData(_context.Set<Order>().AsNoTracking().ToList(), xmlFilePathOrders);
			SerializeData(_context.Set<Part>().AsNoTracking().ToList(), xmlFilePathParts);
			SerializeData(_context.Set<Service>().AsNoTracking().ToList(), xmlFilePathServices);

			return View("Index"); // Или другое представление для отображения сообщения
		}


		public IActionResult DropTable()
		{
			ClearAllTable();
			return View("Index");
		}


		public void ClearTable<T>(DbSet<T> dbSet) where T : class
		{
			try
			{
				dbSet.RemoveRange(dbSet);
				_context.SaveChanges();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Ошибка при очистке таблицы {typeof(T).Name}: {ex.Message}"); // Добавлено для обработки ошибок
			}

		}

		public void ClearAllTable()
		{
			ClearTable(_context.Car);
			ClearTable(_context.Client);
			ClearTable(_context.Employee);
			ClearTable(_context.Order);
			ClearTable(_context.Part);
			ClearTable(_context.Service);
		}


		public void CreateDirectory() //Добавили параметр basePath с значением по умолчанию
		{
			string DatabaseBackupPath = _configuration["DatabaseBackupPath"];
			if (DatabaseBackupPath != null && !Directory.Exists(DatabaseBackupPath))
			{
				Directory.CreateDirectory(DatabaseBackupPath);
			}
		}


		public void CreateEmptyFile(string filePath)
		{
			if (!System.IO.File.Exists(filePath))
			{
				using (FileStream uploadFileStream = System.IO.File.Create(filePath)) { }
			}
		}


		public void CreateAllFiles()
		{

			string DatabaseBackupPath = _configuration["DatabaseBackupPath"];

			xmlFilePathCars = Path.Combine(DatabaseBackupPath, "CarTable.xml");
			xmlFilePathClients = Path.Combine(DatabaseBackupPath, "ClientTable.xml");
			xmlFilePathEmployees = Path.Combine(DatabaseBackupPath, "EmployeeTable.xml");
			xmlFilePathOrders = Path.Combine(DatabaseBackupPath, "OrderTable.xml");
			xmlFilePathParts = Path.Combine(DatabaseBackupPath, "PartTable.xml");
			xmlFilePathServices = Path.Combine(DatabaseBackupPath, "ServiceTable.xml");

			CreateEmptyFile(xmlFilePathCars);
			CreateEmptyFile(xmlFilePathClients);
			CreateEmptyFile(xmlFilePathEmployees);
			CreateEmptyFile(xmlFilePathOrders);
			CreateEmptyFile(xmlFilePathParts);
			CreateEmptyFile(xmlFilePathServices);
		}

		private void SerializeData<T>(List<T> data, string filePath)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
			using (var writer = new StreamWriter(filePath))
			{
				try
				{
					serializer.Serialize(writer, data);
				}
				catch (Exception ex)
				{
					// Обработка ошибок сериализации
					Console.WriteLine($"Ошибка сериализации: {ex.Message}");
					// Можно добавить логирование или другое действие
				}
			}
		}

		private void DeserializeData<T>(string filePath, DbSet<T> dbSet) where T : class
		{
			if (!System.IO.File.Exists(filePath))
			{
				throw new FileNotFoundException($"Файл {filePath} не найден.");
			}

			XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
			using (StreamReader reader = new StreamReader(filePath))
			{
				try
				{
					List<T> importedData = (List<T>)serializer.Deserialize(reader);
					dbSet.AddRange(importedData);
				}
				catch (Exception ex)
				{
					throw new Exception($"Ошибка десериализации файла {filePath}: {ex.Message}", ex); // Передаем исходное исключение
				}
			}
		}

		public IActionResult Import()
		{
			try
			{
				ClearAllTable();

				// Проверка каждого файла перед десериализацией
				if (!FileExistsAndHasContent(xmlFilePathCars))
				{
					ViewBag.Message = "Файл с автомобилями пуст или не существует.";
					return View("Index");
				}
				DeserializeData(xmlFilePathCars, _context.Car);

				if (!FileExistsAndHasContent(xmlFilePathClients))
				{
					ViewBag.Message = "Файл с клиентами пуст или не существует.";
					return View("Index");
				}
				DeserializeData(xmlFilePathClients, _context.Client);

				if (!FileExistsAndHasContent(xmlFilePathEmployees))
				{
					ViewBag.Message = "Файл с сотрудниками пуст или не существует.";
					return View("Index");
				}
				DeserializeData(xmlFilePathEmployees, _context.Employee);

				if (!FileExistsAndHasContent(xmlFilePathOrders))
				{
					ViewBag.Message = "Файл с заказами пуст или не существует.";
					return View("Index");
				}
				DeserializeData(xmlFilePathOrders, _context.Order);

				if (!FileExistsAndHasContent(xmlFilePathParts))
				{
					ViewBag.Message = "Файл с запчастями пуст или не существует.";
					return View("Index");
				}
				DeserializeData(xmlFilePathParts, _context.Part);

				if (!FileExistsAndHasContent(xmlFilePathServices))
				{
					ViewBag.Message = "Файл с услугами пуст или не существует.";
					return View("Index");
				}
				DeserializeData(xmlFilePathServices, _context.Service);

				_context.SaveChanges();
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", $"Ошибка при импорте данных: {ex.Message}");
				ViewBag.Message = $"Ошибка при импорте данных: {ex.Message}";
				return View("Index");
			}

			return View("Index");
		}

		// Метод для проверки существования файла и его содержимого
		private bool FileExistsAndHasContent(string filePath)
		{
			return System.IO.File.Exists(filePath) && new FileInfo(filePath).Length > 0;
		}

	}
}