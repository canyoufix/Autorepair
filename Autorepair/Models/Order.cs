using System.ComponentModel.DataAnnotations;

namespace Autorepair.Models
{
	public class Order
	{
		public Guid Id { get; set; }

		[Display(Name = "Гос номер", Order = 1)]
		public Guid CarId { get; set; }

		[Display(Name = "Гос номер", Order = 1)]
		public Car? Car { get; set; }

		[Display(Name = "Сотрудник", Order = 2)]
		public Guid? EmployeeId { get; set; } // Внешний ключ на таблицу сотрудников

		[Display(Name = "Сотрудник", Order = 2)]
		public Employee? Employee { get; set; } // Навигационное свойство

		[Display(Name = "Дата начала ремонта", Order = 3)]
		public DateTime? OrderDate { get; set; }

		[Display(Name = "Дата завершения ремонта", Order = 4)]
		public DateTime? CompletionDate { get; set; }

		[Display(Name = "Итоговая стоимость", Order = 5)]
		public int? TotalCost { get; set; }

		[Display(Name = "Ремонт завершен", Order = 6)]
		public bool IsCompleted { get; set; } = false;


		public List<Service> Services { get; set; } = new List<Service>();
	}

}
